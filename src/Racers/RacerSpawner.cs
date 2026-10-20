using System;
using GTA;
using GTA.Math;
using GTA.Native;
using LosSantosUnderground.Races;

namespace LosSantosUnderground.Racers
{
    public class RacerSpawner
    {
        private readonly Random _random = new Random();

        private static readonly string[] VehicleModels =
        {
            "sultan",
            "elegy",
            "jester",
            "kuruma",
            "comet2",
            "futo",
            "buffalo",
            "schafter2",
            "massacro",
            "carbonizzare"
        };

        private const string DriverModelName = "s_m_y_dealer_01";

        public Racer SpawnPlayerRacer(
            Vector3 position,
            float heading)
        {
            Ped player = Game.Player.Character;

            if (player == null ||
                !player.Exists())
                return null;

            Vehicle vehicle =
                player.IsInVehicle()
                    ? player.CurrentVehicle
                    : null;

            // A race selected from the F5 menu may be started far from
            // the meet and while the player is on foot. Ensure there is
            // always a valid player racer before the session countdown.
            if (vehicle == null || !vehicle.Exists())
            {
                Model fallbackModel = new Model("sultan");
                if (!fallbackModel.IsInCdImage || !fallbackModel.IsValid)
                    return null;

                fallbackModel.Request(2000);
                if (!fallbackModel.IsLoaded)
                    return null;

                vehicle = Vehicle.Create(fallbackModel, position, heading);
                fallbackModel.MarkAsNoLongerNeeded();

                if (vehicle == null || !vehicle.Exists())
                    return null;

                Function.Call(
                    Hash.SET_PED_INTO_VEHICLE,
                    player.Handle,
                    vehicle.Handle,
                    (int)VehicleSeat.Driver);
            }

            vehicle.Position = position;
            vehicle.Heading = heading;
            vehicle.IsPersistent = true;

            Function.Call(
                Hash.SET_ENTITY_COORDS,
                vehicle.Handle,
                position.X,
                position.Y,
                position.Z,
                false,
                false,
                false,
                true);

            Racer racer = new Racer();

            racer.Id = "player";
            racer.Name = "Player";

            racer.AssignEntities(
                player,
                vehicle);

            SetVehicleRaceState(vehicle);

            return racer;
        }

        public Racer SpawnNpcRacer(
            string id,
            string name,
            Vector3 position,
            float heading)
        {
            string modelName =
                VehicleModels[
                    _random.Next(
                        VehicleModels.Length)];

            Model vehicleModel =
                new Model(modelName);

            if (!vehicleModel.IsInCdImage ||
                !vehicleModel.IsValid)
                return null;

            vehicleModel.Request(1000);

            if (!vehicleModel.IsLoaded)
                return null;

            Vehicle vehicle =
                Vehicle.Create(
                    vehicleModel,
                    position,
                    heading);

            vehicleModel.MarkAsNoLongerNeeded();

            if (vehicle == null ||
                !vehicle.Exists())
                return null;

            Model driverModel =
                new Model(DriverModelName);

            if (!driverModel.IsInCdImage ||
                !driverModel.IsValid)
            {
                vehicle.Delete();
                return null;
            }

            driverModel.Request(1000);

            if (!driverModel.IsLoaded)
            {
                vehicle.Delete();
                return null;
            }

            Ped driver =
                vehicle.CreatePedOnSeat(
                    VehicleSeat.Driver,
                    driverModel);

            driverModel.MarkAsNoLongerNeeded();

            if (driver == null ||
                !driver.Exists())
            {
                vehicle.Delete();
                return null;
            }

            Racer racer =
                new Racer();

            racer.Id = id;
            racer.Name = name;

            racer.AssignEntities(
                driver,
                vehicle);

            SetVehicleRaceState(vehicle);

            driver.BlockPermanentEvents = true;

            return racer;
        }

        public Racer[] SpawnRaceRacers(
            RaceDefinition race,
            int npcCount)
        {
            if (race == null ||
                npcCount < 0)
            {
                return new Racer[0];
            }

            int total =
                npcCount + 1;

            Racer[] racers =
                new Racer[total];

            if (race.Checkpoints == null ||
                race.Checkpoints.Count == 0)
            {
                return racers;
            }

            Vector3 startPosition =
                race.Checkpoints[0].Position;

            float heading =
                GetHeadingFromDirection(
                    race.Checkpoints[0].Direction);

            racers[0] =
                SpawnPlayerRacer(
                    startPosition,
                    heading);

            for (int i = 0;
                 i < npcCount;
                 i++)
            {
                Vector3 spawnPosition =
                    GetGridPosition(
                        startPosition,
                        heading,
                        i + 1);

                racers[i + 1] =
                    SpawnNpcRacer(
                        "npc_" + (i + 1),
                        GetNpcName(i),
                        spawnPosition,
                        heading);
            }

            return racers;
        }

        private Vector3 GetGridPosition(
            Vector3 start,
            float heading,
            int slot)
        {
            float radians =
                heading *
                (float)Math.PI /
                180.0f;

            Vector3 forward =
                new Vector3(
                    (float)Math.Sin(radians),
                    (float)Math.Cos(radians),
                    0.0f);

            Vector3 right =
                new Vector3(
                    forward.Y,
                    -forward.X,
                    0.0f);

            int row =
                (slot - 1) / 2;

            int side =
                (slot - 1) % 2 == 0
                    ? -1
                    : 1;

            return start
                   - forward * (7.0f + row * 6.0f)
                   + right * (side * 3.0f);
        }

        private float GetHeadingFromDirection(
            Vector3 direction)
        {
            if (direction.Length() <= 0.001f)
                return 0.0f;

            return Function.Call<float>(
                Hash.GET_HEADING_FROM_VECTOR_2D,
                direction.X,
                direction.Y);
        }

        private string GetNpcName(int index)
        {
            string[] names =
            {
                "Ghost",
                "Viper",
                "Razor",
                "Shadow",
                "Nitro",
                "Torque",
                "Blaze",
                "Drift",
                "Phantom",
                "Ace"
            };

            return names[
                index % names.Length];
        }

        private void SetVehicleRaceState(
            Vehicle vehicle)
        {
            if (vehicle == null ||
                !vehicle.Exists())
                return;

            vehicle.IsPersistent = true;

            Function.Call(
                Hash.SET_VEHICLE_ENGINE_ON,
                vehicle.Handle,
                true,
                true,
                false);

            Function.Call(
                Hash.SET_VEHICLE_MOD_KIT,
                vehicle.Handle,
                0);

            Function.Call(
                Hash.SET_VEHICLE_FIXED,
                vehicle.Handle);

            Function.Call(
                Hash.SET_VEHICLE_DEFORMATION_FIXED,
                vehicle.Handle);
        }

        public void DeleteRacer(
            Racer racer)
        {
            if (racer == null)
                return;

            racer.Dispose();
        }
    }
}