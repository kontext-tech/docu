using Microsoft.Extensions.Logging;

namespace Kontext.Logging
{
    /// <summary>
    /// Pre defined logging events in the application
    /// </summary>
    public class LoggingEvents
    {
        public static readonly EventId INIT_DATABASE_ERROR = new EventId(101, "Error while creating and seeding database");
        public static readonly EventId SEND_EMAIL_ERROR = new EventId(201, "Error while sending email");
    }
}
