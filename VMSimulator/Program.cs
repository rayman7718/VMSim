using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Extreme.Mathematics;
using Extreme.Statistics;
using Extreme.Statistics.TimeSeriesAnalysis;

namespace VMSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            IPolicy policy1 = new LRUReactivePolicy();
            IPolicy policy2 = new BeladyEviction();
            IPolicy policy3 = new SlidingWindowLastInterarrival();
            IPolicy policy4 = new SlidingWindowARMA();
            IPolicy policy5 = new SlidingWindowGroundTruth();

            EvalPolicy(policy1);
            EvalPolicy(policy2);
            EvalPolicy(policy3);
            EvalPolicy(policy4);
            EvalPolicy(policy5);
        }
        static void EvalPolicy(IPolicy policy)
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

            ICostMatrix costmatrix = new LXCCostMatrix();
            int nArrivals = 25;

            //int[] nVMs = { 250,350,450,550,650, 750, 800 };//260,270,280,290,300,310,320,330,340,
            int[] nVMs = { 260, 270, 280, 290, 300, 310, 320, 330, 340, 560, 570, 580, 590, 600, 610, 620, 630, 640 };
            foreach (int nVM in nVMs)
            {
                //IWorkloadGen workload = new FixedArrivalFixedDuration(52010, 7050, nVM, nArrivals);
                //IWorkloadGen workload = new CDFInterarrivalFixedDuration(tuples_interarrivals, 7050, nVM, nArrivals);
                IWorkloadGen workload = new CDFInterarrivalCDFDuration(tuples_interarrivals, tuples_durations, nVM, nArrivals);

                string experimentType = workload.GetType().FullName.Replace("VMSimulator.", "") + "-" + policy.GetType().FullName.Replace("VMSimulator.", "");
                string latenciesfile = "latencies-" + experimentType + "-" + nVM + "-VMs.txt";
                string cdffile = "cdf-" + experimentType + "-" + nVM + "-VMs.txt";
                string pdffile = "pdf-" + experimentType + "-" + nVM + "-VMs.txt";
                string loggerfile = "logger-" + experimentType + "-" + nVM + "-VMs.txt";
                string servedfromstatefile = "state-" + experimentType + "-" + nVM + "-VMs.txt";
                string statsfile = "stats-" + experimentType + ".txt";

                VMSimulator vmsim = new VMSimulator(policy, costmatrix, workload);//
                vmsim.Initialize(nVM);
                vmsim.Run();
                List<MyTuple<double, double>> stats = new List<MyTuple<double, double>>();
                stats.Add(new MyTuple<double, double>(nVM, vmsim.latency_penalties.Mean()));
                stats.WriteToFile<MyTuple<double, double>>(statsfile);

                vmsim.stateAtRequestArrival.PDF(1).WriteToFile<MyTuple<MyTuple<double, double>, double>>(servedfromstatefile);

                vmsim.latency_penalties.WriteToFile<double>(latenciesfile);
                //vmsim.latency_penalties.CDF(10).WriteToFile<MyTuple<double, double>>(cdffile);
                vmsim.latency_penalties.PDF(100).WriteToFile<MyTuple<MyTuple<double, double>, double>>(pdffile);

                Log.logger.WriteToFile<string>(loggerfile);
                Log.Flush();

            }
        }
    }
}
/*
 *             List<Tuple<double,double>> t = new List<Tuple<double,double>>();
            t.Add(new Tuple<double,double>(0,0));
            t.Add(new Tuple<double, double>(10000, 0.55));
            t.Add(new Tuple<double, double>(20000, 0.78));
            t.Add(new Tuple<double, double>(30000, 0.95));
            t.Add(new Tuple<double, double>(30000, 0.97));
            t.Add(new Tuple<double,double>(50000,1));
            CDFInterarrivalFixedDuration c = new CDFInterarrivalFixedDuration(t, 10000, 1, 100);

 // This QuickStart Sample fits an ARMA(2,1) model and
            // an ARIMA(0,1,1) model to sunspot data.

            // The time series data is stored in a numerical variable:
            NumericalVariable sunspots = new NumericalVariable("sunspots", new double[] {
                100.8, 81.6, 66.5, 34.8, 30.6, 7, 19.8, 92.5,
                154.4, 125.9, 84.8, 68.1, 38.5, 22.8, 10.2, 24.1, 82.9,
                132, 130.9, 118.1, 89.9, 66.6, 60, 46.9, 41, 21.3, 16,
                6.4, 4.1, 6.8, 14.5, 34, 45, 43.1, 47.5, 42.2, 28.1, 10.1,
                8.1, 2.5, 0, 1.4, 5, 12.2, 13.9, 35.4, 45.8, 41.1, 30.4,
                23.9, 15.7, 6.6, 4, 1.8, 8.5, 16.6, 36.3, 49.7, 62.5, 67,
                71, 47.8, 27.5, 8.5, 13.2, 56.9, 121.5, 138.3, 103.2,
                85.8, 63.2, 36.8, 24.2, 10.7, 15, 40.1, 61.5, 98.5, 124.3,
                95.9, 66.5, 64.5, 54.2, 39, 20.6, 6.7, 4.3, 22.8, 54.8,
                93.8, 95.7, 77.2, 59.1, 44, 47, 30.5, 16.3, 7.3, 37.3,
                73.9});

            // ARMA models (no differencing) are constructed from
            // the variable containing the time series data, and the
            // AR and MA orders. The following constructs an ARMA(2,1)
            // model:
            ArimaModel model = new ArimaModel(sunspots, 2, 1);

            // The Compute methods fits the model.
            model.Compute();

            // The model's Parameters collection contains the fitted values.
            // For an ARIMA(p,d,q) model, the first p parameters are the 
            // auto-regressive parameters. The last q parametere are the
            // moving average parameters.
            Console.WriteLine("Variable              Value    Std.Error  t-stat  p-Value");
            foreach (Parameter parameter in model.Parameters)
                // Parameter objects have the following properties:
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
                    parameter.PValue);


            // The log-likelihood of the computed solution is also available:
            Console.WriteLine("Log-likelihood: {0:F4}", model.GetLogLikelihood());
            // as is the Akaike Information Criterion (AIC):
            Console.WriteLine("AIC: {0:F4}", model.GetAkaikeInformationCriterion());
            // and the Baysian Information Criterion (BIC):
            Console.WriteLine("BIC: {0:F4}", model.GetBayesianInformationCriterion());

            // The Forecast method can be used to predict the next value in the series...
            double nextValue = model.Forecast();
            Console.WriteLine("One step ahead forecast: {0:F3}", nextValue);

            // or to predict a specified number of values:
            Vector nextValues = model.Forecast(5);
            Console.WriteLine("First five forecasts: {0:F3}", nextValues);   
*/