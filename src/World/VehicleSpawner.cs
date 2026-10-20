using GTA;
using GTA.Math;

namespace LosSantosUnderground.World
{
    public class VehicleSpawner
    {
        public Vehicle SpawnVehicle(
            string modelName,
            Vector3 position,
            float heading)
        {
            Model model = new Model(modelName);

            if (!model.IsInCdImage || !model.IsValid)
                return null;

            model.Request(1000);

            if (!model.IsLoaded)
                return null;

            Vehicle vehicle = Vehicle.Create(model, position, heading);

            if (vehicle != null && vehicle.Exists())
            {
                vehicle.IsPersistent = true;
                vehicle.IsEngineRunning = true;
            }

            model.MarkAsNoLongerNeeded();

            return vehicle;
        }

        public Vehicle Spawn(
            string modelName,
            Vector3 position,
            float heading)
        {
            return SpawnVehicle(modelName, position, heading);
        }

        public void Delete(Vehicle vehicle)
        {
            if (vehicle == null || !vehicle.Exists())
                return;

            vehicle.MarkAsNoLongerNeeded();
        }
    }
}