using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NaiveImplementation
{
    class Program
    {
        static void Main(string[] args)
        {
            for (var x = 0; x < int.MaxValue; x++)
            {
                var tom = new TomBanks(startBalance: 70);

                var cashton = new BadActor(tom, withdraw: 50);
                var bill = new BadActor(tom, withdraw: 70);

                Parallel.ForEach(new Action[] { () => cashton.DrawMoney(), () => bill.DrawMoney() }, t => t());
            }
        }
    }

    public class TomBanks
    {
        public decimal _currentBalance;

        public TomBanks(decimal startBalance = 0)
        {
            _currentBalance = startBalance;
        }

        public bool DrawCash(decimal amount)
        {
            if (CanDraw(amount))
            {
                _currentBalance -= amount;

                if (_currentBalance < 0)
                {
                    throw new InvalidOperationException("lol");
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CanDraw(decimal amount)
        {
            return _currentBalance - amount >= 0;
        }
    }

    public class BadActor
    {
        private TomBanks _tom;
        private decimal _amountToDraw;

        public BadActor(TomBanks tom, decimal withdraw = 0)
        {
            _tom = tom;
            _amountToDraw = withdraw;
        }

        public void DrawMoney()
        {
            _tom.DrawCash(_amountToDraw);
        }
    }
}