using System;
using System.Configuration;
using CommandLine;
using CommandLine.Text;

namespace RabbitMQ.LoadTest.Subscriber
{
    /// <summary>
    ///  Define command line options
    /// </summary>
    class SubscriberOptions
    {
        
        [Option('a', "amqp-connection-string", Required = false,
            HelpText = "AMQP Connection string.")]
        public string AMQP { get; set; }

        [Option('e', "easynetq-threading", DefaultValue = false, Required = false, HelpText = "Allow EasyNetQ to handle threading with Async subscriptions")]
        public bool useAsync { get; set; }

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

        public int ActualQueues { get { return queues != 0 ? queues : Convert.ToInt32(ConfigurationManager.AppSettings["Queues"]); } }
        public int ActualThreads { get { return threads != 0 ? threads : Convert.ToInt32(ConfigurationManager.AppSettings["Threads"]); } }
        public string ActualVhost { get { return vhost ?? ConfigurationManager.AppSettings["VHost"]; } }
        public string ActualUsername { get { return username ?? ConfigurationManager.AppSettings["RabbitMQUser"]; } }
        public string ActualPassword { get { return password ?? ConfigurationManager.AppSettings["RabbitMQPassword"]; } }
        private string[] ActualHostport { get { return hostport != null ? hostport.Split(':') : (ConfigurationManager.AppSettings["Host"] + ":" + ConfigurationManager.AppSettings["Port"]).Split(':'); } }
        public string ActualHost { get { return ActualHostport[0]; } }
        public ushort ActualPort { get { return ActualHostport.Length > 1 ? Convert.ToUInt16(ActualHostport[1]) : Convert.ToUInt16(ConfigurationManager.AppSettings["Port"]); } }
    }
}