using Akka.Actor;
using Akka.Configuration;

namespace System1
{
    class Program
    {
        static void Main()
        {
            var config = ConfigurationFactory.ParseString(@"
                    akka {  
                        actor.provider = remote
                        remote {
                            dot-netty.tcp {
                                port = 8090
                                hostname = 0.0.0.0
                                public-hostname = 10.5.0.5
                            }
                        }
                    }");

            using var system = ActorSystem.Create("DeployTarget", config);
            Console.ReadLine();
        }
    }
}
