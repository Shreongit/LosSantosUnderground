using System;
using GTA;
using GTA.Math;
using GTA.Native;

namespace LosSantosUnderground.Police
{
    public class PoliceDetection
    {
        private const float DetectionRadius = 85.0f;
        private const float MinimumRaceSpeed = 18.0f;
        private const int DetectionIntervalMilliseconds = 2500;

        private DateTime _nextDetectionCheck =
            DateTime.UtcNow.AddSeconds(5);

        public bool CanPoliceDetectRace()
        {
            Ped player = Game.Player.Character;

            return player != null &&
                   player.Exists() &&
                   !player.IsDead;
        }

        public bool IsPoliceNearby()
        {
            Ped player = Game.Player.Character;

            if (player == null ||
                !player.Exists())
            {
                return false;
            }

            Vehicle[] vehicles =
                GTA.World.GetNearbyVehicles(
                    player.Position,
                    DetectionRadius);

            if (vehicles == null)
                return false;

            foreach (Vehicle vehicle in vehicles)
            {
                if (vehicle == null ||
                    !vehicle.Exists() ||
                    vehicle.IsDead)
                {
                    continue;
                }

                if (IsPoliceVehicle(vehicle))
                    return true;
            }

            return false;
        }

        public bool ShouldPoliceDetect(Random random)
        {
            if (random == null)
                return false;

            if (!CanPoliceDetectRace())
                return false;

            /*
             * Do not perform a detection check every frame.
             */
            if (DateTime.UtcNow < _nextDetectionCheck)
                return false;

            _nextDetectionCheck =
                DateTime.UtcNow.AddMilliseconds(
                    DetectionIntervalMilliseconds);

            /*
             * Police must actually be nearby.
             */
            if (!IsPoliceNearby())
                return false;

            Ped player =
                Game.Player.Character;

            if (!player.IsInVehicle())
                return false;

            Vehicle vehicle =
                player.CurrentVehicle;

            if (vehicle == null ||
                !vehicle.Exists())
            {
                return false;
            }

            float speed =
                vehicle.Speed;

            if (speed < MinimumRaceSpeed)
                return false;

            float speedFactor =
                Math.Min(
                    1.0f,
                    (speed - MinimumRaceSpeed) / 25.0f);

            float detectionChance =
                0.08f +
                (speedFactor * 0.17f);

            return random.NextDouble() <
                   detectionChance;
        }

        public bool IsPoliceVehicle(
            Vehicle vehicle)
        {
            if (vehicle == null ||
                !vehicle.Exists())
            {
                return false;
            }

            try
            {
                int vehicleClass =
                    Function.Call<int>(
                        Hash.GET_VEHICLE_CLASS,
                        vehicle.Handle);

                if (vehicleClass == 18)
                    return true;
            }
            catch
            {
            }

            return IsKnownPoliceModel(
                vehicle.Model.Hash);
        }

        private bool IsKnownPoliceModel(
            int modelHash)
        {
            return modelHash ==
                       StringHash.AtStringHash(
                           "police", 0) ||
                   modelHash ==
                       StringHash.AtStringHash(
                           "police2", 0) ||
                   modelHash ==
                       StringHash.AtStringHash(
                           "police3", 0) ||
                   modelHash ==
                       StringHash.AtStringHash(
                           "police4", 0) ||
                   modelHash ==
                       StringHash.AtStringHash(
                           "policeb", 0) ||
                   modelHash ==
                       StringHash.AtStringHash(
                           "policet", 0) ||
                   modelHash ==
                       StringHash.AtStringHash(
                           "sheriff", 0) ||
                   modelHash ==
                       StringHash.AtStringHash(
                           "sheriff2", 0) ||
                   modelHash ==
                       StringHash.AtStringHash(
                           "fbi", 0) ||
                   modelHash ==
                       StringHash.AtStringHash(
                           "fbi2", 0);
        }

        public float GetNearestPoliceDistance()
        {
            Ped player =
                Game.Player.Character;

            if (player == null ||
                !player.Exists())
            {
                return float.MaxValue;
            }

            Vehicle[] vehicles =
                GTA.World.GetNearbyVehicles(
                    player.Position,
                    DetectionRadius);

            if (vehicles == null)
                return float.MaxValue;

            float nearestDistance =
                float.MaxValue;

            foreach (Vehicle vehicle in vehicles)
            {
                if (vehicle == null ||
                    !vehicle.Exists() ||
                    vehicle.IsDead)
                {
                    continue;
                }

                if (!IsPoliceVehicle(vehicle))
                    continue;

                float distance =
                    vehicle.Position.DistanceTo(
                        player.Position);

                if (distance < nearestDistance)
                    nearestDistance = distance;
            }

            return nearestDistance;
        }

        public void Reset()
        {
            _nextDetectionCheck =
                DateTime.UtcNow.AddSeconds(5);
        }
    }
}