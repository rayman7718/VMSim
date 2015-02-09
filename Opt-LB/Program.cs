using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSimulator;

namespace Opt_LB
{
    class Program
    {
        static void Main(string[] args)
        {
            int nArrivals = 10;
            int[] nVMs = {700};//{ 250,500, 750,800};
            foreach (int nVM in nVMs)
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


                //IWorkloadGen workload = new FixedArrivalFixedDuration(52010, 7050, nVM, nArrivals);
                IWorkloadGen workload = new CDFInterarrivalFixedDuration(tuples_interarrivals, 7050, nVM, nArrivals);
                GetOPT(nArrivals, nVM, workload);
            }
        }

        static void GetOPT(int nArrivals, int nVMs, IWorkloadGen workload)
        {
            string experimentType = workload.GetType().FullName.Replace("VMSimulator.", "") + "-" + "OPTLB";

            ICostMatrix lxc = new LXCCostMatrix();
            int[] states = lxc.GetVMStates();


            List<MyTuple<double, double>> retVal = new List<MyTuple<double, double>>();
            double h1 = 0, h2 = 0;
            for (int i = 0; i < states.Length - 1; i++)
            {
                {
                    int size1 = 0;
                    for (int j = 0; j <= i; j++)
                        size1 += lxc.GetVMLimit(states[j]);

                    int[] vmlimits1 = { size1, nVMs };
                    CustomCostMatrix cost1 = new CustomCostMatrix(vmlimits1);
                    IPolicy policy = new BeladyEviction();
                    VMSimulator.VMSimulator vmsim = new VMSimulator.VMSimulator(policy, cost1, workload);
                    vmsim.Initialize(nVMs);
                    vmsim.Run();
                    h1 = vmsim.stateAtRequestArrival.PDF(1)[0].Item2;

                    string loggerfile = "logger-" + experimentType + "-" + nVMs + "-VMs.txt";
                    Log.logger.WriteToFile<string>(loggerfile);
                    Log.Flush();
                }

                retVal.Add(new MyTuple<double, double>(i, h1 - h2));
                h2 = h1;
            }

            double last_h = 0;
            foreach (MyTuple<double, double> t in retVal)
                last_h += t.Item2;
            retVal.Add(new MyTuple<double, double>(states.Last(), 1 - last_h));

            string servedfromstatefile = "state-" + experimentType + "-" + nVMs + "-VMs.txt";
            retVal.WriteToFile<MyTuple<double, double>>(servedfromstatefile);

            string statsfile = "stats-" + experimentType + ".txt";
            double optlb = retVal[1].Item2 * lxc.GetTransitionCost(1, 0, null) + retVal[2].Item2 * lxc.GetTransitionCost(2, 0, null);
            List<MyTuple<double, double>> stats = new List<MyTuple<double, double>>();
            stats.Add(new MyTuple<double, double>(nVMs, optlb));
            stats.WriteToFile<MyTuple<double, double>>(statsfile);
        }
    }


    public class CustomCostMatrix : ICostMatrix
    {
        int[] VMStates = { 0, 1 };
        int[] VMLimits;

        public CustomCostMatrix(int[] VMLimits)
        {
            this.VMLimits = VMLimits;
        }

        public int[] GetVMStates()
        {
            return this.VMStates;
        }

        public long GetTransitionCost(int fromstate, int tostate, Dictionary<int, List<VMInfoBase>> currentconfig)
        {
            return 0;
        }

        public int GetVMLimit(int state)
        {
            return VMLimits[state];
        }



    }


}

/*
 * if(i>0)
                {
                    int size2 = 0;
                    for (int j = 0; j <= i - 1; j++)
                        size2 += lxc.GetVMLimit(states[j]);
                    int[] vmlimits2 = { size2, nVMs };
                    CustomCostMatrix cost2 = new CustomCostMatrix(vmlimits2);
                    IPolicy policy = new BeladyEviction();
                    VMSimulator.VMSimulator vmsim = new VMSimulator.VMSimulator(policy, cost2, workload);
                    vmsim.Initialize(nVMs);
                    vmsim.Run();
                    h2=vmsim.stateAtRequestArrival.PDF(1)[0].Item2;
                }*/
