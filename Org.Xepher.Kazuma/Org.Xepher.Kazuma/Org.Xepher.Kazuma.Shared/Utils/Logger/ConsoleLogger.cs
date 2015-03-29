using Splat;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Xepher.Kazuma.Utils.Logger
{
    public class ConsoleLogger : ILogger
    {
        public LogLevel Level { get; set; }

        public void Write([Splat.Localizable(false)]string message, LogLevel logLevel)
        {
            LogLevel level = this.Level;

            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
