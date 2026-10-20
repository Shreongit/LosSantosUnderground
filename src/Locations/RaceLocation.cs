using GTA.Math;

namespace LosSantosUnderground.Locations
{
    public class RaceLocation
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string RaceId { get; set; }

        public Vector3 Position { get; set; }

        public float Radius { get; set; } = 35.0f;

        public float BlipRadius { get; set; } = 1.0f;

        public bool IsActive { get; set; } = true;

        public RaceLocation()
        {
        }

        public RaceLocation(
            string id,
            string name,
            string raceId,
            Vector3 position,
            float radius = 35.0f)
        {
            Id = id;
            Name = name;
            RaceId = raceId;
            Position = position;
            Radius = radius;
            IsActive = true;
        }

        public bool IsPlayerNearby(
            Vector3 playerPosition)
        {
            return IsActive &&
                   playerPosition.DistanceTo(
                       Position) <= Radius;
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Id) &&
                   !string.IsNullOrWhiteSpace(Name) &&
                   !string.IsNullOrWhiteSpace(RaceId) &&
                   Radius > 0.0f;
        }
    }
}