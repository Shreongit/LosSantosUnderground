using System;

namespace LosSantosUnderground.Racers
{
    public class RacerProfile
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string VehicleModel { get; set; }

        public float Skill { get; set; } = 0.75f;

        public float Aggression { get; set; } = 0.5f;

        public float RiskTaking { get; set; } = 0.5f;

        public float MistakeChance { get; set; } = 0.05f;

        public float OvertakeAbility { get; set; } = 0.5f;

        public float BrakingSkill { get; set; } = 0.75f;

        public float PoliceAwareness { get; set; } = 0.5f;

        public RacerProfile()
        {
        }

        public RacerProfile(
            string id,
            string name,
            string vehicleModel,
            float skill,
            float aggression,
            float riskTaking,
            float mistakeChance,
            float overtakeAbility,
            float brakingSkill,
            float policeAwareness)
        {
            Id = id;
            Name = name;
            VehicleModel = vehicleModel;
            Skill = Clamp(skill);
            Aggression = Clamp(aggression);
            RiskTaking = Clamp(riskTaking);
            MistakeChance = Clamp(mistakeChance);
            OvertakeAbility = Clamp(overtakeAbility);
            BrakingSkill = Clamp(brakingSkill);
            PoliceAwareness = Clamp(policeAwareness);
        }

        public float GetPerformanceMultiplier()
        {
            float performance =
                (Skill * 0.45f) +
                (BrakingSkill * 0.20f) +
                (OvertakeAbility * 0.15f) +
                (RiskTaking * 0.10f) +
                (Aggression * 0.10f);

            return 0.90f + (Clamp(performance) * 0.20f);
        }

        public bool ShouldMakeMistake(Random random)
        {
            if (random == null)
                return false;

            return random.NextDouble() <
                   MistakeChance;
        }

        public bool ShouldAttemptOvertake(Random random)
        {
            if (random == null)
                return false;

            return random.NextDouble() <
                   OvertakeAbility;
        }

        public bool ShouldTakeRisk(Random random)
        {
            if (random == null)
                return false;

            return random.NextDouble() <
                   RiskTaking;
        }

        public bool HasStrongPoliceAwareness()
        {
            return PoliceAwareness >= 0.75f;
        }

        private static float Clamp(float value)
        {
            return Math.Max(
                0.0f,
                Math.Min(1.0f, value));
        }
    }
}