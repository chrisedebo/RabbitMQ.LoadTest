using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
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
        protected static LoaderOptions options = new LoaderOptions();

        private static MessageLogger logger;

        static void Main(string[] args)
        {
            //Parse command line options and quit if invalid.
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                //Read message file contents into array for publishing.
                string[] filecontents = new string[1];// new string[Directory.GetFiles(options.ActualFolderPath).Length];
                int filecounter = 0;

                foreach (string file in Directory.EnumerateFiles(options.ActualFolderPath).Take(1))
                {
                    filecontents[filecounter] = File.ReadAllText(file);
                    filecounter++;
                }
                using (logger = new MessageLogger())
                {
                    if (options.useReflection)
                        DoReflectyPublish(filecontents);
                    else
                        DoOriginalPublish(filecontents);
                }
            }
        }

        private static void DoReflectyPublish(string[] filecontents)
        {
            //Set up cancellation token
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            var utils = new Utils();
            var tasks = new List<Task>();

            IBus bus;
            bus = CreateBus();
            foreach (var message in utils.All.Take(options.ActualQueues))
            {
                logger.AddType(message.Name);
                //bus = CreateBus();
                var channel = bus.OpenPublishChannel();

                MethodInfo openPublishMethod = channel.GetType().GetMethods().First(x => x.Name == "Publish" && x.GetParameters().Length == 1);
                MethodInfo closedPublishMethod = openPublishMethod.MakeGenericMethod(message);
                tasks.Add(Task.Factory.StartNew(() => CreateReflectyPublisher(message, closedPublishMethod, filecontents, channel, token)));

            }

            Console.WriteLine("Reflecting Publisher Started, Hit enter to cancel");
            Console.ReadLine();

            tokenSource.Cancel();
            tasks.ForEach(x => x.Wait());
            bus.Dispose();
        }

        private static void CreateReflectyPublisher(Type messageType, MethodInfo closedPublishMethod, string[] filecontents, IPublishChannel channel, CancellationToken token)
        {
            var rnd = new Random();

            while (!token.IsCancellationRequested) //Infinte Loop. Keep publishing until program is stopped.
            {
                var args = new object[1];
                args[0] = Activator.CreateInstance(messageType);
                ((BaseMessage) args[0]).XMLString = filecontents[rnd.Next(filecontents.Length)];
                closedPublishMethod.Invoke(channel, args);
                logger.Log(messageType.Name);
            }
            //var bus = channel.Bus;
            channel.Dispose();
            //bus.Dispose();
        }




        private static void DoOriginalPublish(string[] filecontents)
        {
            //Set up cancellation token
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            //Start Threads
            Task[] tasks = new Task[options.ActualThreads];
            for (int i = 0; i < tasks.Length; i++)
            {
                string threadno = i.ToString();
                logger.AddType(threadno);
                tasks[i] = Task.Factory.StartNew(() => Publish(threadno, filecontents, token), token);
            }

            Console.WriteLine("Publisher Started, Hit enter to cancel");
            Console.ReadLine();

            tokenSource.Cancel();
            tasks.ToList().ForEach(x => x.Wait());
        }

        protected static void Publish(string ThreadNo, string[] files, CancellationToken token)
        {
            //int publisherDelay = options.delay != 0 ? options.delay : d;

            using (var bus = CreateBus())
            {
                using (var channel = bus.OpenPublishChannel())
                {
                    Random rnd = new Random();

                    while (!token.IsCancellationRequested) //Infinte Loop. Keep publishing until program is stopped.
                    {
                        //Select message type, if anyone has a better way of doing this I'd be interested to hear from you :)
                        switch (Convert.ToInt32(ThreadNo) % options.ActualQueues)
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
                        logger.Log(ThreadNo);
            
                        //Thread.Sleep(rnd.Next(publisherDelay));
                    }

                    Console.WriteLine("Thread {0} stopped", ThreadNo);
                }
            }
        }
        private static IBus CreateBus()
        {
            return RabbitHutch.CreateBus(options.ActualHost, options.ActualPort, options.ActualVhost, options.ActualUsername, options.ActualPassword, 3, serviceRegister => serviceRegister.Register<IEasyNetQLogger>(serviceProvider => new NullLogger()));
        }
    }
}

