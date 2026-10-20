using System.Collections.Generic;
using GTA.Math;

namespace LosSantosUnderground.Races
{
    public class RaceRoute
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public List<RaceCheckpoint> Checkpoints { get; set; }
            = new List<RaceCheckpoint>();

        public int CheckpointCount =>
            Checkpoints?.Count ?? 0;

        public RaceCheckpoint GetCheckpoint(int index)
        {
            if (Checkpoints == null)
                return null;

            if (index < 0 || index >= Checkpoints.Count)
                return null;

            return Checkpoints[index];
        }

        public RaceCheckpoint GetNextCheckpoint(int currentIndex)
        {
            return GetCheckpoint(currentIndex + 1);
        }

        public bool IsLastCheckpoint(int index)
        {
            return Checkpoints != null &&
                   Checkpoints.Count > 0 &&
                   index == Checkpoints.Count - 1;
        }

        public bool ContainsCheckpoint(int index)
        {
            return Checkpoints != null &&
                   index >= 0 &&
                   index < Checkpoints.Count;
        }

        public float GetApproximateDistance()
        {
            if (Checkpoints == null || Checkpoints.Count < 2)
                return 0.0f;

            float distance = 0.0f;

            for (int i = 1; i < Checkpoints.Count; i++)
            {
                Vector3 previous =
                    Checkpoints[i - 1].Position;

                Vector3 current =
                    Checkpoints[i].Position;

                distance += previous.DistanceTo(current);
            }

            return distance;
        }

        public void AddCheckpoint(RaceCheckpoint checkpoint)
        {
            if (checkpoint == null)
                return;

            checkpoint.Index = Checkpoints.Count;
            Checkpoints.Add(checkpoint);
        }

        public void Clear()
        {
            Checkpoints.Clear();
        }
    }
}