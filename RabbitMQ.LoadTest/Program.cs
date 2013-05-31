using System;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using EasyNetQ;
using RabbitMQ.LoadTest.Messages;

namespace RabbitMQ.LoadTest
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

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            int threads = args.Length > 4 ? Convert.ToInt16(args[4]) : Convert.ToInt16(ConfigurationManager.AppSettings["Threads"]);

            Task[] tasks = new Task[threads];

            for (int i = 0; i < tasks.Length; i++)
            {
                string threadno = i.ToString();
                tasks[i] = Task.Factory.StartNew(() => Publish(host, port, vhost, username, password, threadno,token), token);
            }

            Console.WriteLine("Publisher Started, Hit enter to cancel");
            Console.ReadLine();

            tokenSource.Cancel();
        }

        protected static void Publish(string host, ushort port, string vhost, string username, string password, string ThreadNo, CancellationToken token)
        {
            using (var bus = RabbitHutch.CreateBus(host, port, vhost, username, password, 3, serviceRegister => serviceRegister.Register<IEasyNetQLogger>(serviceProvider => new NullLogger())))
            {
                int counter = 0;
                using (var channel = bus.OpenPublishChannel())
                {
                    Random rnd = new Random();
                    string folderPath = ConfigurationManager.AppSettings["MessageFilePath"];

                    while (1 == 1 && !token.IsCancellationRequested) //Infinte Loop. Keep publishing until program is stopped.
                    {
                        foreach (string file in Directory.EnumerateFiles(folderPath))
                        {
                            //Only Publish 80% of the Messages on each pass to Randomise load on server
                            if (rnd.Next(100) <= 80 && !token.IsCancellationRequested)
                            {
                                string contents = File.ReadAllText(file);
                                channel.Publish(new XMLMessage { XMLString = contents });
                                counter++;
                                if (counter % 100 == 0)
                                    Console.WriteLine(ThreadNo.ToString() + " - " + counter.ToString());
                            }

                        }
                    }

                    Console.WriteLine("Thread {0} stopped", ThreadNo);
                }
            }
        }
    }
}
