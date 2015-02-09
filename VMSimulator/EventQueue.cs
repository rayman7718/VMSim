using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSimulator
{
    public class EventQueue
    {
        BinaryHeap<EventBase> queue;
        // private List<EventBase> queue;

        public EventQueue()
        {
            this.queue = new BinaryHeap<EventBase>();
            //this.queue = new List<EventBase>();
        }

        public void Initialize(List<RequestReceivedEvent> events)
        {
            foreach (EventBase e in events)
                queue.Add(e);

            //queue.Sort();
        }

        public EventBase PopEvent()
        {
            EventBase retVal = null;
            lock (queue)
            {
                //queue.Sort();
                if (queue.Count > 0)
                {
                    retVal = queue.Remove();
                    //retVal = queue.ElementAt(0);
                    //queue.RemoveAt(0);
                }
            }
            return retVal;
        }

        public void AddEvent(EventBase e)
        {
            lock (queue)
            {
                /*
                int i = 0;
                while(i < queue.Count)
                {
                    if (queue.ElementAt(i).CompareTo(e) >= 0)
                        break;
                    i++;
                }*/

                //queue.Insert(i, e);
                queue.Add(e);
            }

        }

        public BinaryHeap<EventBase> GetHeap()
        {
            return queue;
        }

        public int Count()
        {
            return queue.Count;
        }

    }
}
