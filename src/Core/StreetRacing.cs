using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using GTA;
using LemonUI;
using LosSantosUnderground.Input;
using LosSantosUnderground.Locations;
using LosSantosUnderground.Police;
using LosSantosUnderground.Races;
using LosSantosUnderground.UI;

namespace LosSantosUnderground.Core
{
    public class StreetRacing : Script
    {
        private readonly GameClockManager _gameClockManager;
        private readonly RaceManager _raceManager;
        private readonly RaceLocationManager _locationManager;
        private readonly RaceBlipManager _blipManager;
        private readonly PoliceManager _policeManager;

        private readonly ObjectPool _menuPool;
        private readonly RaceMenu _raceMenu;
        private readonly InputManager _inputManager;

        private bool _initialized;
        private bool _f5Pressed;
        private bool _ePressed;

        public StreetRacing()
        {
            // Rendering/UI must run every frame.
            Interval = 0;

            _gameClockManager =
                new GameClockManager();

            _raceManager =
                new RaceManager(
                    _gameClockManager);

            _locationManager =
                new RaceLocationManager(
                    _gameClockManager,
                    _raceManager);

            _blipManager =
                new RaceBlipManager();

            _policeManager =
                new PoliceManager(
                    _raceManager);

            _menuPool =
                new ObjectPool();

            _raceMenu =
                new RaceMenu(
                    _menuPool,
                    _raceManager);

            _inputManager =
                new InputManager();

            _inputManager.MenuPressed +=
                OnMenuPressed;

            _inputManager.RaceInteractPressed +=
                OnRaceInteractPressed;

            _inputManager.CancelPressed +=
                OnCancelPressed;

            Tick += OnTick;
            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;

            Initialize();
        }

        private void Initialize()
        {
            try
            {
                LoadRaces();
                LoadLocations();

                _initialized = true;

                GTA.UI.Screen.ShowSubtitle(
                    string.Format(
                        "~g~Los Santos Underground loaded.~s~ Races: {0}, locations: {1}",
                        _raceManager.Races.Count,
                        _locationManager.Locations.Count),
                    6000);

                if (_raceManager.Races.Count == 0 ||
                    _locationManager.Locations.Count == 0)
                {
                    GTA.UI.Screen.ShowSubtitle(
                        "~r~No race data loaded. Check scripts/LosSantosUnderground/Data/Json/.~s~",
                        8000);
                }
            }
            catch (Exception ex)
            {
                _initialized = false;

                GTA.UI.Screen.ShowSubtitle(
                    "~r~Los Santos Underground failed to load.~s~",
                    7000);

                System.Diagnostics.Debug.WriteLine(
                    "LosSantosUnderground initialization error: " +
                    ex);
            }
        }

        private void LoadRaces()
        {
            List<RaceDefinition> races =
                RaceLoader.LoadRaces();

            if (races == null)
            {
                races =
                    new List<RaceDefinition>();
            }

            // Normalize authored checkpoints against GTA's road graph before
            // any race is started. Long-range vehicle tasks need road targets,
            // not arbitrary coordinates that can land inside walls or yards.
            NormalizeRaceRoadPositions(races);
            _raceManager.LoadRaces(races);

            System.Diagnostics.Debug.WriteLine(
                string.Format(
                    "[LosSantosUnderground] Race data: parsed {0}, accepted {1}.",
                    races.Count,
                    _raceManager.Races.Count));
        }

        private void LoadLocations()
        {
            string scriptDirectory =
                Path.GetDirectoryName(typeof(StreetRacing).Assembly.Location);

            string path =
                Path.Combine(
                    scriptDirectory ?? AppDomain.CurrentDomain.BaseDirectory,
                    "Data",
                    "Json",
                    "locations.json");

            if (!File.Exists(path))
            {
                path = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "scripts",
                    "Data",
                    "Json",
                    "locations.json");
            }

            string json = null;

            if (File.Exists(path))
            {
                json = File.ReadAllText(path);
            }
            else
            {
                using (System.IO.Stream stream =
                    typeof(StreetRacing).Assembly.GetManifestResourceStream(
                        "LosSantosUnderground.Data.Json.locations.json"))
                {
                    if (stream != null)
                    {
                        using (StreamReader reader = new StreamReader(stream))
                            json = reader.ReadToEnd();
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(json))
            {
                GTA.UI.Screen.ShowSubtitle(
                    "~r~Race location data missing (file and embedded fallback).~s~",
                    7000);
                return;
            }

            JavaScriptSerializer serializer =
                new JavaScriptSerializer();

            LocationDataContainer data =
                serializer.Deserialize<LocationDataContainer>(
                    json);

            if (data == null ||
                data.Locations == null)
            {
                return;
            }

            List<RaceLocation> locations =
                new List<RaceLocation>();

            foreach (LocationData location
                     in data.Locations)
            {
                if (location == null ||
                    location.Position == null)
                {
                    continue;
                }

                RaceLocation raceLocation =
                    new RaceLocation();

                raceLocation.Id =
                    location.Id;

                raceLocation.Name =
                    location.Name;

                raceLocation.RaceId =
                    location.RaceId;

                raceLocation.Position =
                    new GTA.Math.Vector3(
                        location.Position.X,
                        location.Position.Y,
                        location.Position.Z);
                raceLocation.Position = SnapToRoadIfNear(
                    raceLocation.Position,
                    "location " + raceLocation.Name);

                raceLocation.Radius =
                    location.Radius;

                raceLocation.BlipRadius =
                    location.BlipRadius;

                raceLocation.IsActive =
                    location.IsActive;

                locations.Add(
                    raceLocation);
            }

            _locationManager.LoadLocations(
                locations);

            _blipManager.CreateBlips(
                _locationManager.Locations);

            System.Diagnostics.Debug.WriteLine(
                string.Format(
                    "[LosSantosUnderground] Loaded {0} locations and created race blips from: {1}",
                    _locationManager.Locations.Count,
                    path));
        }

        private void NormalizeRaceRoadPositions(
            IEnumerable<RaceDefinition> races)
        {
            if (races == null)
                return;

            foreach (RaceDefinition race in races)
            {
                if (race == null || race.Checkpoints == null)
                    continue;

                foreach (RaceCheckpoint checkpoint in race.Checkpoints)
                {
                    if (checkpoint == null)
                        continue;

                    checkpoint.Position = SnapToRoadIfNear(
                        checkpoint.Position,
                        "race " + race.Name + " checkpoint " + checkpoint.Index);
                }
            }
        }

        private GTA.Math.Vector3 SnapToRoadIfNear(
            GTA.Math.Vector3 authoredPosition,
            string label)
        {
            try
            {
                GTA.Math.Vector3 roadPosition =
                    GTA.World.GetNextPositionOnStreet(authoredPosition);

                if (roadPosition == GTA.Math.Vector3.Zero)
                    return authoredPosition;

                float distance = authoredPosition.DistanceTo(roadPosition);

                // Do not silently redirect remote/off-road routes to an
                // unrelated road. Nearby nodes are safe corrections for
                // checkpoints that were authored a few metres off the road.
                if (distance <= 65.0f)
                {
                    if (distance > 2.0f)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            string.Format(
                                "[LosSantosUnderground] Snapped {0} to road node ({1:0.0}m).",
                                label,
                                distance));
                    }

                    return roadPosition;
                }

                System.Diagnostics.Debug.WriteLine(
                    string.Format(
                        "[LosSantosUnderground] WARNING: {0} is {1:0.0}m from the nearest street; left unchanged.",
                        label,
                        distance));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "[LosSantosUnderground] Road snapping failed for " + label + ": " + ex.Message);
            }

            return authoredPosition;
        }

        private void OnTick(
            object sender,
            EventArgs e)
        {
            if (!_initialized)
                return;

            _gameClockManager.Update();

            _locationManager.Update();

            _raceManager.Update();

            _policeManager.Update();

            _menuPool.Process();
        }

        private void OnKeyDown(
            object sender,
            KeyEventArgs e)
        {
            if (!_initialized ||
                e == null)
            {
                return;
            }

            if (e.KeyCode == Keys.F5)
            {
                if (_f5Pressed)
                    return;

                _f5Pressed = true;

                _inputManager.HandleKeyDown(e);

                return;
            }

            if (e.KeyCode == Keys.E)
            {
                if (_ePressed)
                    return;

                _ePressed = true;

                _inputManager.HandleKeyDown(e);

                _locationManager.HandleKeyDown(e);

                return;
            }

            if (e.KeyCode == Keys.Escape)
            {
                _inputManager.HandleKeyDown(e);
            }
        }

        private void OnKeyUp(
            object sender,
            KeyEventArgs e)
        {
            if (e == null)
                return;

            if (e.KeyCode == Keys.F5)
                _f5Pressed = false;

            if (e.KeyCode == Keys.E)
                _ePressed = false;
        }

        private void OnMenuPressed(
            object sender,
            EventArgs e)
        {
            if (!_gameClockManager.IsRaceRegistrationOpen())
            {
                GTA.UI.Screen.ShowSubtitle(
                    "~y~Street races are closed during the day.~s~",
                    3000);

                return;
            }

            if (_raceManager.IsRaceActive)
            {
                GTA.UI.Screen.ShowSubtitle(
                    "~y~A race is already active.~s~",
                    3000);

                return;
            }

            _raceMenu.Toggle();
        }

        private void OnRaceInteractPressed(
            object sender,
            EventArgs e)
        {
            if (!_gameClockManager.IsRaceRegistrationOpen())
                return;

            _locationManager.HandleKeyDown(
                new KeyEventArgs(Keys.E));
        }

        private void OnCancelPressed(
            object sender,
            EventArgs e)
        {
            _raceMenu.Hide();
        }

        private class LocationDataContainer
        {
            public List<LocationData> Locations
            {
                get;
                set;
            }
        }

        private class LocationData
        {
            public string Id
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }

            public string RaceId
            {
                get;
                set;
            }

            public PositionData Position
            {
                get;
                set;
            }

            public float Radius
            {
                get;
                set;
            }

            public float BlipRadius
            {
                get;
                set;
            }

            public bool IsActive
            {
                get;
                set;
            }
        }

        private class PositionData
        {
            public float X
            {
                get;
                set;
            }

            public float Y
            {
                get;
                set;
            }

            public float Z
            {
                get;
                set;
            }
        }
    }
}
