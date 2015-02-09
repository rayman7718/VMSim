using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSimulator
{
    public class ExponentialArrivalFixedDuration : IWorkloadGen
    {
        int arrivalMeanInMilliseconds;
        int durationinmilliseconds;
        int nVMs;
        int nArrivals;


        public ExponentialArrivalFixedDuration(int arrivalMeanInMilliseconds, int durationinmilliseconds, int nVMs, int nArrivals)
        {
            this.arrivalMeanInMilliseconds = arrivalMeanInMilliseconds;
            this.durationinmilliseconds = durationinmilliseconds;
            this.nArrivals = nArrivals;
            this.nVMs = nVMs;
        }


        public List<RequestReceivedEvent> GetArrivals()
        {
            List<RequestReceivedEvent> retVal = new List<RequestReceivedEvent>();
            Random rnd = new Random(7718);

            for (int i = 1; i <= nVMs; i++)
            {
                long ts = 0;
                int j = 1;
                while(j<=nArrivals)
                {
                    double r = rnd.NextDouble();
                    double interarrival = Math.Log(1 / (1 - r)) * arrivalMeanInMilliseconds;

                    if(interarrival >= durationinmilliseconds)
                    {
                        ts += (long)interarrival;
                        RequestReceivedEvent rxe = new RequestReceivedEvent(new VMId(i), ts * 1000 , durationinmilliseconds * 1000);
                        retVal.Add(rxe);
                        j++;
                    }
                }

            }
            
            return retVal;
        }
    }
}
