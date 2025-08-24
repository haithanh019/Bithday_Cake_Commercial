using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultitity.Email.Interface;

namespace Ultitity.Email
{
    public class EmailQueue : IEmailQueue
    {
        private readonly ConcurrentQueue<(
            string Email,
            string Subject,
            string HtmlMessage
        )> _queue = new();

        public void QueueEmail(string email, string subject, string htmlMessage) =>
            _queue.Enqueue((email, subject, htmlMessage));

        public bool TryDequeue(out (string Email, string Subject, string HtmlMessage) emailData) =>
            _queue.TryDequeue(out emailData);
    }

}
