using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSimulator
{
    public class CDFInterarrivalFixedDuration : IWorkloadGen
    {
        const int granularity_decimalplaces =2;// 2=> 1/10^2 or 0.01

        List<Tuple<double,double>> CDFTuples ; 
        int durationinmilliseconds;
        int nVMs;
        int nArrivals;
        Dictionary<double, double> CDF_approx;

        public CDFInterarrivalFixedDuration(List<Tuple<double, double>> CDFTuples_seconds, int durationinmilliseconds, int nVMs, int nArrivals)
        {
            this.CDFTuples = CDFTuples_seconds;
            this.durationinmilliseconds = durationinmilliseconds;
            this.nArrivals = nArrivals;
            this.nVMs = nVMs;
            this.CDF_approx = new Dictionary<double, double>();

            for(int i=0; i<= CDFTuples.Count-2;i++)
            {
                double x1 = CDFTuples[i].Item1;
                double y1 = CDFTuples[i].Item2;
                double x2 = CDFTuples[i + 1].Item1;
                double y2 = CDFTuples[i+1].Item2;
                double m = (y2 - y1) / (x2 - x1);//slope in y=mx+c
                double c = y1 - m * x1;

                
                double granularity = 1 / Math.Pow(10, granularity_decimalplaces);
                for(double y=y1; y<=y2 ;y=y+granularity)
                {
                    double x = (y - c) / m;
                    CDF_approx[Math.Round(y,granularity_decimalplaces)]= x;
                    y = Math.Round(y, granularity_decimalplaces);
                }
            }

        }


        public List<RequestReceivedEvent> GetArrivals()
        {
            List<double> test = new List<double>();
            List<RequestReceivedEvent> retVal = new List<RequestReceivedEvent>();
            Random rnd = new Random(7718);

            for (int i = 1; i <= nVMs; i++)
            {
                long ts = 0;
                int j = 1;
                while (j <= nArrivals)
                {
                    double r = rnd.NextDouble();
                    double interarrival;

                    if(CDF_approx.ContainsKey(r))
                    {
                        interarrival = CDF_approx[r]; 
                    }
                    else
                    {
                        r = Math.Round(r, granularity_decimalplaces);
                        interarrival = CDF_approx[r];
                    }

                    interarrival = interarrival * 1000;//to ms
                    if (interarrival >= durationinmilliseconds)
                    {
                        test.Add(interarrival);
                        ts += (long)interarrival*1000;// to micro-sec
                        RequestReceivedEvent rxe = new RequestReceivedEvent(new VMId(i), ts, durationinmilliseconds * 1000);
                        retVal.Add(rxe);
                        j++;
                    }
                }

            }

            //test.CDF(10).WriteToFile<MyTuple<double, double>>("test1.txt");
            //test.CDF(100).WriteToFile<MyTuple<double, double>>("test2.txt");
            return retVal;
        }
    }
}
