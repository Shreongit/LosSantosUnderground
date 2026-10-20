using System;

namespace LosSantosUnderground.Progression
{
    public class ReputationData
    {
        public int Reputation { get; set; }

        public int RacesEntered { get; set; }

        public int RacesFinished { get; set; }

        public int RacesWon { get; set; }

        public int PoliceAbortCount { get; set; }

        public int TotalMoneyWon { get; set; }

        public DateTime LastRaceTime { get; set; }

        public ReputationData()
        {
            Reputation = 0;
            RacesEntered = 0;
            RacesFinished = 0;
            RacesWon = 0;
            PoliceAbortCount = 0;
            TotalMoneyWon = 0;
            LastRaceTime = DateTime.MinValue;
        }

        public void RegisterEntry()
        {
            RacesEntered++;
            LastRaceTime =
                DateTime.UtcNow;
        }

        public void RegisterFinish(
            bool won)
        {
            RacesFinished++;

            if (won)
                RacesWon++;

            LastRaceTime =
                DateTime.UtcNow;
        }

        public void RegisterPoliceAbort()
        {
            PoliceAbortCount++;
            LastRaceTime =
                DateTime.UtcNow;
        }

        public void RegisterMoneyWon(
            int amount)
        {
            if (amount <= 0)
                return;

            TotalMoneyWon =
                Math.Min(
                    int.MaxValue,
                    TotalMoneyWon + amount);
        }

        public int GetWinRate()
        {
            if (RacesFinished <= 0)
                return 0;

            return (int)(
                (RacesWon /
                 (float)RacesFinished) *
                100.0f);
        }

        public void Reset()
        {
            Reputation = 0;
            RacesEntered = 0;
            RacesFinished = 0;
            RacesWon = 0;
            PoliceAbortCount = 0;
            TotalMoneyWon = 0;
            LastRaceTime =
                DateTime.MinValue;
        }
    }
}