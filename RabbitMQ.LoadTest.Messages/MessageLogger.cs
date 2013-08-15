using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace RabbitMQ.LoadTest
{
    public class MessageLogger : IDisposable
    {
        private readonly Dictionary<string,int> types = new Dictionary<string, int>();
        private readonly Stopwatch timing;
        private long total = 0;
        private int interval = 5000;
        private long lastLogTime = 0;

        public MessageLogger()
        {
            timing = new Stopwatch();
            timing.Start();
        }
        public void AddType(string type)
        {
            types[type] = 0;
        }

        public void Log(string type)
        {
            Interlocked.Increment(ref total);
            types[type]++;
            // 2 different logging options here. the newer, time-based one is nice when flitting between big & small messages.
            if (timing.ElapsedMilliseconds > lastLogTime + 5000)
            {
                lastLogTime = timing.ElapsedMilliseconds;
                Console.WriteLine("Running for " + timing.Elapsed.TotalMinutes + "mins. Processed " + total + " messages at a rate of " + (total/timing.Elapsed.TotalSeconds) + " per second.");
            }
            //if (total % interval == 0)
            //    Console.WriteLine("Processed " + total + " messages at a rate of " + (total/timing.Elapsed.TotalSeconds) + " per second.");
            //if (types[type] % interval == 0)
            //    Console.WriteLine(type + ": " + types[type]);
        }

        public void Dispose()
        {
            timing.Stop();
            Console.WriteLine("Ran for " + timing.Elapsed.TotalMinutes + " minutes");
            foreach (var type in types)
            {
                Console.WriteLine("-" + type.Key + ": " + type.Value);
            }
            if(timing.Elapsed.Seconds > 0)
                Console.WriteLine("Processed a total of " + total + " at a rate of " + (total/timing.Elapsed.TotalSeconds) + " per second.");
            Console.ReadLine();
        }

    }
}