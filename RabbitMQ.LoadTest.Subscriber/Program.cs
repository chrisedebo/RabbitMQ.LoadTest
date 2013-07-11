using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using RabbitMQ.LoadTest.Messages;
using CommandLine;
using CommandLine.Text;

namespace RabbitMQ.LoadTest.Subscriber
{
    class Program
    {
        protected static Options options = new Options();
        protected static string hp = ConfigurationManager.AppSettings["Host"] + ":" + ConfigurationManager.AppSettings["Port"];
        protected static string vh = ConfigurationManager.AppSettings["VHost"];
        protected static string ru = ConfigurationManager.AppSettings["RabbitMQUser"];
        protected static string rp = ConfigurationManager.AppSettings["RabbitMQPassword"];
        protected static string fp = ConfigurationManager.AppSettings["MessageFilePath"];
        protected static int t = Convert.ToInt32(ConfigurationManager.AppSettings["Threads"]);
        protected static int q = Convert.ToInt32(ConfigurationManager.AppSettings["Queues"]);
        protected static int d = Convert.ToInt32(ConfigurationManager.AppSettings["PublisherMessageDelay"]);

        static void Main(string[] args)
        {

            //Parse command line options and quit if invalid.
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                //Assign command line options or defaults to local variables
                string[] hostport = options.hostport != null ? options.hostport.Split(':') : hp.ToString().Split(':');

                string host = hostport[0];
                ushort port = hostport.Length > 1 ? Convert.ToUInt16(hostport[1]) : Convert.ToUInt16(ConfigurationManager.AppSettings["Port"]);

                string vhost = options.vhost != null ? options.vhost : vh;
                string username = options.username != null ? options.username : ru;
                string password = options.password != null ? options.password : rp;

                int threads = options.threads != 0 ? options.threads : t;

                //Set up cancellation token
                var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;

                //Start Threads
                Task[] tasks = new Task[threads];
                for (int i = 0; i < tasks.Length; i++)
                {
                    string threadno = i.ToString();
                    tasks[i] = Task.Factory.StartNew(() => Subscribe(host, port, vhost, username, password, threadno, token), token);
                }

                Console.WriteLine("Subscription Started. Hit enter to quit");
                Console.ReadLine();
                tokenSource.Cancel();
            }
        }

        protected static void Subscribe(string host, ushort port, string vhost, string username, string password, string threadno, CancellationToken token)
        {
            int queues = options.queues != 0 ? options.queues : q;

            using (var bus = RabbitHutch.CreateBus(host, port, vhost, username, password, 3, serviceRegister => serviceRegister.Register<IEasyNetQLogger>(serviceProvider => new NullLogger())))
            {
                int counter = 0;
                
                //Select message type, if anyone has a better way of doing this I'd be interested to hear from you :)
                switch (Convert.ToInt32(threadno) % queues)
                {
                    case 0:
                        bus.Subscribe<XMLMessage0>("XML_subscriber" + (Convert.ToInt32(threadno) % queues).ToString(), message => outputtoconsole(counter++, threadno));
                        break;
                    case 1:
                        bus.Subscribe<XMLMessage1>("XML_subscriber" + (Convert.ToInt32(threadno) % queues).ToString(), message => outputtoconsole(counter++, threadno));
                        break;
                    case 2:
                        bus.Subscribe<XMLMessage2>("XML_subscriber" + (Convert.ToInt32(threadno) % queues).ToString(), message => outputtoconsole(counter++, threadno));
                        break;
                    case 3:
                        bus.Subscribe<XMLMessage3>("XML_subscriber" + (Convert.ToInt32(threadno) % queues).ToString(), message => outputtoconsole(counter++, threadno));
                        break;
                    case 4:
                        bus.Subscribe<XMLMessage4>("XML_subscriber" + (Convert.ToInt32(threadno) % queues).ToString(), message => outputtoconsole(counter++, threadno));
                        break;
                    case 5:
                        bus.Subscribe<XMLMessage5>("XML_subscriber" + (Convert.ToInt32(threadno) % queues).ToString(), message => outputtoconsole(counter++, threadno));
                        break;
                    case 6:
                        bus.Subscribe<XMLMessage6>("XML_subscriber" + (Convert.ToInt32(threadno) % queues).ToString(), message => outputtoconsole(counter++, threadno));
                        break;
                    case 7:
                        bus.Subscribe<XMLMessage7>("XML_subscriber" + (Convert.ToInt32(threadno) % queues).ToString(), message => outputtoconsole(counter++, threadno));
                        break;
                    case 8:
                        bus.Subscribe<XMLMessage8>("XML_subscriber" + (Convert.ToInt32(threadno) % queues).ToString(), message => outputtoconsole(counter++, threadno));
                        break;
                    case 9:
                        bus.Subscribe<XMLMessage9>("XML_subscriber" + (Convert.ToInt32(threadno) % queues).ToString(), message => outputtoconsole(counter++, threadno));
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

        static void outputtoconsole(int counter,string threadno)
        {
            if (counter % 100 == 0) 
                Console.WriteLine(threadno + "-" + counter.ToString());

        }
    }

    /// <summary>
    ///  Define command line options
    /// </summary>
    class Options
    {
        
        [Option('a', "amqp-connection-string", Required = false,
            HelpText = "AMQP Connection string.")]
        public string AMQP { get; set; }

        [Option('h', "host-port", HelpText = "Rabbit Host to connect to ( and :port optionally)")]
        public string hostport { get; set; }

        [Option('v', "vhost", HelpText = "Virtual host on Rabbit server to connect to")]
        public string vhost { get; set; }

        [Option('u', "username", HelpText = "RabbitMQ Username")]
        public string username { get; set; }

        [Option('p', "password", HelpText = "RabbitMQ password")]
        public string password { get; set; }

        [Option('t', "threads", HelpText = "Number of threads (ideally divisble by number of queues)")]
        public int threads { get; set; }

        [Option('q', "queues", HelpText = "Number of queues (ideally threads a multiple of queues)")]
        public int queues { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
