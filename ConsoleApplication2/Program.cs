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

        public TomBanks(decimal startBalance = 0)
        {
            CurrentBalance = startBalance;
        }

        public bool DrawCash(decimal amount)
        {
            if (CanDraw(amount))
            {
                CurrentBalance -= amount;

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CanDraw(decimal amount)
        {
            return CurrentBalance - amount >= 0;
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