using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace DevTools
{
    public class FileLogger
    {
        /// <summary>
        /// Mutex to allow multiple processes to write to the file.
        /// </summary>
        private static readonly Mutex mutex = new Mutex(false, @"Global\TransClearLogFile");

        /// <summary>
        /// The log file path.
        /// </summary>
        private readonly string logPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogger"/> class.
        /// </summary>
        /// <param name="logPath">The log path.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public FileLogger(string logPath)
        {
            this.logPath = logPath;
        }

        /// <summary>
        /// Writes the specified line to the log file.
        /// </summary>
        /// <param name="line">The line.</param>
        private void Write(string line)
        {
            try
            {
                var time_stap = DateTime.Now.ToShortTimeString();

                //  Wait for access via the mutex.
                mutex.WaitOne();

                //  Write to the line to the file.
                using (var w = File.AppendText(logPath))
                    w.WriteLine(time_stap + " " + line);
            }
            catch (Exception exception)
            {
                //Debug.WriteLine("An exception occured trying to write to the file log. Details: {0}", exception);
            }
            finally
            {
                //  Release the mutex.
                mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="error">The error.</param>
        public void LogError(string error)
        {
            Write("Error: " + error);
        }

        /// <summary>
        /// Logs a warning.
        /// </summary>
        /// <param name="warning">The warning.</param>
        public void LogWarning(string warning)
        {
            Write("Warning: " + warning);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void LogMessage(string message)
        {
            Write(message);
        }

        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="error">The error.</param>
        public void LogError(Exception e)
        {
            var error_message = new StringBuilder();

            error_message.AppendLine("Error:");
            error_message.AppendLine("Message:" + e.Message);
            error_message.AppendLine("Source:" + e.Source);
            error_message.AppendLine("Stack trace:");
            error_message.AppendLine(e.StackTrace);

            Write(error_message.ToString());
        }
    }
}
