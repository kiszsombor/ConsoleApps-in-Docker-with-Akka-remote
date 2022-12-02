using Akka.Actor;
using Akka.Configuration;

namespace System1
{
    class Program
    {
        static void Main()
        {
            // Console.WriteLine("Enter your name: ");
            // var name = Console.ReadLine();
            // Console.WriteLine($"Hello {name}");

            using (var system = ActorSystem.Create("DeployTarget", ConfigurationFactory.ParseString(@"
                    akka {  
                        actor.provider = remote
                        remote {
                            dot-netty.tcp {
                                port = 8090
                                hostname = 0.0.0.0
                                public-hostname = 10.5.0.5
                            }
                        }
                    }")))
            {
                // Console.ReadKey();
                Console.ReadLine();

                //string hostName = Dns.GetHostName(); // Retrive the Name of HOST
                //Console.WriteLine(hostName);
                //// Get the IP
                //string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
                //Console.WriteLine("My IP Address is: " + myIP);

                //// Console.ReadLine();

                //bool pingable = false;
                //Ping? pinger = null;

                //try
                //{
                //    pinger = new Ping();
                //    PingReply reply = pinger.Send("10.5.0.6");
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
}
