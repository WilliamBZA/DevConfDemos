using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockingImplementation
{
    class Program
    {
        static void Main(string[] args)
        {
            for (var x = 0; x < int.MaxValue; x++)
            {
                var tom = new TomBanks(70);
                var cashton = new BadActor(tom, 50);
                var bill = new BadActor(tom, 70);

                Parallel.ForEach(new Action[] { () => cashton.DrawMoney(), () => bill.DrawMoney() }, t => t());
            }
        }
    }

    public class TomBanks
    {
        public decimal _currentBalance;
        public object _lock = new object();

        public TomBanks(decimal startBalance)
        {
            _currentBalance = startBalance;
        }

        private bool CanDraw(decimal amount)
        {
            return _currentBalance - amount >= 0;
        }

        public bool DrawCash(decimal amount)
        {
            if (CanDraw(amount))
            {
                lock(_lock)
                {
                    if (CanDraw(amount))
                    {
                        System.Threading.Thread.MemoryBarrier();

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
            }
            else
            {
                return false;
            }
        }
    }

    public class BadActor
    {
        private TomBanks _tom;
        private decimal _amountToDraw;

        public BadActor(TomBanks tom, decimal amountToDraw)
        {
            _tom = tom;
            _amountToDraw = amountToDraw;
        }

        public void DrawMoney()
        {
            _tom.DrawCash(_amountToDraw);
        }
    }
}