using System.Collections.Generic;
using GTA.Math;

namespace LosSantosUnderground.Races
{
    public static class RaceValidator
    {
        private const int MinimumCheckpoints = 2;
        private const int MinimumRacers = 2;
        private const int MaximumRacers = 6;

        public static bool Validate(
            RaceDefinition race)
        {
            if (race == null)
                return false;

            if (string.IsNullOrWhiteSpace(race.Id))
                return false;

            if (string.IsNullOrWhiteSpace(race.Name))
                return false;

            if (race.EntryFee < 0)
                return false;

            if (!IsValidRacerCount(
                    race.MaxRacers))
                return false;

            if (race.Checkpoints == null ||
                race.Checkpoints.Count < MinimumCheckpoints)
                return false;

            if (!ValidateCheckpoints(
                    race.Checkpoints))
                return false;

            if (!ValidateNightHours(
                    race))
                return false;

            return true;
        }

        private static bool ValidateCheckpoints(
            List<RaceCheckpoint> checkpoints)
        {
            if (checkpoints == null ||
                checkpoints.Count < MinimumCheckpoints)
            {
                return false;
            }

            for (int i = 0;
                 i < checkpoints.Count;
                 i++)
            {
                RaceCheckpoint checkpoint =
                    checkpoints[i];

                if (checkpoint == null)
                    return false;

                if (!IsValidVector(
                        checkpoint.Position))
                {
                    return false;
                }

                if (!IsValidVector(
                        checkpoint.Direction))
                {
                    return false;
                }

                if (checkpoint.Radius <= 0.0f)
                    return false;

                checkpoint.Index = i;
            }

            RaceCheckpoint finishCheckpoint =
                checkpoints[checkpoints.Count - 1];

            if (!finishCheckpoint.IsFinish)
                finishCheckpoint.IsFinish = true;

            return true;
        }

        private static bool ValidateNightHours(
            RaceDefinition race)
        {
            if (race == null)
                return false;

            return race.MinHour == 17 &&
                   race.MaxHour == 5;
        }

        private static bool IsValidVector(
            Vector3 vector)
        {
            return !float.IsNaN(vector.X) &&
                   !float.IsNaN(vector.Y) &&
                   !float.IsNaN(vector.Z) &&
                   !float.IsInfinity(vector.X) &&
                   !float.IsInfinity(vector.Y) &&
                   !float.IsInfinity(vector.Z);
        }

        public static bool IsValidCheckpoint(
            RaceCheckpoint checkpoint)
        {
            if (checkpoint == null)
                return false;

            if (checkpoint.Radius <= 0.0f)
                return false;

            return IsValidVector(
                checkpoint.Position) &&
                   IsValidVector(
                       checkpoint.Direction);
        }

        public static bool IsValidEntryFee(
            int entryFee)
        {
            return entryFee >= 0;
        }

        public static bool IsValidRacerCount(
            int racerCount)
        {
            return racerCount >= MinimumRacers &&
                   racerCount <= MaximumRacers;
        }

        public static bool HasValidFinishCheckpoint(
            RaceDefinition race)
        {
            if (race == null ||
                race.Checkpoints == null ||
                race.Checkpoints.Count < MinimumCheckpoints)
            {
                return false;
            }

            RaceCheckpoint finishCheckpoint =
                race.Checkpoints[
                    race.Checkpoints.Count - 1];

            return finishCheckpoint != null &&
                   finishCheckpoint.IsFinish;
        }
    }
}