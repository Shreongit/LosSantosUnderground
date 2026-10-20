using System;
using GTA.Math;

namespace LosSantosUnderground.Police
{
    public class PoliceIncident
    {
        public DateTime TriggeredAt { get; private set; }

        public Vector3 Location { get; private set; }

        public float DetectionDistance { get; private set; }

        public int WantedLevel { get; private set; }

        public bool RaceAborted { get; private set; }

        public PoliceIncident(
            Vector3 location,
            float detectionDistance,
            int wantedLevel)
        {
            TriggeredAt = DateTime.UtcNow;
            Location = location;
            DetectionDistance = detectionDistance;
            WantedLevel = wantedLevel;
            RaceAborted = false;
        }

        public void MarkRaceAborted()
        {
            RaceAborted = true;
        }
    }
}