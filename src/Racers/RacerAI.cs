using System;
using GTA;
using GTA.Math;
using GTA.Native;
using LosSantosUnderground.Races;

namespace LosSantosUnderground.Racers
{
    public class RacerAI
    {
        private readonly VehicleDrivingFlags _drivingFlags =
            VehicleDrivingFlags.SwerveAroundAllVehicles |
            VehicleDrivingFlags.SteerAroundStationaryVehicles |
            VehicleDrivingFlags.SteerAroundPeds |
            VehicleDrivingFlags.UseShortCutLinks |
            VehicleDrivingFlags.AllowGoingWrongWay;

        public void Update(
            Racer racer,
            RaceDefinition race)
        {
            if (racer == null ||
                race == null ||
                !racer.IsActive ||
                racer.IsFleeing ||
                !racer.HasValidEntities())
            {
                return;
            }

            if (race.Checkpoints == null ||
                race.Checkpoints.Count == 0)
            {
                return;
            }

            int checkpointIndex =
                racer.CurrentCheckpoint;

            if (checkpointIndex < 0 ||
                checkpointIndex >= race.Checkpoints.Count)
            {
                return;
            }

            RaceCheckpoint checkpoint =
                race.Checkpoints[checkpointIndex];

            if (checkpoint == null)
                return;

            DriveToCheckpoint(
                racer,
                checkpoint);
        }

        private void DriveToCheckpoint(
            Racer racer,
            RaceCheckpoint checkpoint)
        {
            Ped ped = racer.Ped;
            Vehicle vehicle = racer.Vehicle;

            if (ped == null ||
                vehicle == null ||
                !ped.Exists() ||
                !vehicle.Exists())
            {
                return;
            }

            if (!ped.IsInVehicle(vehicle))
                return;

            Vector3 target =
                checkpoint.Position;

            float speed =
                GetRaceSpeed(vehicle);

            if (racer.Profile != null)
            {
                speed += racer.Profile.Aggression * 8.0f;
                speed += racer.Profile.Skill * 4.0f;
            }
            speed = Math.Max(56.0f, Math.Min(70.0f, speed));

            Function.Call(Hash.SET_VEHICLE_MAX_SPEED, vehicle.Handle, speed);

            // DriveTo expects speed before the driving flags and stopping
            // range last. Passing these in the wrong order made the AI crawl
            // and stop far from the checkpoint.
            ped.Task.DriveTo(
                vehicle,
                target,
                speed,
                _drivingFlags,
                Math.Max(2.0f, Math.Min(checkpoint.Radius, 4.0f)));

            Function.Call(
                Hash.SET_DRIVE_TASK_DRIVING_STYLE,
                ped.Handle,
                786468);
        }

        private float GetRaceSpeed(
            Vehicle vehicle)
        {
            if (vehicle == null ||
                !vehicle.Exists())
            {
                return 30.0f;
            }

            float currentSpeed =
                vehicle.Speed;

            if (currentSpeed < 20.0f)
                return 38.0f;

            if (currentSpeed < 35.0f)
                return 42.0f;

            return 48.0f;
        }

        public void Stop(
            Racer racer)
        {
            if (racer == null ||
                racer.Ped == null ||
                !racer.Ped.Exists())
            {
                return;
            }

            racer.Ped.Task.ClearAll();
        }

        public void ResetVehicle(
            Racer racer)
        {
            if (racer == null ||
                racer.Vehicle == null ||
                !racer.Vehicle.Exists())
            {
                return;
            }

            Function.Call(
                Hash.SET_VEHICLE_ENGINE_ON,
                racer.Vehicle.Handle,
                true,
                true,
                false);
        }
    }
}