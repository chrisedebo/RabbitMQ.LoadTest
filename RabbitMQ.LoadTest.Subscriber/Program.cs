using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using RabbitMQ.LoadTest.Messages;
using CommandLine;

namespace RabbitMQ.LoadTest.Subscriber
{
    class Program
    {
        private static SubscriberOptions options = new SubscriberOptions();
        private static MessageLogger logger;

        static void Main(string[] args)
        {

            //Parse command line options and quit if invalid.
            if (Parser.Default.ParseArguments(args, options))
            {

                using(logger = new MessageLogger())
                {
                    if (options.useAsync)
                        RunAysnc();
                    else
                        RunThreaded();
                }
            }
        }

        private static void RunThreaded()
        {

            //Set up cancellation token
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            //Start Threads
            Task[] tasks = new Task[options.ActualThreads];
            for (int i = 0; i < tasks.Length; i++)
            {
                int threadno = i;
                logger.AddType(threadno.ToString());
                tasks[i] = Task.Factory.StartNew(() => SubscribeThreaded(threadno, token), token);
            }

            Console.WriteLine("Subscription Started. Hit enter to quit");
            Console.ReadLine();
            tokenSource.Cancel();
            tasks.ToList().ForEach(x=>x.Wait());
        }

        protected static void SubscribeThreaded(int threadno, CancellationToken token)
        {
            int queues = options.ActualQueues;

            using (var bus = CreateBus())
            {
                int counter = 0;
                
                Console.WriteLine("Subscribing for thread:" + threadno + " queue:" + queues + " with id:" + "XML_subscriber" + (Convert.ToInt32(threadno) % queues).ToString());
                //Select message type, if anyone has a better way of doing this I'd be interested to hear from you :)
                switch (Convert.ToInt32(threadno) % queues)
                {
                    case 0:
                        bus.Subscribe<XMLMessage0>("LoadTestSubscriber", message => logger.Log(threadno.ToString()));
                        break;
                    case 1:
                        bus.Subscribe<XMLMessage1>("LoadTestSubscriber", message => logger.Log(threadno.ToString()));
                        break;
                    case 2:
                        bus.Subscribe<XMLMessage2>("LoadTestSubscriber", message => logger.Log(threadno.ToString()));
                        break;
                    case 3:
                        bus.Subscribe<XMLMessage3>("LoadTestSubscriber", message => logger.Log(threadno.ToString()));
                        break;
                    case 4:
                        bus.Subscribe<XMLMessage4>("LoadTestSubscriber", message => logger.Log(threadno.ToString()));
                        break;
                    case 5:
                        bus.Subscribe<XMLMessage5>("LoadTestSubscriber", message => logger.Log(threadno.ToString()));
                        break;
                    case 6:
                        bus.Subscribe<XMLMessage6>("LoadTestSubscriber", message => logger.Log(threadno.ToString()));
                        break;
                    case 7:
                        bus.Subscribe<XMLMessage7>("LoadTestSubscriber", message => logger.Log(threadno.ToString()));
                        break;
                    case 8:
                        bus.Subscribe<XMLMessage8>("LoadTestSubscriber", message => logger.Log(threadno.ToString()));
                        break;
                    case 9:
                        bus.Subscribe<XMLMessage9>("LoadTestSubscriber", message => logger.Log(threadno.ToString()));
                        break;
                
                }

                while (!token.IsCancellationRequested)
                {
                   //Wait until cancel signal is sent.
                    Thread.Sleep(1000);
                }
                Console.WriteLine("Thread {0} stopped", threadno);
            }
        }

        private static IBus CreateBus()
        {
            return RabbitHutch.CreateBus(options.ActualHost, options.ActualPort, options.ActualVhost, options.ActualUsername, options.ActualPassword, 3, serviceRegister => serviceRegister.Register<IEasyNetQLogger>(serviceProvider => new NullLogger()));
        }

        private static void RunAysnc()
        {
            Console.WriteLine("Using EasyNetQ Async subscribers");

            using (var bus = CreateBus())
            {
                var task = Task.Factory.StartNew(() => SubscribeToAllWithAsync(bus)); // running this as a task lets us abort out quickly if there's lots on the bus already
                Console.WriteLine("Press Enter to exit");
                Console.ReadLine();
                try{task.Dispose();}catch{}// this is dirty, but nevermind, we only use it if we bail quickly
            }
        }

        private static void SubscribeToAllWithAsync(IBus bus)
        {
            var utils = new Utils();
            MethodInfo openSubscribeMethod = bus.GetType().GetMethods().First(x => x.Name == "SubscribeAsync" && x.GetParameters().Length == 2);
            foreach (var message in utils.All.Take(options.ActualQueues))
            {
                // these bits could be switched in to have a bus per message type - you'd need to collect 'em all, pass em back, and dispose em after your readline though.
                //var bus = CreateBus();
                //MethodInfo openSubscribeMethod = bus.GetType().GetMethods().First(x => x.Name == "SubscribeAsync" && x.GetParameters().Length == 2);
                logger.AddType(message.Name);

                // adding a commandline flag to optionally add somat to the subscriber id below would allow tests with multiple subscribers 
                // getting the same message instead of all round-robining em.
                var args = new System.Collections.ArrayList { "LoadTestSubscriber", (Func<object, Task>)(arg => Task.Factory.StartNew(() => logger.Log(message.Name))) };
                MethodInfo closedSubscribeMethod = openSubscribeMethod.MakeGenericMethod(message);
                closedSubscribeMethod.Invoke(bus, args.ToArray());
                Console.WriteLine("subscribed to " + message.Name);
            }
        }
    }
}
