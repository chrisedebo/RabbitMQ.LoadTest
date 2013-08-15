using System;
using System.Configuration;
using CommandLine;
using CommandLine.Text;

namespace RabbitMQ.LoadTest
{
    /// <summary>
    ///  Define command line options
    /// </summary>
    class LoaderOptions
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

        [Option('r', "reflector", DefaultValue = false, Required = false, HelpText = "Go mental with reflection to set this mofo up...")]
        public bool useReflection { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }

        //private int _arse;
        //public int Arse
        //{
        //    set { _arse = value; }
        //    get { _arse == 0 ? configgy : _arse; }
        //}

        public int ActualQueues { get { return queues != 0 ? queues : Convert.ToInt32(ConfigurationManager.AppSettings["Queues"]); } }
        public int ActualThreads { get { return threads != 0 ? threads : Convert.ToInt32(ConfigurationManager.AppSettings["Threads"]); } }
        public string ActualVhost { get { return vhost ?? ConfigurationManager.AppSettings["VHost"]; } }
        public string ActualUsername { get { return username ?? ConfigurationManager.AppSettings["RabbitMQUser"]; } }
        public string ActualPassword { get { return password ?? ConfigurationManager.AppSettings["RabbitMQPassword"]; } }
        private string[] ActualHostport { get { return hostport != null ? hostport.Split(':') : (ConfigurationManager.AppSettings["Host"] + ":" + ConfigurationManager.AppSettings["Port"]).Split(':'); } }
        public string ActualHost { get { return ActualHostport[0]; } }
        public ushort ActualPort { get { return ActualHostport.Length > 1 ? Convert.ToUInt16(ActualHostport[1]) : Convert.ToUInt16(ConfigurationManager.AppSettings["Port"]); } }
        public string ActualFolderPath { get { return folderpath ?? ConfigurationManager.AppSettings["MessageFilePath"]; }}
    }
}