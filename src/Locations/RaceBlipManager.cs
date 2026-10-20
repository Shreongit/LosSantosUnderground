using System.Collections.Generic;
using GTA;
using GTA.UI;

namespace LosSantosUnderground.Locations
{
    public class RaceBlipManager
    {
        private readonly List<Blip> _raceBlips =
            new List<Blip>();

        public void CreateBlips(
            IEnumerable<RaceLocation> locations)
        {
            ClearBlips();

            if (locations == null)
                return;

            foreach (RaceLocation location in locations)
            {
                if (location == null)
                    continue;

                Blip blip =
                    Blip.Create(location.Position);

                if (blip == null)
                    continue;

                blip.Sprite = BlipSprite.Race;
                blip.Color = BlipColor.Red;
                blip.Name = location.Name;
                blip.IsShortRange = true;

                _raceBlips.Add(blip);
            }
        }

        public void CreateBlip(
            RaceLocation location)
        {
            if (location == null)
                return;

            Blip blip =
                Blip.Create(location.Position);

            if (blip == null)
                return;

            blip.Sprite = BlipSprite.Race;
            blip.Color = BlipColor.Red;
            blip.Name = location.Name;
            blip.IsShortRange = true;

            _raceBlips.Add(blip);
        }

        public void ClearBlips()
        {
            foreach (Blip blip in _raceBlips)
            {
                if (blip != null && blip.Exists())
                    blip.Delete();
            }

            _raceBlips.Clear();
        }

        public void UpdateVisibility(bool visible)
        {
            foreach (Blip blip in _raceBlips)
            {
                if (blip == null || !blip.Exists())
                    continue;

                blip.IsFriendly = visible;
            }
        }

        public void Dispose()
        {
            ClearBlips();
        }
    }
}