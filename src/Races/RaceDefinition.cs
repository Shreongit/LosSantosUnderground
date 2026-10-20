using System.Collections.Generic;

namespace LosSantosUnderground.Races
{
    public class RaceDefinition
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public RaceType Type { get; set; }

        public int EntryFee { get; set; }

        public int MaxRacers { get; set; }

        public int MinHour { get; set; } = 17;

        public int MaxHour { get; set; } = 5;

        public List<RaceCheckpoint> Checkpoints { get; set; }
            = new List<RaceCheckpoint>();

        public RaceDefinition()
        {
        }

        public RaceDefinition(
            string id,
            string name,
            RaceType type,
            int entryFee,
            int maxRacers)
        {
            Id = id;
            Name = name;
            Type = type;
            EntryFee = entryFee;
            MaxRacers = maxRacers;
        }
    }
}