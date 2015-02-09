using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSimulator
{
    public class EventBase : IComparable<EventBase>
    {
        public long Timestamp { get; private set; }

        public EventBase(long ts)
        {
            this.Timestamp = ts;
        }


        public int CompareTo(EventBase obj)
        {
            int retVal = Timestamp.CompareTo(obj.Timestamp);
            if (retVal != 0)
                return retVal;

            /*
            if((this is VMIdleEvent ) && !(obj is VMIdleEvent))
            {
                retVal = -1;
            }
            
            if ((this is ReconfigureEvent) && !(obj is ReconfigureEvent))
            {
                retVal = -1;
            }

            if ((this is RequestReceivedEvent) && !(obj is RequestReceivedEvent))
            {
                retVal = -1;
            }
            */

            return retVal;
        }
    }


    public class RequestReceivedEvent : EventBase
    {
        public VMId vmid { get; private set; }
        public long Duration { get; private set; }
        public RequestReceivedEvent(VMId vmid, long ts, long duration) : base(ts)
        {
            Duration = duration;
            this.vmid = vmid;
        }


    }

    public class ReconfigureEvent : EventBase
    {
        public ReconfigureEvent(long ts) : base(ts)
        {

        }

    }

    public class ReconfigureForArrival : ReconfigureEvent
    {
        public VMInfoBase vmbase { get; private set; }

        public ReconfigureForArrival(long ts, VMInfoBase vmbase)
            : base(ts)
        {
            this.vmbase = vmbase;
        }

    }

    public class ReconfigureForDeparture : ReconfigureEvent
    {
        public VMInfoBase vmbase { get; private set; }

        public ReconfigureForDeparture(long ts, VMInfoBase vmbase)
            : base(ts)
        {
            this.vmbase = vmbase;
        }

    }


    public class TransitionsEvent: EventBase
    {
        public List<VMTransition> transitions { get; private set; }
        public TransitionsEvent(long ts, List<VMTransition> transitions):base(ts)
        {
            this.transitions = transitions;
        }
    }

    public class VMIdleEvent : EventBase
    {
        public VMId vmid { get; private set; }
        public VMIdleEvent(long ts, VMId vmid) : base(ts)
        {
            this.vmid = vmid;
        }
    }

}
