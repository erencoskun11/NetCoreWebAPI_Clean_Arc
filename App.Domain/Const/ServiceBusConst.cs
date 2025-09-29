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
        public const string ProductDeletedEventQueueName = "clean.app.productdeleted.event.queue";
        public const string CategoryDeletedEventQueueName = "clean.app.categorydeleted.event.queue";

        // Yeni: reserve / command queue
        public const string ReserveProductCommandQueueName = "clean.app.reserveproduct.command.queue";
        public const string ReserveCategoryCommandQueueName = "clean.app.reservecategory.command.queue";


    }
}
