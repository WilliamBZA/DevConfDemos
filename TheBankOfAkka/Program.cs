using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheBankOfAkka
{
    class Program
    {
        static void Main(string[] args)
        {
            var actorSystem = ActorSystem.Create("QuietOnTheSet");
            var dollarParton = actorSystem.ActorOf<DollarParton>();

            var johnnyCash = actorSystem.ActorOf<WithdrawActor>("JohnnyCash");
            var larsUltraRich = actorSystem.ActorOf<WithdrawActor>("LarsUltraRich");

            johnnyCash.Tell(new RequestCash { Amount = 50 });
            larsUltraRich.Tell(new RequestCash { Amount = 70 });
        }
    }

    public class DollarParton : ReceiveActor
    {
        public decimal CurrentBalance;

        protected override void PreStart()
        {
            CurrentBalance = 70;
            Become(() =>
            {
                Receive<DrawCash>(msg => DrawCash(msg));
            });

            base.PreStart();
        }

        private bool CanDraw(decimal amount)
        {
            return CurrentBalance - amount >= 0;
        }

        private void DrawCash(DrawCash request)
        {
            if (CanDraw(request.Amount))
            {
                CurrentBalance -= request.Amount;

                Sender.Tell(true);
            }
            else
            {
                Sender.Tell(false);
            }
        }
    }

    public class WithdrawActor : ReceiveActor
    {
        private IActorRef dollarParton;

        protected override void PreStart()
        {
            dollarParton = Context.ActorOf<DollarParton>();

            Become(() =>
            {
                Receive<RequestCash>(msg => DrawMoney(msg.Amount));
            });

            base.PreStart();
        }

        protected async Task DrawMoney(decimal amount)
        {
            var wasSuccessful = await dollarParton.Ask<bool>(new DrawCash { Amount = amount });
        }
    }

    public class DrawCash
    {
        public decimal Amount { get; set; }
    }

    public class RequestCash
    {
        public decimal Amount { get; set; }
    }
}