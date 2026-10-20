using GTA.Math;

namespace LosSantosUnderground.Races
{
    public class RaceCheckpoint
    {
        public int Index { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 Direction { get; set; }

        public float Radius { get; set; } = 15.0f;

        public bool IsFinish { get; set; }

        public RaceCheckpoint()
        {
        }

        public RaceCheckpoint(
            int index,
            Vector3 position,
            Vector3 direction,
            float radius = 15.0f,
            bool isFinish = false)
        {
            Index = index;
            Position = position;
            Direction = direction;
            Radius = radius;
            IsFinish = isFinish;
        }

        public bool IsPlayerCrossingCorrectly(Vector3 playerPosition)
        {
            Vector3 offset = playerPosition - Position;

            if (offset.Length() > Radius)
                return false;

            if (Direction.Length() <= 0.001f)
                return true;

            Vector3 normalizedDirection = Direction.Normalized;

            return Vector3.Dot(offset.Normalized, normalizedDirection) <= 0.5f;
        }

        public bool IsWithinRadius(Vector3 position)
        {
            return position.DistanceTo(Position) <= Radius;
        }
    }
}