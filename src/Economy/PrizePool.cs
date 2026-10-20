using System;

namespace LosSantosUnderground.Economy
{
    public class PrizePool
    {
        private int _totalAmount;
        private bool _closed;

        public int TotalAmount => _totalAmount;

        public bool IsClosed => _closed;

        public bool HasPrize =>
            _totalAmount > 0;

        public void AddEntry(int amount)
        {
            if (_closed || amount <= 0)
                return;

            _totalAmount += amount;
        }

        public int Close()
        {
            _closed = true;
            return _totalAmount;
        }

        public int Collect()
        {
            if (_closed)
                return 0;

            _closed = true;

            int amount =
                _totalAmount;

            _totalAmount = 0;

            return amount;
        }

        public void Clear()
        {
            _totalAmount = 0;
            _closed = false;
        }

        public void Reset()
        {
            Clear();
        }
    }
}