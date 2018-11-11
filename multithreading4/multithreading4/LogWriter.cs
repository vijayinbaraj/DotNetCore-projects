using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace multithreading4
{
    class LogWriter
    {
        private string m_exePath = string.Empty;
        public LogWriter(string logMessage, String subdir, string logname)
        {
            LogWrite(logMessage, subdir, logname);
        }
        public void LogWrite(string logMessage, string subdir, string logname)
        {
            m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!Directory.Exists(m_exePath + subdir))
            { 
                System.IO.Directory.CreateDirectory(m_exePath + subdir);
            }
            try
            {
                //using (StreamWriter w = File.AppendText(m_exePath + "\\" + subdir + "\\Logs" +"\\" + "log.txt"))
                using (StreamWriter w = File.AppendText(m_exePath + subdir + logname))
                {
                    Log(logMessage, w);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("This is an error : " + ex);
            }
        }

        public void Log(string logMessage, TextWriter txtWriter)
        {
            try
            {
                //txtWriter.Write("\r\nLog Entry : ");
                txtWriter.Write("Log Entry : ");
                txtWriter.WriteLine("{0} {1} {2} {3}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString(), " :",logMessage);
                //txtWriter.WriteLine("  :");
                //txtWriter.WriteLine("  :{0}", logMessage);
                //txtWriter.WriteLine("-------------------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine("This is an error : " + ex);
            }
        }
    }
}
