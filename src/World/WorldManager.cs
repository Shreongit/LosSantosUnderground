using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;

namespace LosSantosUnderground.World
{
    public class WorldManager : IDisposable
    {
        private readonly List<Vehicle> _spawnedVehicles;
        private readonly VehicleSpawner _vehicleSpawner;

        private bool _disposed;

        public IReadOnlyList<Vehicle> SpawnedVehicles =>
            _spawnedVehicles;

        public WorldManager()
        {
            _spawnedVehicles =
                new List<Vehicle>();

            _vehicleSpawner =
                new VehicleSpawner();
        }

        public Vehicle SpawnRaceVehicle(
            string modelName,
            Vector3 position,
            float heading)
        {
            if (_disposed)
                return null;

            Vehicle vehicle =
                _vehicleSpawner.Spawn(
                    modelName,
                    position,
                    heading);

            if (vehicle == null ||
                !vehicle.Exists())
            {
                return null;
            }

            _spawnedVehicles.Add(vehicle);

            return vehicle;
        }

        public void RemoveVehicle(
            Vehicle vehicle)
        {
            if (vehicle == null)
                return;

            _vehicleSpawner.Delete(vehicle);
            _spawnedVehicles.Remove(vehicle);
        }

        public void CleanupRaceVehicles()
        {
            if (_disposed)
                return;

            for (int i = _spawnedVehicles.Count - 1;
                 i >= 0;
                 i--)
            {
                Vehicle vehicle =
                    _spawnedVehicles[i];

                if (vehicle != null &&
                    vehicle.Exists())
                {
                    _vehicleSpawner.Delete(
                        vehicle);
                }

                _spawnedVehicles.RemoveAt(i);
            }
        }

        public bool IsVehicleNear(
            Vector3 position,
            float radius)
        {
            if (_disposed ||
                radius <= 0.0f)
                return false;

            foreach (Vehicle vehicle
                     in _spawnedVehicles)
            {
                if (vehicle == null ||
                    !vehicle.Exists() ||
                    vehicle.IsDead)
                    continue;

                if (vehicle.Position.DistanceTo(
                        position) <= radius)
                {
                    return true;
                }
            }

            return false;
        }

        public void Update()
        {
            if (_disposed)
                return;

            for (int i = _spawnedVehicles.Count - 1;
                 i >= 0;
                 i--)
            {
                Vehicle vehicle =
                    _spawnedVehicles[i];

                if (vehicle == null ||
                    !vehicle.Exists())
                {
                    _spawnedVehicles.RemoveAt(i);
                }
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            CleanupRaceVehicles();
            _spawnedVehicles.Clear();
        }
    }
}