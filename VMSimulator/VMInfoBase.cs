using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSimulator
{
    public class VMInfoBase
    {
        public VMId vmid { get; private set; }
        public bool isIdle;

        public List<long> ArrivalTimestamp { get; private set; }
        public List<long> ArrivalDurations { get; private set; }

        public long IdleEventTimestamp;


        public long nextUseTs;

        public VMInfoBase(VMId id, int state)
        {
            vmid = id;
            isIdle = true;
            ArrivalTimestamp = new List<long>();
            ArrivalDurations = new List<long>();
            IdleEventTimestamp = 0;
            nextUseTs = long.MaxValue;
        }

        public void AddArrivalTS(long ts)
        {
            ArrivalTimestamp.Add(ts);
        }

        public void AddArrivalDuration(long d)
        {
            ArrivalDurations.Add(d);
        }

    }
}
