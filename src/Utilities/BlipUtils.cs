using GTA;
using GTA.Native;

namespace LosSantosUnderground.Utilities
{
    public static class BlipUtils
    {
        public static Blip CreateRaceBlip(
            float x,
            float y,
            float z,
            string name)
        {
            Blip blip =
                Blip.Create(
                    new GTA.Math.Vector3(
                        x,
                        y,
                        z));

            if (blip == null)
                return null;

            blip.Sprite = BlipSprite.Race;
            blip.Color = BlipColor.Red;
            blip.Name = string.IsNullOrWhiteSpace(name)
                ? "Street Race"
                : name;
            blip.IsShortRange = true;

            return blip;
        }

        public static Blip CreateBlip(
            GTA.Math.Vector3 position,
            BlipSprite sprite,
            BlipColor color,
            string name,
            bool shortRange)
        {
            Blip blip =
                Blip.Create(position);

            if (blip == null)
                return null;

            blip.Sprite = sprite;
            blip.Color = color;
            blip.Name = name ?? string.Empty;
            blip.IsShortRange = shortRange;

            return blip;
        }

        public static void SetVisible(
            Blip blip,
            bool visible)
        {
            if (blip == null ||
                !blip.Exists())
                return;

            blip.IsFriendly = visible;
        }

        public static void SetRoute(
            Blip blip,
            bool enabled)
        {
            if (blip == null ||
                !blip.Exists())
                return;

            Function.Call(
                Hash.SET_BLIP_ROUTE,
                blip.Handle,
                enabled);
        }

        public static void Delete(
            Blip blip)
        {
            if (blip == null ||
                !blip.Exists())
                return;

            blip.Delete();
        }
    }
}