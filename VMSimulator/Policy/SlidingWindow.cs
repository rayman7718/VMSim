using Extreme.Mathematics;
using Extreme.Statistics;
using Extreme.Statistics.TimeSeriesAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSimulator
{
    public class SlidingWindowGroundTruth : SlidingWindow
    {
        public SlidingWindowGroundTruth()
            : base()
        {
            this.NextUse = NextUseMode.GroundTruth;
        }
    }

    public class SlidingWindowLastInterarrival : SlidingWindow
    {
        public SlidingWindowLastInterarrival():base()
        {
            this.NextUse = NextUseMode.Infer;
        }
    }

    public class SlidingWindowARMA : SlidingWindow
    {
        public SlidingWindowARMA()
            : base()
        {
            this.NextUse = NextUseMode.ARMA;
        }
    }

    public class SlidingWindow : IPolicy
    {
        public const int Model_P = 2;
        public const int Model_Q = 1; 

        protected enum NextUseMode { GroundTruth = 0, Infer, ARMA } ;
        protected NextUseMode NextUse ;

        public Dictionary<int, List<VMInfoBase>> GetNewConfig(EventQueue queue, ReconfigureEvent e, Dictionary<int, List<VMInfoBase>> currentConfig, Dictionary<VMId, VMInfoBase> vminfos, ICostMatrix costmatrix)
        {
            if(e is ReconfigureForArrival)
            {
                VMInfoBase v = ((ReconfigureForArrival)e).vmbase;

                if (VMSimulator.GetStateOfVM(v, currentConfig).Equals(0))// if in booted do nothing.
                    return currentConfig;

                List<VMInfoBase> PutToBooted = new List<VMInfoBase>();
                List<VMInfoBase> PutToFrozen = new List<VMInfoBase>();
                List<VMInfoBase> PutToShutdown = new List<VMInfoBase>();

                List<VMInfoBase> AllIdleVMsInfo = new List<VMInfoBase>();
                foreach(VMId vmid in vminfos.Keys)
                {
                    VMInfoBase vminfo = vminfos[vmid];
                    if (vminfo.isIdle)
                    {
                        AllIdleVMsInfo.Add(vminfo);
                    }
                    else
                    {
                        PutToBooted.Add(vminfo);
                    }
                }

                PopulateNextUseTs(AllIdleVMsInfo, queue);
                AllIdleVMsInfo.Sort(delegate(VMInfoBase a, VMInfoBase b) { return a.nextUseTs.CompareTo(b.nextUseTs); });

                int remainingBootedCapacity = costmatrix.GetVMLimit(0) - PutToBooted.Count;
                for(int i=1; i <=remainingBootedCapacity; i++)
                {
                    if (AllIdleVMsInfo.Count > 0)
                    {
                        VMInfoBase temp = AllIdleVMsInfo.First();
                        AllIdleVMsInfo.Remove(temp);
                        PutToBooted.Add(temp);
                    }
                }


                int frozenStateCapacity = costmatrix.GetVMLimit(1);
                for (int i = 1; i <= frozenStateCapacity; i++)
                {
                    if (AllIdleVMsInfo.Count > 0)
                    {
                        VMInfoBase temp = AllIdleVMsInfo.First();
                        AllIdleVMsInfo.Remove(temp);
                        PutToFrozen.Add(temp);
                    }
                }

                foreach (VMInfoBase b in AllIdleVMsInfo)
                    PutToShutdown.Add(b);

                Dictionary<int, List<VMInfoBase>> retVal = new Dictionary<int, List<VMInfoBase>>();
                retVal.Add(0, PutToBooted);
                retVal.Add(1, PutToFrozen);
                retVal.Add(2, PutToShutdown);
                return retVal;
            }

            else
            {
                VMInfoBase v = ((ReconfigureForDeparture)e).vmbase;
                List<VMInfoBase> l = new List<VMInfoBase>();
                l.Add(v);
                PopulateNextUseTs(l, queue);


                if (currentConfig[1].Count==0 || v.nextUseTs < currentConfig[1].First().nextUseTs)
                    return currentConfig;//booted

                else if(v.nextUseTs < currentConfig[1].Last().nextUseTs)
                {
                    //add to frozen and boot one from frozen
                    VMInfoBase temp = currentConfig[1].First();
                    currentConfig[1].Remove(temp);
                    currentConfig[0].Add(temp);
                    currentConfig[0].Remove(v);
                    currentConfig[1].Add(v);
                }

                else if(currentConfig[2].Count!=0)
                {
                    // add to shutdown and move one from shutdown to frozen and one from frozen to booted
                    VMInfoBase temp1 = currentConfig[1].First();
                    VMInfoBase temp2 = currentConfig[2].First();
                    currentConfig[1].Remove(temp1);
                    currentConfig[0].Add(temp1);
                    currentConfig[2].Remove(temp2);
                    currentConfig[1].Add(temp2);
                    currentConfig[0].Remove(v);
                    currentConfig[2].Add(v);
                }

                currentConfig[1].Sort(delegate(VMInfoBase a, VMInfoBase b) { return a.nextUseTs.CompareTo(b.nextUseTs); });
                currentConfig[2].Sort(delegate(VMInfoBase a, VMInfoBase b) { return a.nextUseTs.CompareTo(b.nextUseTs); });
                return currentConfig;
            }

        }


        private void InferNextUseTS_Naive(List<VMInfoBase> VMs)
        {
            foreach (VMInfoBase v in VMs)
            {
                if (v.ArrivalTimestamp.Count > 2)
                {
                    v.nextUseTs=v.ArrivalTimestamp.Last() + (v.ArrivalTimestamp[v.ArrivalTimestamp.Count - 1] - v.ArrivalTimestamp[v.ArrivalTimestamp.Count - 2]);
                }
                else
                    v.nextUseTs=long.MaxValue;
            }
        }

        private void InferNextUseTS_ARMA(List<VMInfoBase> VMs)
        {
            foreach(VMInfoBase v in VMs)
            {
                List<double> interarrivals = new List<double>();
                for (int i = 1; i <v.ArrivalTimestamp.Count;i++ )
                {
                    interarrivals.Add(v.ArrivalTimestamp[i] - v.ArrivalTimestamp[i - 1]);
                }
                v.nextUseTs = DoARMA(interarrivals);
            }
        }

        void PopulateNextUseTs(List<VMInfoBase> VMs, EventQueue queue)
        {
            if (NextUse.Equals(NextUseMode.GroundTruth))
                PopulateNextUseFromQueue(VMs, queue);
            else if (NextUse.Equals(NextUseMode.Infer))
                InferNextUseTS_Naive(VMs);
            else if(NextUse.Equals(NextUseMode.ARMA))
                InferNextUseTS_ARMA(VMs);

        }

        void PopulateNextUseFromQueue(List<VMInfoBase> VMs, EventQueue queue)
        {

            List<VMId> todo = new List<VMId>();
            Dictionary<VMId, VMInfoBase> lookuptable = new Dictionary<VMId, VMInfoBase>();

            foreach (VMInfoBase v in VMs)
            {
                todo.Add(v.vmid);
                lookuptable.Add(v.vmid, v);
            }

            int i = 0;
            int count = queue.Count();

            foreach (EventBase e in queue.GetHeap())
            {
                if (e is RequestReceivedEvent)
                {
                    RequestReceivedEvent rqe = (RequestReceivedEvent)e;
                    if (lookuptable.ContainsKey(rqe.vmid))
                    {
                        lookuptable[rqe.vmid].nextUseTs = e.Timestamp;
                        todo.Remove(rqe.vmid);
                    }
                }

                if (todo.Count == 0) break;
            }

        }

        public static long DoARMA(List<double> values)
        {
            if (values.Count < 6)
                return long.MaxValue;
            // This QuickStart Sample fits an ARMA(2,1) model and
            // an ARIMA(0,1,1) model to sunspot data.
            // The time series data is stored in a numerical variable:
            NumericalVariable sunspots = new NumericalVariable("sunspots", values.ToArray());
            // ARMA models (no differencing) are constructed from
            // the variable containing the time series data, and the
            // AR and MA orders. The following constructs an ARMA(2,1)
            // model:
            ArimaModel model = new ArimaModel(sunspots, Model_P, Model_Q);
            // The Compute methods fits the model.
            model.Compute();
            
            /*
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
            */
            // The Forecast method can be used to predict the next value in the series...
            double nextValue = model.Forecast();
            return (long)nextValue;
            //Console.WriteLine("One step ahead forecast: {0:F3}", nextValue);

            // or to predict a specified number of values:
            //Vector nextValues = model.Forecast(5);
            //Console.WriteLine("First five forecasts: {0:F3}", nextValues);
        }


    }
}
