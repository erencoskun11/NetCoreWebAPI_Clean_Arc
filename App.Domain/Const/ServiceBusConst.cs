using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Const
{
    public class ServiceBusConst
    {
        //<>app.<ecentname>.<queue-name>
        public const string ProductAddedEventQueueName = "clean.app.productadded.event.queue";
        public const string CategoryAddedEventQueueName = "clean.app.categoryadded.event.queue";
    }
}
