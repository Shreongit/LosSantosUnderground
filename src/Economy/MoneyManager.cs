using GTA;

namespace LosSantosUnderground.Economy
{
    /// <summary>
    /// Uses the single-player wallet exposed by ScriptHookVDotNet.
    /// The old MP0_WALLET_BALANCE stat was unreliable in story mode.
    /// </summary>
    public class MoneyManager
    {
        public int GetMoney()
        {
            return Game.Player.Money;
        }

        public bool CanAfford(int amount)
        {
            return amount >= 0 && GetMoney() >= amount;
        }

        public bool RemoveMoney(int amount)
        {
            if (amount < 0 || !CanAfford(amount))
                return false;

            Game.Player.Money -= amount;
            return true;
        }

        public bool AddMoney(int amount)
        {
            if (amount < 0)
                return false;

            Game.Player.Money += amount;
            return true;
        }

        public bool TryPayEntryFee(int entryFee)
        {
            return entryFee >= 0 && RemoveMoney(entryFee);
        }

        public void Refund(int amount)
        {
            if (amount > 0)
                AddMoney(amount);
        }
    }
}
