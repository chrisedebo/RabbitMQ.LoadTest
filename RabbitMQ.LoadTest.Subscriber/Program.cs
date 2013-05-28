using System;
using EasyNetQ;
using RabbitMQ.LoadTest.Messages;

namespace RabbitMQ.LoadTest.Subscriber

{
    class Program
    {
        static void Main(string[] args)
        {
            //var topic = args.Length > 0 ? args[0] : "#";
            var topic = "#";

            string host = args.Length > 0 ? args[0] : "localhost";
            string vhost = args.Length > 1 ? args[1] : "/";

                using (var bus = RabbitHutch.CreateBus(host, 5672 , vhost, "guest", "guest",3, serviceRegister => serviceRegister.Register(serviceProvider => new NullLogger())))
                {
                    bus.Subscribe<XMLMessage>("word_subscriber", topic, message => Console.WriteLine(message.Word));

                    Console.WriteLine("Subscription Started. Hit any key quit");
                    Console.ReadKey();
                }
            }
        }
    }
}