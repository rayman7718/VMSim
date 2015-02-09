using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSimulator
{
    public class Log
    {
        public const int level = 0;
        public static List<string> logger { get;  private set; }
        private enum LogType { Debug = 0, Warning = 1, Eval = 3, Exception }

        /*public static List<string> GetLogs()
        {
            return logger;
        }*/

        public static void Flush()
        {
            logger = new List<string>();
        }

        public static void exception(string text)
        {
            printLog(text, LogType.Exception);
        }

        public static void debug(string text)
        {
            printLog(text, LogType.Debug);
        }
        public static void warning(string text)
        {
            printLog(text, LogType.Warning);
        }

        private static void printLog(string str, LogType t)
        {

            if (Log.level <= (int)t)
            {
                string tolog = "";
                if (t.Equals(LogType.Exception))
                    tolog += DateTime.Now + "Exception ";
                if (t.Equals(LogType.Debug))
                    tolog += DateTime.Now + "Debug ";
                if (t.Equals(LogType.Warning))
                    tolog += DateTime.Now + "Warning ";
                if (t.Equals(LogType.Warning))
                    tolog += DateTime.Now + "Eval ";
                tolog += Stopwatch.GetTimestamp() + " ";
                //tolog += Globals.GetCurrentTS() + " ";

                tolog += str;
                //Console.WriteLine(tolog);

                if (logger == null)
                    logger = new List<string>();

                logger.Add(tolog);
            }
        }

    }
}
