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

                Task.WaitAll(new[] {
                    Task.Run(() => cashton.DrawMoney()),
                    Task.Run(() => bill.DrawMoney())
                });

                if (tom.CurrentBalance < 0)
                {
                    throw new ApplicationException("LOL");
                }
            }
        }
    }

    public class TomBanks
    {
        public decimal CurrentBalance;
        public object _lock = new object();

        public TomBanks(decimal startBalance)
        {
            CurrentBalance = startBalance;
        }

        private bool CanDraw(decimal amount)
        {
            return CurrentBalance - amount >= 0;
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

                        CurrentBalance -= amount;

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