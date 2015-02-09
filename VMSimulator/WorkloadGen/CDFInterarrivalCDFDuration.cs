using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSimulator
{
    public class CDFInterarrivalCDFDuration : IWorkloadGen
    {
        const int granularity_decimalplaces =2;// 2=> 1/10^2 or 0.01

        List<Tuple<double,double>> CDFTuples_interarrivals ;
        List<Tuple<double, double>> CDFTuples_duration;
        int nVMs;
        int nArrivals;
        Dictionary<double, double> CDF_interarrivals_approx;

        Dictionary<double, double> CDF_duration_approx;

        public CDFInterarrivalCDFDuration(List<Tuple<double, double>> CDFTuples_seconds, List<Tuple<double, double>> durationinmilliseconds, int nVMs, int nArrivals)
        {
            this.CDFTuples_interarrivals = CDFTuples_seconds;
            this.CDFTuples_duration = durationinmilliseconds;
            this.nArrivals = nArrivals;
            this.nVMs = nVMs;
            this.CDF_interarrivals_approx = new Dictionary<double, double>();
            this.CDF_duration_approx = new Dictionary<double, double>();

            for(int i=0; i<= CDFTuples_interarrivals.Count-2;i++)
            {
                double x1 = CDFTuples_interarrivals[i].Item1;
                double y1 = CDFTuples_interarrivals[i].Item2;
                double x2 = CDFTuples_interarrivals[i + 1].Item1;
                double y2 = CDFTuples_interarrivals[i+1].Item2;
                double m = (y2 - y1) / (x2 - x1);//slope in y=mx+c
                double c = y1 - m * x1;

                
                double granularity = 1 / Math.Pow(10, granularity_decimalplaces);
                for(double y=y1; y<=y2 ;y=y+granularity)
                {
                    double x = (y - c) / m;
                    CDF_interarrivals_approx[Math.Round(y,granularity_decimalplaces)]= x;
                    y = Math.Round(y, granularity_decimalplaces);
                }
            }

            for (int i = 0; i <=  CDFTuples_duration.Count - 2; i++)
            {
                double x1 = CDFTuples_duration[i].Item1;
                double y1 = CDFTuples_duration[i].Item2;
                double x2 = CDFTuples_duration[i + 1].Item1;
                double y2 = CDFTuples_duration[i + 1].Item2;
                double m = (y2 - y1) / (x2 - x1);//slope in y=mx+c
                double c = y1 - m * x1;


                double granularity = 1 / Math.Pow(10, granularity_decimalplaces);
                for (double y = y1; y <= y2; y = y + granularity)
                {
                    double x = (y - c) / m;
                    CDF_duration_approx[Math.Round(y, granularity_decimalplaces)] = x;
                    y = Math.Round(y, granularity_decimalplaces);
                }
            }

        }


        public List<RequestReceivedEvent> GetArrivals()
        {
            List<double> test = new List<double>();
            List<RequestReceivedEvent> retVal = new List<RequestReceivedEvent>();
            Random rnd1 = new Random(7718);
            Random rnd2 = new Random(7719);

            for (int i = 1; i <= nVMs; i++)
            {
                long ts = 0;
                int j = 1;
                while (j <= nArrivals)
                {
                    double r1 = rnd1.NextDouble();
                    double r2 = rnd2.NextDouble();
                    double interarrival;
                    double durationinmilliseconds;

                    if(CDF_interarrivals_approx.ContainsKey(r1))
                    {
                        interarrival = CDF_interarrivals_approx[r1]; 
                    }
                    else
                    {
                        r1 = Math.Round(r1, granularity_decimalplaces);
                        interarrival = CDF_interarrivals_approx[r1];
                    }

                    if (CDF_duration_approx.ContainsKey(r2))
                    {
                        durationinmilliseconds = CDF_duration_approx[r2];
                    }
                    else
                    {
                        r2 = Math.Round(r2, granularity_decimalplaces);
                        durationinmilliseconds = CDF_duration_approx[r2];
                    }


                    durationinmilliseconds = durationinmilliseconds * 1000;
                    interarrival = interarrival * 1000;//to ms
                    if (interarrival >= durationinmilliseconds)
                    {
                        test.Add(interarrival);
                        ts += (long)interarrival*1000;// to micro-sec
                        RequestReceivedEvent rxe = new RequestReceivedEvent(new VMId(i), ts, (long)(durationinmilliseconds * 1000));
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
