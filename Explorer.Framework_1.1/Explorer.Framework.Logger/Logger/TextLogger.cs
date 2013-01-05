using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;

namespace Explorer.Framework.Logger
{
    public class TextLogger : BaseLogger
    {
        private string _realLogFilename = String.Empty;

        public TextLogger(string name, DataRow row)
            : base(name, row)
        {
            if (row == null)
            {
                this.Level = "DEBUG,ERROR";
                this.IsOutputConsole = false;
                this.Template = "[${DateTime}][${Level}][${TypeName}][${ThreadName}]${NewLine}${Message}${NewLine}${Exception}";
                this.Position = "${Path}${Date}.log.txt";
            }
            this._realLogFilename = string.Empty;
        }

        private string GetRealLogFilename()
        {
            if (_realLogFilename == String.Empty)
            {
                string outputFile = this.Position;
                outputFile = this.ConvertVar(outputFile);
                return outputFile;
            }
            else
            {
                return _realLogFilename;
            }
        }

        public override void LogWrite(string level, string message, Exception exception)
        {
            lock (LoggerFactory.Instance)
            {
                if (this.Level.ToUpper().IndexOf(level.ToUpper()) < 0)
                {
                    return;
                }

                string outputText = this.Template;
                outputText = this.ConvertVar(outputText);
                outputText = outputText.Replace("${Level}", level);
                outputText = outputText.Replace("${Message}", message);
                if (exception != null)
                {
                    outputText = outputText.Replace("${Exception}", exception.StackTrace);
                }
                else
                {
                    outputText = outputText.Replace("${Exception}", "");
                }

                if (this.IsOutputConsole)
                {
                    System.Console.WriteLine(outputText);
                }

                LogOutput(outputText);
            }
        }

        protected override void LogOutput(object input)
        {
            string path = GetRealLogFilename();

            if (!File.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            try
            {
                File.AppendAllText(path, (string)input);
                File.AppendAllText(path, "\r\n");
            }
            catch (Exception ex) { }
        }

    }
}
