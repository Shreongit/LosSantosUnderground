using System;

namespace LosSantosUnderground.Racers
{
    public class RacerResult
    {
        public string RacerId { get; set; }

        public string RacerName { get; set; }

        public bool IsPlayer { get; set; }

        public int Position { get; set; }

        public int Checkpoint { get; set; }

        public TimeSpan FinishTime { get; set; }

        public bool HasFinished { get; set; }

        public RacerResult()
        {
            Position = 0;
            Checkpoint = 0;
            FinishTime = TimeSpan.Zero;
            HasFinished = false;
        }
    }
}