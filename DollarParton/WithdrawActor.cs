using Akka.Actor;
using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actors
{
    public class WithdrawActor : ReceiveActor
    {
        private IActorRef _bankActor;

        protected override async void PreStart()
        {
            _bankActor = await Context.System.ActorSelection("akka.tcp://QuietOnTheSetScaleoutClient@localhost:8091/user/DollarParton").ResolveOne(TimeSpan.FromSeconds(15));

            Become(() =>
            {
                Receive<RequestCash>(msg => AskForMoney(msg.Amount));
            });

            base.PreStart();
        }

        protected async Task AskForMoney(decimal amount)
        {
            var wasSuccessful = await _bankActor.Ask<bool>(new DrawCash { Amount = amount });
        }
    }
}
