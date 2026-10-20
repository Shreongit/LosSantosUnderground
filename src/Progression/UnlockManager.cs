using System;
using System.Collections.Generic;
using LosSantosUnderground.Races;

namespace LosSantosUnderground.Progression
{
    public class UnlockManager
    {
        private readonly HashSet<string> _unlockedRaces;

        public UnlockManager()
        {
            _unlockedRaces =
                new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase);
        }

        public bool IsUnlocked(
            RaceDefinition race,
            int reputation)
        {
            if (race == null)
                return false;

            return IsUnlocked(
                race.Id,
                reputation);
        }

        public bool IsUnlocked(
            string raceId,
            int reputation)
        {
            if (string.IsNullOrWhiteSpace(raceId))
                return false;

            if (_unlockedRaces.Contains(raceId))
                return true;

            int requiredReputation =
                GetRequiredReputation(raceId);

            return reputation >=
                   requiredReputation;
        }

        public void Unlock(
            string raceId)
        {
            if (string.IsNullOrWhiteSpace(raceId))
                return;

            _unlockedRaces.Add(raceId);
        }

        public void Lock(
            string raceId)
        {
            if (string.IsNullOrWhiteSpace(raceId))
                return;

            _unlockedRaces.Remove(raceId);
        }

        public int GetRequiredReputation(
            string raceId)
        {
            if (string.IsNullOrWhiteSpace(raceId))
                return int.MaxValue;

            switch (raceId.ToLowerInvariant())
            {
                case "vespucci_night_run":
                    return 0;

                case "downtown_after_dark":
                    return 0;

                case "la_mesa_redline":
                    return 250;

                case "cypress_smoke":
                    return 500;

                case "vinewood_nightfall":
                    return 750;

                case "tongva_descent":
                    return 1000;

                case "sandy_storm":
                    return 1250;

                case "harmony_highway":
                    return 1500;

                case "paleto_midnight":
                    return 2000;

                case "the_long_haul":
                    return 3000;

                default:
                    return int.MaxValue;
            }
        }

        public IReadOnlyCollection<string>
            GetUnlockedRaces()
        {
            return _unlockedRaces;
        }

        public void Clear()
        {
            _unlockedRaces.Clear();
        }
    }
}