using Akka.Actor;
using Akka.Configuration;
using Shared;

namespace System2
{
    class Program
    {
        class SayHello { }

        class HelloActor : ReceiveActor
        {
            private readonly IActorRef _remoteActor;
            private int _helloCounter;
            private ICancelable _helloTask;

            public HelloActor(IActorRef remoteActor)
            {
                _remoteActor = remoteActor;
                Receive<Hello>(hello =>
                {
                    Console.WriteLine("Received {1} from {0}", Sender, hello.Message);
                });

                Receive<SayHello>(sayHello =>
                {
                    _remoteActor.Tell(new Hello("hello" + _helloCounter++));
                });
            }

            protected override void PreStart()
            {
                _helloTask = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1), Context.Self, new SayHello(), ActorRefs.NoSender);
            }

            protected override void PostStop()
            {
                _helloTask.Cancel();
            }
        }

        static void Main()
        {
            using (var system = ActorSystem.Create("Deployer", ConfigurationFactory.ParseString(@"
                akka {  
                    actor{
                        provider = remote
                        deployment {
                                    /remoteecho {
                                        remote = ""akka.tcp://DeployTarget@10.5.0.5:8090""
                                    }
                                }
                    }
                    serializers {
                        hyperion = ""Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion""
                    }
                    serialization-bindings {
                        ""System.Object"" = hyperion
                    }
                    remote {
                        dot-netty.tcp {
                            port = 0
                            hostname = 10.5.0.6
                        }
                    }
                }")))
            {
                //deploy remotely via config
                var remoteEcho1 = system.ActorOf(Props.Create(() => new EchoActor()), "remoteecho");

                //deploy remotely via code
                var remoteAddress = Address.Parse("akka.tcp://DeployTarget@10.5.0.5:8090");
                var remoteEcho2 =
                    system.ActorOf(
                        Props.Create(() => new EchoActor())
                            .WithDeploy(Deploy.None.WithScope(new RemoteScope(remoteAddress))), "coderemoteecho");


                system.ActorOf(Props.Create(() => new HelloActor(remoteEcho1)));
                system.ActorOf(Props.Create(() => new HelloActor(remoteEcho2)));

                Console.ReadLine();
            }

                

            //string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            //Console.WriteLine(hostName);
            //// Get the IP
            //string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            //Console.WriteLine("My IP Address is: " + myIP);
            //// Console.ReadKey();

            //bool pingable = false;
            //Ping? pinger = null;

            //try
            //{
            //    pinger = new Ping();
            //    PingReply reply = pinger.Send("10.5.0.5");
            //    pingable = reply.Status == IPStatus.Success;
            //    Console.WriteLine($"pinging {pingable}");
            //}
            //catch (PingException)
            //{
            //    // Discard PingExceptions and return false;
            //    Console.WriteLine("PingExceptions");
            //}
            //finally
            //{
            //    pinger?.Dispose();
            //}
        }
    }
}
