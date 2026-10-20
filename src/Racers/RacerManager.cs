using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using LosSantosUnderground.Racers;
using LosSantosUnderground.Races;

namespace LosSantosUnderground.Core
{
    public class RaceManager : IDisposable
    {
        private readonly List<RaceDefinition> _races =
            new List<RaceDefinition>();

        private readonly GameClockManager _clockManager;

        private RaceSession _activeSession;
        private bool _disposed;

        public IReadOnlyList<RaceDefinition> Races
        {
            get { return _races.AsReadOnly(); }
        }

        public RaceSession ActiveSession
        {
            get { return _activeSession; }
        }

        public RaceSession CurrentSession
        {
            get { return _activeSession; }
        }

        public bool HasActiveRace
        {
            get
            {
                return _activeSession != null &&
                       _activeSession.IsActive;
            }
        }

        public bool IsRaceActive
        {
            get { return HasActiveRace; }
        }

        public RaceManager()
        {
            _clockManager = new GameClockManager();
        }

        public RaceManager(
            GameClockManager clockManager)
        {
            _clockManager =
                clockManager ??
                new GameClockManager();
        }

        public void LoadRaces(
            IEnumerable<RaceDefinition> races)
        {
            _races.Clear();

            if (races == null)
                return;

            foreach (RaceDefinition race in races)
            {
                if (race == null)
                    continue;

                if (!RaceValidator.Validate(race))
                    continue;

                _races.Add(race);
            }
        }

        public void AddRace(
            RaceDefinition race)
        {
            if (race == null)
                return;

            if (!RaceValidator.Validate(race))
                return;

            _races.Add(race);
        }

        public RaceDefinition GetRace(
            string raceId)
        {
            if (string.IsNullOrWhiteSpace(raceId))
                return null;

            foreach (RaceDefinition race in _races)
            {
                if (race == null)
                    continue;

                if (string.Equals(
                        race.Id,
                        raceId,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return race;
                }
            }

            return null;
        }

        public RaceDefinition GetRaceById(
            string raceId)
        {
            return GetRace(raceId);
        }

        public IEnumerable<RaceDefinition> GetAvailableRaces()
        {
            _clockManager.Update();

            if (!_clockManager.IsRaceRegistrationOpen())
                yield break;

            foreach (RaceDefinition race in _races)
            {
                if (race != null)
                    yield return race;
            }
        }

        public bool CanRegisterRace(
            RaceDefinition race)
        {
            if (_disposed ||
                race == null ||
                HasActiveRace)
            {
                return false;
            }

            if (!RaceValidator.Validate(race))
                return false;

            _clockManager.Update();

            return _clockManager.IsRaceRegistrationOpen();
        }

        public bool CanRegisterRace()
        {
            if (_disposed ||
                HasActiveRace)
            {
                return false;
            }

            _clockManager.Update();

            return _clockManager.IsRaceRegistrationOpen();
        }

        public bool StartRace(
            RaceDefinition race)
        {
            if (!CanRegisterRace(race))
                return false;

            Economy.MoneyManager moneyManager =
                new Economy.MoneyManager();

            if (!moneyManager.CanAfford(race.EntryFee))
            {
                GTA.UI.Screen.ShowSubtitle(
                    string.Format("~r~You need ${0:N0} to enter this race.~s~", race.EntryFee),
                    4000);
                return false;
            }

            if (!moneyManager.TryPayEntryFee(race.EntryFee))
                return false;

            RacerManager racerManager =
                new RacerManager();

            racerManager.SetRace(race);

            if (!racerManager.HasPlayerRacer())
            {
                racerManager.Dispose();
                moneyManager.Refund(race.EntryFee);
                GTA.UI.Screen.ShowSubtitle(
                    "~r~Could not prepare your race vehicle. Race entry refunded.~s~",
                    5000);
                return false;
            }

            Police.PoliceManager policeManager =
                new Police.PoliceManager(this);

            _activeSession =
                new RaceSession(
                    race,
                    racerManager,
                    policeManager,
                    moneyManager);

            if (!_activeSession.Start())
            {
                _activeSession.Dispose();
                _activeSession = null;
                moneyManager.Refund(race.EntryFee);
                return false;
            }

            return true;
        }

        public bool StartRace(
            string raceId)
        {
            RaceDefinition race =
                GetRace(raceId);

            return StartRace(race);
        }

        public void Update()
        {
            if (_disposed)
                return;

            _clockManager.Update();

            if (_activeSession == null)
                return;

            _activeSession.Update();

            if (_activeSession.State ==
                RaceState.Finished ||
                _activeSession.State ==
                RaceState.Aborted)
            {
                _activeSession.Dispose();
                _activeSession = null;
            }
        }

        public void HandleKeyDown(
            GTA.Control key)
        {
            if (_disposed)
                return;

            if (_activeSession == null)
                return;
        }

        public void AbortRace()
        {
            AbortActiveRace();
        }

        public void AbortActiveRace()
        {
            if (_activeSession == null)
                return;

            _activeSession.Dispose();
            _activeSession = null;
        }

        public void StopRace()
        {
            AbortActiveRace();
        }

        public void ClearRaces()
        {
            AbortActiveRace();
            _races.Clear();
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            AbortActiveRace();
            _races.Clear();
        }
    }
}