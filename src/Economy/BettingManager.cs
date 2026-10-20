using System;
using System.Collections.Generic;
using System.Linq;
using GTA;
using LosSantosUnderground.Racers;

namespace LosSantosUnderground.Economy
{
    public class BettingManager : IDisposable
    {
        private readonly MoneyManager _moneyManager;

        private int _entryFee;
        private int _prizePool;

        private bool _playerPaid;
        private bool _poolClosed;
        private bool _payoutCompleted;
        private bool _disposed;

        public int EntryFee
        {
            get { return _entryFee; }
        }

        public int PrizePool
        {
            get { return _prizePool; }
        }

        public bool PlayerPaid
        {
            get { return _playerPaid; }
        }

        public bool IsPoolClosed
        {
            get { return _poolClosed; }
        }

        public bool PayoutCompleted
        {
            get { return _payoutCompleted; }
        }

        public BettingManager()
        {
            _moneyManager = new MoneyManager();
        }

        public bool TryCollectPlayerEntryFee(
            int entryFee)
        {
            if (_disposed ||
                _playerPaid ||
                entryFee < 0)
            {
                return false;
            }

            if (!_moneyManager.CanAfford(entryFee))
                return false;

            if (!_moneyManager.RemoveMoney(entryFee))
                return false;

            _entryFee = entryFee;
            _playerPaid = true;

            return true;
        }

        public void RefundPlayerEntryFee()
        {
            if (_disposed ||
                !_playerPaid ||
                _payoutCompleted)
            {
                return;
            }

            if (_entryFee > 0)
            {
                _moneyManager.AddMoney(_entryFee);
            }

            _playerPaid = false;
            _entryFee = 0;
        }

        public void AddEntry(
            int amount)
        {
            if (_disposed ||
                _poolClosed ||
                amount < 0)
            {
                return;
            }

            _prizePool += amount;
        }

        public void PayWinner(
            Ped player,
            List<RacerResult> results,
            TimeSpan elapsed)
        {
            if (_disposed ||
                _poolClosed ||
                _payoutCompleted ||
                !_playerPaid)
            {
                return;
            }

            _poolClosed = true;

            int prize = _prizePool;

            if (prize <= 0)
            {
                _payoutCompleted = true;
                return;
            }

            if (player == null ||
                !player.Exists() ||
                player.IsDead)
            {
                _payoutCompleted = true;
                return;
            }

            _moneyManager.AddMoney(prize);

            _payoutCompleted = true;
        }

        public void CloseWithoutPayout()
        {
            if (_disposed ||
                _poolClosed)
            {
                return;
            }

            _poolClosed = true;
            _payoutCompleted = false;
        }

        public int GetPrizePool()
        {
            return _prizePool;
        }

        public bool HasPlayerWon(
            IEnumerable<RacerResult> results)
        {
            if (results == null)
                return false;

            RacerResult first =
                results.FirstOrDefault();

            return first != null &&
                   first.IsPlayer;
        }

        public void Reset()
        {
            _entryFee = 0;
            _prizePool = 0;
            _playerPaid = false;
            _poolClosed = false;
            _payoutCompleted = false;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
        }
    }
}