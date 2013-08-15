using System;
using System.Collections.Generic;
using System.Linq;
using RabbitMQ.LoadTest.Messages;

namespace RabbitMQ.LoadTest
{
    public class Utils
    {
        public IList<Type> All { get; set; }

        public Utils()
        {
            var allTypes = this.GetType().Assembly.GetTypes();
            All = allTypes.Where(x => x.IsSubclassOf(typeof(BaseMessage))).ToList();
        }
    }
}