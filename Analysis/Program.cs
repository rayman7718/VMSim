using Extreme.Statistics;
using Extreme.Statistics.TimeSeriesAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSimulator;

namespace Analysis
{
    public class AutoCorrelation
    {

        static List<Tuple<double, double>> tuples_durations = new List<Tuple<double, double>>();
        static List<Tuple<double, double>> tuples_interarrivals = new List<Tuple<double, double>>();

        public static void Main()
        {
            //GetApproxCDFs();
            //GetOptimalPQs();
            GetPredictionError();
        }

        public static void GetInterarrivalsStatistics()
        {
            tuples_durations.Add(new Tuple<double, double>(0,0));
            tuples_durations.Add(new Tuple<double, double>(0.1, 0.2));
            tuples_durations.Add(new Tuple<double, double>(1, 0.36));
            tuples_durations.Add(new Tuple<double, double>(5, 0.44));
            tuples_durations.Add(new Tuple<double, double>(8, 0.52));
            tuples_durations.Add(new Tuple<double, double>(10, 0.55));
            tuples_durations.Add(new Tuple<double, double>(27, 0.7));
            tuples_durations.Add(new Tuple<double, double>(30, 0.72));
            tuples_durations.Add(new Tuple<double, double>(60, 0.8));
            tuples_durations.Add(new Tuple<double, double>(120, 0.9));
            tuples_durations.Add(new Tuple<double, double>(300, 1));

            
            tuples_interarrivals.Add(new Tuple<double, double>(0.989952, 0.00128205));
            tuples_interarrivals.Add(new Tuple<double, double>(0.989952, 0.269231));
            tuples_interarrivals.Add(new Tuple<double, double>(2.02776, 0.364103));
            tuples_interarrivals.Add(new Tuple<double, double>(2.71776, 0.410256));
            tuples_interarrivals.Add(new Tuple<double, double>(3.64253, 0.453846));
            tuples_interarrivals.Add(new Tuple<double, double>(4.83293, 0.497436));
            tuples_interarrivals.Add(new Tuple<double, double>(6.34792, 0.539744));
            tuples_interarrivals.Add(new Tuple<double, double>(8.50794, 0.588462));
            tuples_interarrivals.Add(new Tuple<double, double>(10.8415, 0.629487));
            tuples_interarrivals.Add(new Tuple<double, double>(14.6780, 0.680769));
            tuples_interarrivals.Add(new Tuple<double, double>(19.4748, 0.720513));
            tuples_interarrivals.Add(new Tuple<double, double>(25.5797, 0.771795));
            tuples_interarrivals.Add(new Tuple<double, double>(34.9832, 0.810256));
            tuples_interarrivals.Add(new Tuple<double, double>(49.3154, 0.847436));
            tuples_interarrivals.Add(new Tuple<double, double>(70.9377, 0.879487));
            tuples_interarrivals.Add(new Tuple<double, double>(107.325, 0.912821));
            tuples_interarrivals.Add(new Tuple<double, double>(146.780, 0.935897));
            tuples_interarrivals.Add(new Tuple<double, double>(248.163, 0.961538));
            tuples_interarrivals.Add(new Tuple<double, double>(356.970, 0.970513));
            tuples_interarrivals.Add(new Tuple<double, double>(579.639, 0.987179));
            tuples_interarrivals.Add(new Tuple<double, double>(941.205, 0.994872));
            tuples_interarrivals.Add(new Tuple<double, double>(3130.50, 1));
            tuples_interarrivals.Add(new Tuple<double, double>(12872.1, 1));


            //IWorkloadGen workload1 = new CDFInterarrivalFixedDuration(tuples_interarrivals, 7050, 1, 100);
            IWorkloadGen workload1 = new CDFInterarrivalCDFDuration(tuples_interarrivals, tuples_durations, 1, 100);
            List<RequestReceivedEvent> rxes = workload1.GetArrivals();
            List<double> interarrivals = new List<double>();
            List<double> durations = new List<double>();
            for(int i=0; i <= rxes.Count-2; i++)
            {
                double interarrival = rxes[i + 1].Timestamp - rxes[i].Timestamp;
                interarrivals.Add(interarrival);
                if(interarrival<0)
                { 
                }


                durations.Add(rxes[i].Duration);
            }

            Console.WriteLine("interarrivals mean and SD, min, max, median {0}, {1}, {2} {3} {4}", interarrivals.Mean() / 1000.00, interarrivals.StandardDeviation() / 1000.00, interarrivals.Min() / 1000.00, interarrivals.Max() / 1000.00, interarrivals.Median() / 1000.00);
            Console.WriteLine("durations mean and SD {0}, {1}", durations.Mean() / 1000.00, durations.StandardDeviation() / 1000.00);
            
        }

        static void GetPredictionError()
        {
            List<Tuple<double, double>> tuples_interarrivals = new List<Tuple<double, double>>();
            tuples_interarrivals.Add(new Tuple<double, double>(0.989952, 0.00128205));
            tuples_interarrivals.Add(new Tuple<double, double>(0.989952, 0.269231));
            tuples_interarrivals.Add(new Tuple<double, double>(2.02776, 0.364103));
            tuples_interarrivals.Add(new Tuple<double, double>(2.71776, 0.410256));
            tuples_interarrivals.Add(new Tuple<double, double>(3.64253, 0.453846));
            tuples_interarrivals.Add(new Tuple<double, double>(4.83293, 0.497436));
            tuples_interarrivals.Add(new Tuple<double, double>(6.34792, 0.539744));
            tuples_interarrivals.Add(new Tuple<double, double>(8.50794, 0.588462));
            tuples_interarrivals.Add(new Tuple<double, double>(10.8415, 0.629487));
            tuples_interarrivals.Add(new Tuple<double, double>(14.6780, 0.680769));
            tuples_interarrivals.Add(new Tuple<double, double>(19.4748, 0.720513));
            tuples_interarrivals.Add(new Tuple<double, double>(25.5797, 0.771795));
            tuples_interarrivals.Add(new Tuple<double, double>(34.9832, 0.810256));
            tuples_interarrivals.Add(new Tuple<double, double>(49.3154, 0.847436));
            tuples_interarrivals.Add(new Tuple<double, double>(70.9377, 0.879487));
            tuples_interarrivals.Add(new Tuple<double, double>(107.325, 0.912821));
            tuples_interarrivals.Add(new Tuple<double, double>(146.780, 0.935897));
            tuples_interarrivals.Add(new Tuple<double, double>(248.163, 0.961538));
            tuples_interarrivals.Add(new Tuple<double, double>(356.970, 0.970513));
            tuples_interarrivals.Add(new Tuple<double, double>(579.639, 0.987179));
            tuples_interarrivals.Add(new Tuple<double, double>(941.205, 0.994872));
            tuples_interarrivals.Add(new Tuple<double, double>(3130.50, 1));
            tuples_interarrivals.Add(new Tuple<double, double>(12872.1, 1));

            List<Tuple<double, double>> tuples_durations = new List<Tuple<double, double>>();
            tuples_durations.Add(new Tuple<double, double>(0, 0));
            tuples_durations.Add(new Tuple<double, double>(0.1, 0.2));
            tuples_durations.Add(new Tuple<double, double>(1, 0.36));
            tuples_durations.Add(new Tuple<double, double>(5, 0.44));
            tuples_durations.Add(new Tuple<double, double>(8, 0.52));
            tuples_durations.Add(new Tuple<double, double>(10, 0.55));
            tuples_durations.Add(new Tuple<double, double>(27, 0.7));
            tuples_durations.Add(new Tuple<double, double>(30, 0.72));
            tuples_durations.Add(new Tuple<double, double>(60, 0.8));
            tuples_durations.Add(new Tuple<double, double>(120, 0.9));
            tuples_durations.Add(new Tuple<double, double>(300, 1));

            //IWorkloadGen workload = new FixedArrivalFixedDuration(52010, 7050, 800, 25);
            IWorkloadGen workload = new CDFInterarrivalFixedDuration(tuples_interarrivals, 7050, 800, 25);
            //IWorkloadGen workload = new CDFInterarrivalCDFDuration(tuples_interarrivals, tuples_durations, 800, 25);

            List<RequestReceivedEvent> rq= workload.GetArrivals();
            Dictionary<VMId, List<long>> AllArrivals = new Dictionary<VMId, List<long>>();
            Dictionary<VMId, List<double>> AllErrors = new Dictionary<VMId, List<double>>();
            foreach(RequestReceivedEvent r in rq)
            {
                if (!AllArrivals.Keys.Contains(r.vmid))
                {
                    AllArrivals[r.vmid] = new List<long>();
                    AllErrors[r.vmid] = new List<double>();
                }
                AllArrivals[r.vmid].Add(r.Timestamp);
            }

            foreach (VMId v in AllArrivals.Keys)
            {
                List<long> ArrivalTimestamps = AllArrivals[v];
                List<long> Interarrivals = new List<long>();

                for (int i = 1; i <= ArrivalTimestamps.Count - 1; i++)
                    Interarrivals.Add(ArrivalTimestamps[i] - ArrivalTimestamps[i - 1]);

                for (int i = 6; i < Interarrivals.Count - 1; i++)
                {
                    List<double> values = new List<double>();
                    for (int j = 0; j < i; j++)
                        values.Add(Interarrivals[j]);

                    long forecast = SlidingWindow.DoARMA(values);
                    AllErrors[v].Add((forecast - Interarrivals[i]) * (forecast - Interarrivals[i]));
                }
            }


            int[] krange = {250,350,450,550,650,750,800};
            List<MyTuple<double, double>> retVal = new List<MyTuple<double, double>>();
            foreach (int k in krange)
            {
                double totalrmse = 0.0;
                int n = 0;
                double i_min = long.MaxValue, i_max = long.MinValue;
                for (int i = 1; i <= k; i++)
                {
                    VMId v = new VMId(i);
                    foreach (double e in AllErrors[v])
                    {
                        totalrmse = totalrmse + e;
                        n++;
                    }

                    if (i_min > AllArrivals[v].Min())
                        i_min = AllArrivals[v].Min();
                    if (i_max < AllArrivals[v].Max())
                        i_max = AllArrivals[v].Max();

                }
                totalrmse = Math.Sqrt(totalrmse/n)  / 1000000  /(i_max - i_min);

                Console.WriteLine(k + "," + totalrmse);
//                totalrmse = Math.Sqrt(totalrmse / n)/ (1000000 * (MAX_interarrival - MIN_interarrival) );
                
                retVal.Add(new MyTuple<double, double>(k, totalrmse));
            }

            retVal.WriteToFile<MyTuple<double, double>>(workload.GetType()+"-nrmse.txt");
        }

        static void GetOptimalPQs()
        {
            List<Tuple<double, double>> tuples_interarrivals = new List<Tuple<double, double>>();
            tuples_interarrivals.Add(new Tuple<double, double>(0.989952, 0.00128205));
            tuples_interarrivals.Add(new Tuple<double, double>(0.989952, 0.269231));
            tuples_interarrivals.Add(new Tuple<double, double>(2.02776, 0.364103));
            tuples_interarrivals.Add(new Tuple<double, double>(2.71776, 0.410256));
            tuples_interarrivals.Add(new Tuple<double, double>(3.64253, 0.453846));
            tuples_interarrivals.Add(new Tuple<double, double>(4.83293, 0.497436));
            tuples_interarrivals.Add(new Tuple<double, double>(6.34792, 0.539744));
            tuples_interarrivals.Add(new Tuple<double, double>(8.50794, 0.588462));
            tuples_interarrivals.Add(new Tuple<double, double>(10.8415, 0.629487));
            tuples_interarrivals.Add(new Tuple<double, double>(14.6780, 0.680769));
            tuples_interarrivals.Add(new Tuple<double, double>(19.4748, 0.720513));
            tuples_interarrivals.Add(new Tuple<double, double>(25.5797, 0.771795));
            tuples_interarrivals.Add(new Tuple<double, double>(34.9832, 0.810256));
            tuples_interarrivals.Add(new Tuple<double, double>(49.3154, 0.847436));
            tuples_interarrivals.Add(new Tuple<double, double>(70.9377, 0.879487));
            tuples_interarrivals.Add(new Tuple<double, double>(107.325, 0.912821));
            tuples_interarrivals.Add(new Tuple<double, double>(146.780, 0.935897));
            tuples_interarrivals.Add(new Tuple<double, double>(248.163, 0.961538));
            tuples_interarrivals.Add(new Tuple<double, double>(356.970, 0.970513));
            tuples_interarrivals.Add(new Tuple<double, double>(579.639, 0.987179));
            tuples_interarrivals.Add(new Tuple<double, double>(941.205, 0.994872));
            tuples_interarrivals.Add(new Tuple<double, double>(3130.50, 1));
            tuples_interarrivals.Add(new Tuple<double, double>(12872.1, 1));
            CDFInterarrivalFixedDuration w = new CDFInterarrivalFixedDuration(tuples_interarrivals, 0, 1, 150);
            List<RequestReceivedEvent> arrivals = w.GetArrivals();
            List<double> l = new List<double>();
            for (int i = 0; i <= arrivals.Count - 3; i++)
                l.Add(arrivals[i + 1].Timestamp - arrivals[i].Timestamp);

            GetACF(l);


            for(int p=1; p<=10; p++)
            {
                for (int q=1; q <=10 ; q++)
                {
                    NumericalVariable sunspots = new NumericalVariable("sunspots", l.ToArray());
                    ArimaModel model = new ArimaModel(sunspots, p, q);
                    model.Compute();

                    //Console.WriteLine("Variable              Value    Std.Error  t-stat  p-Value");
                    /*foreach (Parameter parameter in model.Parameters)
                        
                        Console.WriteLine("{0,-20}{1,10:F5}{2,10:F5}{3,8:F2} {4,7:F4}",
                            // Name, usually the name of the variable:
                            parameter.Name,
                            // Estimated value of the parameter:
                            parameter.Value,
                            // Standard error:
                            parameter.StandardError,
                            // The value of the t statistic for the hypothesis that the parameter
                            // is zero.
                            parameter.Statistic,
                            // Probability corresponding to the t statistic.
                            parameter.PValue);*/


                    // The log-likelihood of the computed solution is also available:
                    //
                    // as is the Akaike Information Criterion (AIC):
                    //
                    // and the Baysian Information Criterion (BIC):
                    Console.WriteLine("");
                    Console.Write("P:"+p+" Q:"+q); 
                    Console.Write(" BIC: {0:F4}", model.GetBayesianInformationCriterion());
                    Console.Write(" Log-likelihood: {0:F4}", model.GetLogLikelihood());
                    Console.Write(" AIC: {0:F4}", model.GetAkaikeInformationCriterion());
                }
            }



        }

        static void GetApproxCDFs()
        {
            

            CDFInterarrivalFixedDuration w = new CDFInterarrivalFixedDuration(tuples_interarrivals, 0, 1, 1000);
            List<RequestReceivedEvent> arrivals = w.GetArrivals();
            List<double> l = new List<double>();
            for (int i = 0; i <= arrivals.Count-2; i++)
                l.Add(arrivals[i + 1].Timestamp - arrivals[i].Timestamp);

            l.CDF(1000).WriteToFile<MyTuple<double, double>>("cdf-interarrivals.txt");
            l.WriteToFile<double>("drawninterarrivals.txt");
            GetACF(l);

            CDFInterarrivalFixedDuration w1 = new CDFInterarrivalFixedDuration(tuples_durations, 0, 1, 1000);
            List<RequestReceivedEvent> durations = w1.GetArrivals();
            List<double> l1 = new List<double>();
            for (int i = 0; i <= durations.Count - 2; i++)
                l1.Add(durations[i + 1].Timestamp - durations[i].Timestamp);

            l1.CDF(1000).WriteToFile<MyTuple<double, double>>("cdf-durations.txt");
            
        }


        static void GetACF(List<double> vals)
        {
            Console.WriteLine("Computing Autocorrelation...");
            var q = AutoCorrelation.GetAutoCorrelationOfSeries(vals.ToArray());
            File.Delete("result-acf.txt");
            for (int i = 0; i < q.Length; i++)
            {
                File.AppendAllText("result-acf.txt", q[i].ToString() + "\r\n");
            }
            Console.WriteLine("DONE");
        }

        public static double GetAverage(double[] data)
        {
            int len = data.Length;

            if (len == 0)
                throw new Exception("No data");

            double sum = 0;

            for (int i = 0; i < data.Length; i++)
                sum += data[i];

            return sum / len;
        }

        public static double GetVariance(double[] data)
        {
            int len = data.Length;

            // Get average
            double avg = GetAverage(data);

            double sum = 0;

            for (int i = 0; i < data.Length; i++)
                sum += System.Math.Pow((data[i] - avg), 2);

            return sum / len;
        }
        public static double GetStdev(double[] data)
        {
            return Math.Sqrt(GetVariance(data));
        }

        public static double GetCorrelation(double[] x, double[] y)
        {
            if (x.Length != y.Length)
                throw new Exception("Length of sources is different");
            double avgX = GetAverage(x);
            double stdevX = GetStdev(x);
            double avgY = GetAverage(y);
            double stdevY = GetStdev(y);
            double covXY = 0;
            double pearson = 0;
            int len = x.Length;
            for (int i = 0; i < len; i++)
                covXY += (x[i] - avgX) * (y[i] - avgY);
            covXY /= len;
            pearson = covXY / (stdevX * stdevY);
            return pearson;
        }

        public static double[] GetAutoCorrelationOfSeries(double[] x)
        {
            int half = (int)x.Length / 2;
            double[] autoCorrelation = new double[half];
            double[] a = new double[half];
            double[] b = new double[half];
            for (int i = 0; i < half; i++)
            {
                double s = 0;
                double mean = GetAverage(x);
                for (int j = 0; j < x.Length - i - 1; j++)
                {
                    s += (x[j] - mean) * (x[j + i] - mean);
                }
                autoCorrelation[i] = s / (GetVariance(x) * x.Length);
                /*
                a[i] = x[i];
            b[i] = x[i + i];
            autoCorrelation[i] = GetCorrelation(a, b);
            if (i % 1000 == 0)
                Console.WriteLine(i);*/
            }
            return autoCorrelation;
        }
    }
}
