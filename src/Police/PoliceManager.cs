using System;
using GTA;
using LosSantosUnderground.Core;

namespace LosSantosUnderground.Police
{
    public class PoliceManager
    {
        private readonly RaceManager _raceManager;
        private readonly PoliceDetection _policeDetection;
        private readonly WantedLevelManager _wantedLevelManager;
        private readonly Random _random;

        public PoliceManager(
            RaceManager raceManager)
        {
            _raceManager = raceManager;
            _policeDetection = new PoliceDetection();
            _wantedLevelManager = new WantedLevelManager();
            _random = new Random();
        }

        public bool ShouldDetectRace()
        {
            if (_raceManager == null ||
                _raceManager.CurrentSession == null)
            {
                return false;
            }

            if (!_raceManager.CurrentSession.IsActive)
                return false;

            return _policeDetection.ShouldPoliceDetect(
                _random);
        }

        public void TriggerPoliceResponse()
        {
            _wantedLevelManager.SetRaceWantedLevel();
        }

        public void AbortRace()
        {
            if (_raceManager == null)
                return;

            _raceManager.AbortRace();
        }

        public bool IsPoliceNearby()
        {
            return _policeDetection.IsPoliceNearby();
        }

        public float GetNearestPoliceDistance()
        {
            return _policeDetection.GetNearestPoliceDistance();
        }

        public void Update()
        {
            if (_raceManager == null ||
                _raceManager.CurrentSession == null)
            {
                return;
            }

            if (!_raceManager.CurrentSession.IsActive)
                return;

            if (ShouldDetectRace())
            {
                TriggerPoliceResponse();
                AbortRace();
            }
        }
    }
}