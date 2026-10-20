using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using LosSantosUnderground.Economy;
using LosSantosUnderground.Locations;
using LosSantosUnderground.Police;
using LosSantosUnderground.Racers;
using LosSantosUnderground.Races;
using LosSantosUnderground.UI;

namespace LosSantosUnderground.Core
{
    public class RaceSession : IDisposable
    {
        private readonly RaceDefinition _race;
        private readonly RacerManager _racerManager;
        private readonly PoliceManager _policeManager;
        private readonly MoneyManager _moneyManager;

        private RaceState _state;
        private DateTime _raceStartTime;
        private bool _playerFinished;
        private bool _disposed;
        private Blip _routeBlip;
        private int _routeCheckpointIndex = -1;

        public RaceDefinition Race
        {
            get { return _race; }
        }

        public RaceState State
        {
            get { return _state; }
        }

        public bool IsActive
        {
            get
            {
                return _state == RaceState.Preparing ||
                       _state == RaceState.Countdown ||
                       _state == RaceState.Racing;
            }
        }

        public RaceSession(
            RaceDefinition race,
            RacerManager racerManager,
            PoliceManager policeManager,
            MoneyManager moneyManager)
        {
            _race = race;
            _racerManager = racerManager;
            _policeManager = policeManager;
            _moneyManager = moneyManager;

            _state = RaceState.Waiting;
            _playerFinished = false;
        }

        public bool Start()
        {
            if (_disposed || _race == null)
                return false;

            if (_racerManager == null)
                return false;

            _state = RaceState.Preparing;
            _playerFinished = false;

            RaceUI.ShowPreparing(_race);

            return true;
        }

        public void Update()
        {
            if (_disposed)
                return;

            switch (_state)
            {
                case RaceState.Preparing:
                    UpdatePreparing();
                    break;

                case RaceState.Countdown:
                    UpdateCountdown();
                    break;

                case RaceState.Racing:
                    UpdateRacing();
                    break;

                case RaceState.Finished:
                    UpdateFinished();
                    break;

                case RaceState.Aborted:
                    UpdateAborted();
                    break;
            }
        }

        private void UpdatePreparing()
        {
            if (_racerManager == null)
            {
                AbortRace();
                return;
            }

            if (!_racerManager.HasPlayerRacer())
            {
                AbortRace();
                return;
            }

            _state = RaceState.Countdown;

            RaceUI.StartCountdown();
        }

        private void UpdateCountdown()
        {
            if (RaceUI.IsCountdownFinished())
            {
                StartRace();
            }
        }

        private void StartRace()
        {
            if (_racerManager == null)
            {
                AbortRace();
                return;
            }

            _racerManager.StartRace();

            _raceStartTime = DateTime.UtcNow;
            _playerFinished = false;
            _state = RaceState.Racing;

            RaceUI.ShowRaceStarted();
        }

        private void UpdateRacing()
        {
            if (_racerManager == null)
            {
                AbortRace();
                return;
            }

            if (_policeManager != null &&
                _policeManager.ShouldDetectRace())
            {
                AbortForPolice();
                return;
            }

            _racerManager.Update();

            UpdatePlayerProgress();
            UpdateRouteGuidance();

            if (_racerManager.HasPlayerFinished())
            {
                FinishPlayerRace();
                return;
            }

            if (_racerManager.AllRacersFinished())
            {
                FinishRace();
            }
        }

        private void UpdatePlayerProgress()
        {
            Racer playerRacer =
                _racerManager.GetPlayerRacer();

            if (playerRacer == null)
                return;

            int totalCheckpoints =
                _race.Checkpoints != null
                    ? _race.Checkpoints.Count
                    : 0;

            RaceUI.UpdateRaceHUD(
                playerRacer.Position,
                _racerManager.GetActiveRacerCount(),
                playerRacer.CurrentCheckpoint,
                totalCheckpoints);
        }

        private void UpdateRouteGuidance()
        {
            Racer playerRacer = _racerManager.GetPlayerRacer();
            if (playerRacer == null ||
                _race == null ||
                _race.Checkpoints == null ||
                _race.Checkpoints.Count == 0)
                return;

            int index = playerRacer.CurrentCheckpoint;
            if (index < 0 || index >= _race.Checkpoints.Count)
                return;

            RaceCheckpoint checkpoint = _race.Checkpoints[index];
            if (checkpoint == null)
                return;

            if (_routeBlip == null ||
                !_routeBlip.Exists() ||
                _routeCheckpointIndex != index)
            {
                if (_routeBlip != null && _routeBlip.Exists())
                {
                    Function.Call(Hash.SET_BLIP_ROUTE, _routeBlip.Handle, false);
                    _routeBlip.Delete();
                }

                _routeBlip = Blip.Create(checkpoint.Position);
                _routeCheckpointIndex = index;

                if (_routeBlip != null && _routeBlip.Exists())
                {
                    _routeBlip.Sprite = BlipSprite.Race;
                    _routeBlip.Color = BlipColor.Yellow;
                    _routeBlip.Name = "Next Race Checkpoint";
                    _routeBlip.IsShortRange = false;
                    Function.Call(Hash.SET_BLIP_ROUTE, _routeBlip.Handle, true);
                }
            }

            // Keep the active checkpoint visible in-world as well as on GPS.
            Vector3 p = checkpoint.Position;
            Function.Call(
                Hash.DRAW_MARKER,
                (int)MarkerType.Cylinder,
                p.X, p.Y, p.Z - 0.35f,
                0.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f,
                2.6f, 2.6f, 1.2f,
                255, 210, 40, 170,
                false, false, 2, false, null, null, false);
        }

        private void ClearRouteGuidance()
        {
            if (_routeBlip != null && _routeBlip.Exists())
            {
                Function.Call(Hash.SET_BLIP_ROUTE, _routeBlip.Handle, false);
                _routeBlip.Delete();
            }

            _routeBlip = null;
            _routeCheckpointIndex = -1;
        }

        private void FinishPlayerRace()
        {
            if (_playerFinished)
                return;

            _playerFinished = true;

            Racer playerRacer =
                _racerManager.GetPlayerRacer();

            if (playerRacer == null)
            {
                FinishRace();
                return;
            }

            int position =
                _racerManager.GetPlayerPosition();

            int prize =
                position == 1
                    ? _racerManager.GetPrizePool()
                    : 0;

            if (position == 1 &&
                _moneyManager != null)
            {
                _moneyManager.AddMoney(prize);
            }

            RaceUI.ShowRaceResults(
                _race,
                position,
                _racerManager.GetRacerCount(),
                prize);

            _state = RaceState.Finished;
        }

        private void FinishRace()
        {
            if (_state == RaceState.Finished)
                return;

            _state = RaceState.Finished;

            Racer playerRacer =
                _racerManager != null
                    ? _racerManager.GetPlayerRacer()
                    : null;

            int position =
                playerRacer != null
                    ? playerRacer.Position
                    : 0;

            int prize =
                position == 1 &&
                _racerManager != null
                    ? _racerManager.GetPrizePool()
                    : 0;

            RaceUI.ShowRaceResults(
                _race,
                position,
                _racerManager != null
                    ? _racerManager.GetRacerCount()
                    : 0,
                prize);
        }

        private void AbortForPolice()
        {
            _state = RaceState.Aborted;

            if (_racerManager != null)
                _racerManager.FleeFromPolice();

            if (_policeManager != null)
                _policeManager.TriggerPoliceResponse();

            RaceUI.ShowRaceAborted(
                RaceAbortReason.PoliceDetected);
        }

        private void AbortRace()
        {
            _state = RaceState.Aborted;

            if (_racerManager != null)
                _racerManager.AbortRace();

            RaceUI.ShowRaceAborted(
                RaceAbortReason.PoliceDetected);
        }

        private void UpdateFinished()
        {
        }

        private void UpdateAborted()
        {
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            ClearRouteGuidance();

            if (_racerManager != null)
                _racerManager.Dispose();

            RaceUI.Reset();

            _state = RaceState.Aborted;
        }
    }
}