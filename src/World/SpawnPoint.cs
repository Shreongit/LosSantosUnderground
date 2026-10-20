using GTA.Math;

namespace LosSantosUnderground.World
{
    public class SpawnPoint
    {
        public string Id { get; set; }

        public Vector3 Position { get; set; }

        public float Heading { get; set; }

        public SpawnPoint()
        {
        }

        public SpawnPoint(
            string id,
            Vector3 position,
            float heading)
        {
            Id = id;
            Position = position;
            Heading = heading;
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Id);
        }
    }
}