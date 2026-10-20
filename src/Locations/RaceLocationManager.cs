using System.Collections.Generic;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using LosSantosUnderground.Core;
using LosSantosUnderground.Races;

namespace LosSantosUnderground.Locations
{
    public class RaceLocationManager
    {
        private readonly List<RaceLocation> _locations =
            new List<RaceLocation>();

        private readonly GameClockManager _clockManager;
        private readonly RaceManager _raceManager;

        public IReadOnlyList<RaceLocation> Locations
        {
            get { return _locations.AsReadOnly(); }
        }

        public RaceLocationManager()
        {
            _clockManager = new GameClockManager();
            _raceManager = null;
        }

        public RaceLocationManager(
            GameClockManager clockManager,
            RaceManager raceManager)
        {
            _clockManager =
                clockManager ??
                new GameClockManager();

            _raceManager = raceManager;
        }

        public RaceLocationManager(
            RaceManager raceManager,
            GameClockManager clockManager)
        {
            _raceManager = raceManager;

            _clockManager =
                clockManager ??
                new GameClockManager();
        }

        public void LoadLocations(
            IEnumerable<RaceLocation> locations)
        {
            _locations.Clear();

            if (locations == null)
                return;

            foreach (RaceLocation location in locations)
            {
                if (location == null)
                    continue;

                if (!IsValidLocation(location))
                    continue;

                _locations.Add(location);
            }
        }

        public void AddLocation(
            RaceLocation location)
        {
            if (!IsValidLocation(location))
                return;

            _locations.Add(location);
        }

        public RaceLocation GetLocation(
            string raceId)
        {
            if (string.IsNullOrWhiteSpace(raceId))
                return null;

            foreach (RaceLocation location in _locations)
            {
                if (location == null)
                    continue;

                if (location.RaceId == raceId)
                    return location;
            }

            return null;
        }

        public RaceLocation GetNearestLocation(
            Vector3 position,
            float maxDistance)
        {
            RaceLocation nearest = null;
            float nearestDistance = maxDistance;

            foreach (RaceLocation location in _locations)
            {
                if (location == null)
                    continue;

                float distance =
                    location.Position.DistanceTo(position);

                if (distance <= nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = location;
                }
            }

            return nearest;
        }

        public bool IsPlayerNearLocation(
            RaceLocation location,
            float radius)
        {
            if (location == null ||
                radius <= 0.0f)
            {
                return false;
            }

            Ped player =
                Game.Player.Character;

            if (player == null ||
                !player.Exists())
            {
                return false;
            }

            return player.Position.DistanceTo(
                       location.Position) <= radius;
        }

        public bool IsPlayerAtRaceLocation(
            RaceLocation location)
        {
            return IsPlayerNearLocation(
                location,
                5.0f);
        }

        public IEnumerable<RaceLocation> GetAvailableLocations(
            GameClockManager clockManager)
        {
            GameClockManager clock =
                clockManager ?? _clockManager;

            if (clock == null)
                yield break;

            clock.Update();

            if (!clock.IsRaceRegistrationOpen())
                yield break;

            foreach (RaceLocation location in _locations)
            {
                if (location != null)
                    yield return location;
            }
        }

        public IEnumerable<RaceLocation> GetAvailableLocations()
        {
            return GetAvailableLocations(
                _clockManager);
        }

        public void Update()
        {
            if (_clockManager != null)
                _clockManager.Update();

            if (_clockManager == null ||
                !_clockManager.IsRaceRegistrationOpen())
            {
                return;
            }

            Ped player =
                Game.Player.Character;

            if (player == null ||
                !player.Exists())
            {
                return;
            }

            foreach (RaceLocation location in _locations)
            {
                if (location == null ||
                    !location.IsActive)
                {
                    continue;
                }

                float distance =
                    player.Position.DistanceTo(
                        location.Position);

                if (distance > 150.0f)
                    continue;

                DrawRaceMarker(
                    location,
                    distance <= location.Radius);
            }
        }

        private void DrawRaceMarker(
            RaceLocation location,
            bool nearby)
        {
            Vector3 position =
                location.Position;

            Function.Call(
                Hash.DRAW_MARKER,
                (int)MarkerType.Cylinder,
                position.X,
                position.Y,
                position.Z - 1.0f,
                0.0f,
                0.0f,
                0.0f,
                0.0f,
                0.0f,
                0.0f,
                nearby ? 2.5f : 1.8f,
                nearby ? 2.5f : 1.8f,
                nearby ? 1.0f : 0.7f,
                255,
                30,
                30,
                160,
                false,
                false,
                2,
                false,
                null,
                null,
                false);
        }

        public void HandleKeyDown(
            KeyEventArgs e)
        {
            if (e == null)
                return;

            if (e.KeyCode != Keys.E)
                return;

            if (_raceManager == null)
                return;

            if (_clockManager == null ||
                !_clockManager.IsRaceRegistrationOpen())
            {
                return;
            }

            Ped player =
                Game.Player.Character;

            if (player == null ||
                !player.Exists() ||
                player.IsDead)
            {
                return;
            }

            RaceLocation nearest =
                GetNearestLocation(
                    player.Position,
                    5.0f);

            if (nearest == null)
                return;

            RaceDefinition race =
                _raceManager.GetRace(
                    nearest.RaceId);

            if (race == null)
                return;

            if (!_raceManager.CanRegisterRace(race))
                return;

            if (_raceManager.IsRaceActive)
                return;

            _raceManager.StartRace(race);
        }

        public void Clear()
        {
            _locations.Clear();
        }

        private bool IsValidLocation(
            RaceLocation location)
        {
            if (location == null)
                return false;

            if (string.IsNullOrWhiteSpace(
                    location.Id))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(
                    location.RaceId))
            {
                return false;
            }

            return location.IsActive;
        }
    }
}