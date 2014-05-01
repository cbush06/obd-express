using System;
using log4net.Appender;
using log4net.Core;
using System.Text;
using log4net;
using System.IO.Pipes;
using System.IO;

namespace ObdExpress.Global
{
    public class LogTapAppender : AppenderSkeleton
    {
        // Event listeners can register on
        public event NoReturnWithStringParam MessageLogging;

        public LogTapAppender() : base()
        {
            // Ensure the handle for the pipe server is added to the GlobalContext map
            GlobalContext.Properties[Variables.MAP_KEY_LOG4NET_LOG_TAP_INSTANCE] = this;
        }

        protected override bool RequiresLayout
        {
            get
            {
                return true;
            }
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            // If there are listeners, send them the message
            if (MessageLogging != null)
            {
                MessageLogging(this.RenderLoggingEvent(loggingEvent));
            }
        }
    }
}
