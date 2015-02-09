using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSimulator
{
    public class Globals
    {

        private static long CurrentTS = 0;//in microseconds


        public static long GetCurrentTS()
        {
            return CurrentTS;
        }

        public static void SetCurrentTS(long ts)
        {
            CurrentTS = ts;
        }

    }
}
