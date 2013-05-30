using System;
using System.Configuration;
using EasyNetQ;
using RabbitMQ.LoadTest.Messages;

namespace RabbitMQ.LoadTest.Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            string defaulthostport = ConfigurationManager.AppSettings["Host"] + ":" + ConfigurationManager.AppSettings["Port"];
            string[] hostport = args.Length > 0 ? args[0].Split(':') : defaulthostport.Split(':');

            string host = hostport[0];
            ushort port = hostport.Length > 1 ? Convert.ToUInt16(hostport[1]) : Convert.ToUInt16(ConfigurationManager.AppSettings["Port"]);

            string vhost = args.Length > 1 ? args[1] : ConfigurationManager.AppSettings["VHost"];
            string username = args.Length > 2 ? args[2] : ConfigurationManager.AppSettings["RabbitMQUser"];
            string password = args.Length > 3 ? args[3] : ConfigurationManager.AppSettings["RabbitMQPassword"];

            int counter = 0;

            using (var bus = RabbitHutch.CreateBus(host, port, vhost, username, password, 3, serviceRegister => serviceRegister.Register<IEasyNetQLogger>(serviceProvider => new NullLogger())))
            {
                bus.Subscribe<XMLMessage>("XML_subscriber", message => Console.WriteLine(counter++)); 
                //if (counter % 10 = 0)
                
                Console.WriteLine("Subscription Started. Hit enter to quit");
                Console.ReadLine();
            }
        }
    }
}
