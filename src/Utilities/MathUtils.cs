using System;
using GTA.Math;

namespace LosSantosUnderground.Utilities
{
    public static class MathUtils
    {
        public static float Clamp(
            float value,
            float minimum,
            float maximum)
        {
            if (minimum > maximum)
            {
                float temp = minimum;
                minimum = maximum;
                maximum = temp;
            }

            return Math.Max(
                minimum,
                Math.Min(
                    maximum,
                    value));
        }

        public static int Clamp(
            int value,
            int minimum,
            int maximum)
        {
            if (minimum > maximum)
            {
                int temp = minimum;
                minimum = maximum;
                maximum = temp;
            }

            return Math.Max(
                minimum,
                Math.Min(
                    maximum,
                    value));
        }

        public static float Distance(
            Vector3 first,
            Vector3 second)
        {
            return first.DistanceTo(second);
        }

        public static float Distance2D(
            Vector3 first,
            Vector3 second)
        {
            float x =
                first.X - second.X;

            float y =
                first.Y - second.Y;

            return (float)Math.Sqrt(
                (x * x) +
                (y * y));
        }

        public static Vector3 Normalize(
            Vector3 vector)
        {
            if (vector.Length() <= 0.001f)
                return Vector3.Zero;

            return vector.Normalized;
        }

        public static float Dot(
            Vector3 first,
            Vector3 second)
        {
            return Vector3.Dot(
                first,
                second);
        }

        public static float Lerp(
            float start,
            float end,
            float amount)
        {
            amount =
                Clamp(
                    amount,
                    0.0f,
                    1.0f);

            return start +
                   ((end - start) * amount);
        }

        public static bool IsNear(
            Vector3 first,
            Vector3 second,
            float radius)
        {
            if (radius < 0.0f)
                return false;

            return Distance(
                       first,
                       second) <= radius;
        }

        public static float DirectionToHeading(
            Vector3 direction)
        {
            if (direction.Length() <= 0.001f)
                return 0.0f;

            direction.Normalize();

            return (float)(
                Math.Atan2(
                    direction.Y,
                    direction.X) *
                180.0 /
                Math.PI) - 90.0f;
        }

        public static Vector3 HeadingToDirection(
            float heading)
        {
            float radians =
                (heading + 90.0f) *
                ((float)Math.PI / 180.0f);

            return new Vector3(
                (float)Math.Cos(radians),
                (float)Math.Sin(radians),
                0.0f);
        }

        public static float RandomRange(
            Random random,
            float minimum,
            float maximum)
        {
            if (random == null)
                return minimum;

            if (minimum > maximum)
            {
                float temp = minimum;
                minimum = maximum;
                maximum = temp;
            }

            return minimum +
                   ((float)random.NextDouble() *
                    (maximum - minimum));
        }

        public static int RandomRange(
            Random random,
            int minimum,
            int maximum)
        {
            if (random == null)
                return minimum;

            if (minimum > maximum)
            {
                int temp = minimum;
                minimum = maximum;
                maximum = temp;
            }

            return random.Next(
                minimum,
                maximum + 1);
        }

        public static bool IsValidVector(
            Vector3 vector)
        {
            return
                !float.IsNaN(vector.X) &&
                !float.IsNaN(vector.Y) &&
                !float.IsNaN(vector.Z) &&
                !float.IsInfinity(vector.X) &&
                !float.IsInfinity(vector.Y) &&
                !float.IsInfinity(vector.Z);
        }
    }
}