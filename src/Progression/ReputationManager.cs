using System;

namespace LosSantosUnderground.Progression
{
    public class ReputationManager
    {
        private int _reputation;

        public int Reputation =>
            _reputation;

        public int Level =>
            CalculateLevel(_reputation);

        public void AddReputation(
            int amount)
        {
            if (amount <= 0)
                return;

            _reputation =
                Math.Min(
                    int.MaxValue,
                    _reputation + amount);
        }

        public void RemoveReputation(
            int amount)
        {
            if (amount <= 0)
                return;

            _reputation =
                Math.Max(
                    0,
                    _reputation - amount);
        }

        public void SetReputation(
            int amount)
        {
            _reputation =
                Math.Max(
                    0,
                    amount);
        }

        public void RegisterRaceFinish(
            bool playerWon)
        {
            if (playerWon)
            {
                AddReputation(100);
                return;
            }

            AddReputation(25);
        }

        public void RegisterPoliceAbort()
        {
            RemoveReputation(10);
        }

        public bool HasReputation(
            int required)
        {
            return _reputation >=
                   Math.Max(0, required);
        }

        public int GetReputationForNextLevel()
        {
            int nextLevel =
                Level + 1;

            return nextLevel * 500;
        }

        public int GetLevelProgress()
        {
            int currentLevel =
                Level;

            int currentThreshold =
                currentLevel * 500;

            int nextThreshold =
                (currentLevel + 1) * 500;

            if (nextThreshold <= currentThreshold)
                return 100;

            float progress =
                (_reputation - currentThreshold) /
                (float)(nextThreshold - currentThreshold);

            progress =
                Math.Max(
                    0.0f,
                    Math.Min(
                        1.0f,
                        progress));

            return (int)(progress * 100.0f);
        }

        private int CalculateLevel(
            int reputation)
        {
            if (reputation < 500)
                return 0;

            return reputation / 500;
        }

        public void Reset()
        {
            _reputation = 0;
        }
    }
}