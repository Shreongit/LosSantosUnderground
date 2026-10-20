using GTA;
using GTA.Math;
using GTA.Native;

namespace LosSantosUnderground.Utilities
{
    public static class VehicleUtils
    {
        public static bool IsValid(
            Vehicle vehicle)
        {
            return vehicle != null &&
                   vehicle.Exists() &&
                   !vehicle.IsDead;
        }

        public static void SetEngineRunning(
            Vehicle vehicle,
            bool running)
        {
            if (!IsValid(vehicle))
                return;

            Function.Call(
                Hash.SET_VEHICLE_ENGINE_ON,
                vehicle.Handle,
                running,
                true,
                false);
        }

        public static bool IsEngineRunning(
            Vehicle vehicle)
        {
            if (!IsValid(vehicle))
                return false;

            return Function.Call<bool>(
                Hash.GET_IS_VEHICLE_ENGINE_RUNNING,
                vehicle.Handle);
        }

        public static void SetHandbrake(
            Vehicle vehicle,
            bool enabled)
        {
            if (!IsValid(vehicle))
                return;

            Function.Call(
                Hash.SET_VEHICLE_HANDBRAKE,
                vehicle.Handle,
                enabled);
        }

        public static bool IsHandbrakeOn(
            Vehicle vehicle)
        {
            if (!IsValid(vehicle))
                return false;

            return false;
        }

        public static void Repair(
            Vehicle vehicle)
        {
            if (!IsValid(vehicle))
                return;

            Function.Call(
                Hash.SET_VEHICLE_FIXED,
                vehicle.Handle);

            Function.Call(
                Hash.SET_VEHICLE_DEFORMATION_FIXED,
                vehicle.Handle);
        }

        public static void SetPersistent(
            Vehicle vehicle,
            bool persistent)
        {
            if (!IsValid(vehicle))
                return;

            vehicle.IsPersistent = persistent;
        }

        public static float GetSpeed(
            Vehicle vehicle)
        {
            if (!IsValid(vehicle))
                return 0.0f;

            return vehicle.Speed;
        }

        public static Vector3 GetPosition(
            Vehicle vehicle)
        {
            if (!IsValid(vehicle))
                return Vector3.Zero;

            return vehicle.Position;
        }

        public static float GetDistanceTo(
            Vehicle vehicle,
            Vector3 position)
        {
            if (!IsValid(vehicle))
                return float.MaxValue;

            return vehicle.Position.DistanceTo(
                position);
        }

        public static void Delete(
            Vehicle vehicle)
        {
            if (!IsValid(vehicle))
                return;

            vehicle.MarkAsNoLongerNeeded();
        }
    }
}