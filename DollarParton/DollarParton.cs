using Akka.Actor;
using Akka.Configuration;
using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actors
{
    public class DollarParton : ReceiveActor
    {
        private decimal _currentBalance;

        protected override void PreStart()
        {
            _currentBalance = 70;
            Become(() =>
            {
                Receive<DrawCash>(msg => DrawCash(msg));
            });

            base.PreStart();
        }

        private bool CanDraw(decimal amount)
        {
            return _currentBalance - amount >= 0;
        }

        private void DrawCash(DrawCash request)
        {
            Console.WriteLine("Trying to withdraw ${0}", request.Amount);
            if (CanDraw(request.Amount))
            {
                _currentBalance -= request.Amount;

                if (_currentBalance < 0)
                {
                    throw new ApplicationException("lol");
                }

                Sender.Tell(true);
            }
            else
            {
                Sender.Tell(false);
            }
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(e => Directive.Resume);
        }
    }
}
