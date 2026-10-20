using System;
using System.Windows.Forms;
using GTA;

namespace LosSantosUnderground.Input
{
    public class InputManager
    {
        public event EventHandler RaceInteractPressed;
        public event EventHandler MenuPressed;
        public event EventHandler CancelPressed;

        public void HandleKeyDown(
            KeyEventArgs e)
        {
            if (e == null)
                return;

            switch (e.KeyCode)
            {
                case Keys.E:
                    RaceInteractPressed?.Invoke(
                        this,
                        EventArgs.Empty);
                    break;

                case Keys.F5:
                    MenuPressed?.Invoke(
                        this,
                        EventArgs.Empty);
                    break;

                case Keys.Escape:
                    CancelPressed?.Invoke(
                        this,
                        EventArgs.Empty);
                    break;
            }
        }

        public bool IsPlayerInVehicle()
        {
            Ped player =
                Game.Player.Character;

            return player != null &&
                   player.Exists() &&
                   player.IsInVehicle();
        }

        public Vehicle GetPlayerVehicle()
        {
            Ped player =
                Game.Player.Character;

            if (player == null ||
                !player.Exists() ||
                !player.IsInVehicle())
            {
                return null;
            }

            return player.CurrentVehicle;
        }
    }
}