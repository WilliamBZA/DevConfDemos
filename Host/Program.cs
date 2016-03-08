using Actors;
using Akka.Actor;
using Akka.Configuration;
using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Host
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var system = ActorSystem.Create("DistributedHost", ConfigurationFactory.ParseString(@"
                akka {  
                    actor{
                        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                        deployment {
                            /remoteaccountmanager {
                                remote = ""akka.tcp://DistributedDeployTarget@localhost:8090""
                            }
                        }
                    }
                    remote {
                        helios.tcp {
		                    port = 0
		                    hostname = localhost
                        }
                    }
                }")))
            {
                var remoteAddress = Address.Parse("akka.tcp://DistributedDeployTarget@localhost:8090");
                var remoteBankAccountManager = system.ActorOf(Props.Create(() => new DollarParton()), "remoteaccountmanager");

                remoteBankAccountManager.Tell(new DrawCash() { Amount = 10 });

                Console.ReadLine();

                remoteBankAccountManager.Tell(new DrawCash() { Amount = 10 });

                Console.ReadKey();
            }
        }
    }
}