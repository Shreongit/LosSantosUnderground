using GTA;
using GTA.Native;

namespace LosSantosUnderground.Police
{
    public class WantedLevelManager
    {
        private const int RaceWantedLevel = 3;

        public void SetRaceWantedLevel()
        {
            Player player = Game.Player;

            if (player == null)
                return;

            Function.Call(
                Hash.SET_PLAYER_WANTED_LEVEL,
                player.Handle,
                RaceWantedLevel,
                false);

            Function.Call(
                Hash.SET_PLAYER_WANTED_LEVEL_NOW,
                player.Handle,
                false);
        }

        public int GetWantedLevel()
        {
            Player player = Game.Player;

            if (player == null)
                return 0;

            return Function.Call<int>(
                Hash.GET_PLAYER_WANTED_LEVEL,
                player.Handle);
        }

        public bool HasActiveWantedLevel()
        {
            return GetWantedLevel() > 0;
        }

        public bool HasRaceWantedLevel()
        {
            return GetWantedLevel() >= RaceWantedLevel;
        }

        public void ClearWantedLevel()
        {
            Player player = Game.Player;

            if (player == null)
                return;

            Function.Call(
                Hash.CLEAR_PLAYER_WANTED_LEVEL,
                player.Handle);
        }
    }
}