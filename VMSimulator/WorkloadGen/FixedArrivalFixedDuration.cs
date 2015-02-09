using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSimulator
{
    public class FixedArrivalFixedDuration : IWorkloadGen
    {
        int arrivalPeriodInMilliseconds;
        int durationinmilliseconds;
        int nVMs;
        int nArrivals;

        public FixedArrivalFixedDuration(int arrivalPeriodInMilliseconds, int durationinmilliseconds, int nVMs, int nArrivals)
        {
            this.arrivalPeriodInMilliseconds = arrivalPeriodInMilliseconds;
            this.durationinmilliseconds = durationinmilliseconds;
            this.nVMs = nVMs;
            this.nArrivals = nArrivals;
        }

        public List<RequestReceivedEvent> GetArrivals()
        {
            List<RequestReceivedEvent> retVal = new List<RequestReceivedEvent>();
            Random rnd = new Random(7718);
            
            for (int i = 1; i <= nVMs;i++)
            {
                long f = rnd.Next(0, arrivalPeriodInMilliseconds - 1);
                //long f = arrivalPeriodInMilliseconds / i;
                RequestReceivedEvent rxe = new RequestReceivedEvent(new VMId(i), f * 1000, durationinmilliseconds * 1000);
                retVal.Add(rxe);
                for(int j=1 ; j<=nArrivals -1 ;j++)
                {

                    long ts = f * 1000 + j * (long)arrivalPeriodInMilliseconds * 1000;
                    RequestReceivedEvent rxe1 = new RequestReceivedEvent(new VMId(i),ts, durationinmilliseconds * 1000);
                    retVal.Add(rxe1);

                }

            }

            
            return retVal;
        }
    }
}
