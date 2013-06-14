using System;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using EasyNetQ;
using RabbitMQ.LoadTest.Messages;
using CommandLine;
using CommandLine.Text;

namespace RabbitMQ.LoadTest
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

                //Read message file contents into array for publishing.
                string folderPath = options.folderpath != null ? options.folderpath : fp;
                string[] filecontents = new string[Directory.GetFiles(folderPath).Length];
                int filecounter = 0;

                foreach (string file in Directory.EnumerateFiles(folderPath))
                {
                    filecontents[filecounter] = File.ReadAllText(file);
                    filecounter++;
                }

                //Set up cancellation token
                var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;

                //Start Threads
                Task[] tasks = new Task[threads];
                for (int i = 0; i < tasks.Length; i++)
                {
                    string threadno = i.ToString();
                    tasks[i] = Task.Factory.StartNew(() => Publish(host, port, vhost, username, password, threadno, filecontents, token), token);
                }

                Console.WriteLine("Publisher Started, Hit enter to cancel");
                Console.ReadLine();

                tokenSource.Cancel();
            }
        }

        protected static void Publish(string host, ushort port, string vhost, string username, string password, string ThreadNo, string[] files, CancellationToken token)
        {
            int publisherDelay = options.delay != 0 ? options.delay : d;

            using (var bus = RabbitHutch.CreateBus(host, port, vhost, username, password, 3, serviceRegister => serviceRegister.Register<IEasyNetQLogger>(serviceProvider => new NullLogger())))
            {
                int counter = 0;
                using (var channel = bus.OpenPublishChannel())
                {
                    Random rnd = new Random();

                    while (!token.IsCancellationRequested) //Infinte Loop. Keep publishing until program is stopped.
                    {
                        //Select message type, if anyone has a better way of doing this I'd be interested to hear from you :)
                        switch (Convert.ToInt32(ThreadNo) % options.queues)
                        {
                            case 0:
                                channel.Publish(new XMLMessage0 { XMLString = files[rnd.Next(files.Length)].ToString() });
                                break;
                            case 1:
                                channel.Publish(new XMLMessage1 { XMLString = files[rnd.Next(files.Length)].ToString() });
                                break;
                            case 2:
                                channel.Publish(new XMLMessage2 { XMLString = files[rnd.Next(files.Length)].ToString() });
                                break;
                            case 3:
                                channel.Publish(new XMLMessage3 { XMLString = files[rnd.Next(files.Length)].ToString() });
                                break;
                            case 4:
                                channel.Publish(new XMLMessage4 { XMLString = files[rnd.Next(files.Length)].ToString() });
                                break;
                            case 5:
                                channel.Publish(new XMLMessage5 { XMLString = files[rnd.Next(files.Length)].ToString() });
                                break;
                            case 6:
                                channel.Publish(new XMLMessage6 { XMLString = files[rnd.Next(files.Length)].ToString() });
                                break;
                            case 7:
                                channel.Publish(new XMLMessage7 { XMLString = files[rnd.Next(files.Length)].ToString() });
                                break;
                            case 8:
                                channel.Publish(new XMLMessage8 { XMLString = files[rnd.Next(files.Length)].ToString() });
                                break;
                            case 9:
                                channel.Publish(new XMLMessage9 { XMLString = files[rnd.Next(files.Length)].ToString() });
                                break;
                        }
            
                        counter++;
                        if (counter % 100 == 0) Console.WriteLine(ThreadNo.ToString() + " - " + counter.ToString());
                        Thread.Sleep(rnd.Next(publisherDelay));
                    }

                    Console.WriteLine("Thread {0} stopped", ThreadNo);
                }
            }
        }
    }

    /// <summary>
    ///  Define command line options
    /// </summary>
    class Options
    {
        
        // AMQP Connection strings not yet implemented
        //[Option('a', "amqp-connection-string", Required = false, 
        //    HelpText = "AMQP Connection string.")]
        //public string AMQP { get; set; }

        [Option('h', "host-port", HelpText = "Rabbit Host to connect to ( and :port optionally)")]
        public string hostport { get; set; }

        [Option('v', "vhost", HelpText = "Virtual host on Rabbit server to connect to")]
        public string vhost { get; set; }

        [Option('u', "username", HelpText = "RabbitMQ Username")]
        public string username { get; set; }
        
        [Option('p', "password", HelpText = "RabbitMQ password")]
        public string password { get; set; }

        [Option('f', "folder-path", HelpText = "Path to the message files to be published")]
        public string folderpath { get; set; }

        [Option('t', "threads", HelpText = "Number of threads (ideally divisble by number of queues)")]
        public int threads { get; set; }

        [Option('q', "queues", HelpText = "Number of queues (ideally threads a multiple of queues)")]
        public int queues { get; set; }

        [Option('d', "delay", HelpText = "Maximum random delay (miliseconds)")]
        public int delay { get; set; }
                
        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}

