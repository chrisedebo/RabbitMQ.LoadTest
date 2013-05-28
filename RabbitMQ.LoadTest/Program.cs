using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;

namespace RabbitMQ.LoadTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = args.Length > 0 ? args[0] : "localhost";
            string vhost = args.Length > 1 ? args[1] : "/";

            using (var bus = RabbitHutch.CreateBus(host, 5672 , vhost, "guest", "guest",3, serviceRegister => serviceRegister.Register(serviceProvider => new NullLogger())))
            {
                Task.Factory.StartNew(() => Publish(bus));

                Console.WriteLine("Publisher Started, Hit any key to cancel");
                Console.ReadKey();
            }

        }

        protected static void Publish(IBus bus)
        {
            using (var channel = bus.OpenPublishChannel())
            {
                //TODO: Load Files from DIR and publish
            }
        }
    }
}
