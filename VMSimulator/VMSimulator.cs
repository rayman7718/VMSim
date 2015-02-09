using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSimulator
{
    public class VMSimulator
    {
        public const long IDLE_EVENT_THRESHOLD = 0;

        EventQueue eventqueue;
        Dictionary<int, List<VMInfoBase>> currentconfig;
        Dictionary<VMId, VMInfoBase> vminfos;
        IPolicy policy;
        ICostMatrix costmatrix;
        IWorkloadGen workloadgen;

        List<RequestReceivedEvent> outstandingrequests;


        Tuple<ReconfigureEvent, long> lastReconfEvent = null;

        public List<double> latency_penalties { get; private set; }
        public List<double> stateAtRequestArrival { get; private set; }

        public VMSimulator(IPolicy policy, ICostMatrix coststructure, IWorkloadGen workloadgen)
        {
            eventqueue = new EventQueue();
            currentconfig = new Dictionary<int, List<VMInfoBase>>();
            vminfos = new Dictionary<VMId, VMInfoBase>();
            outstandingrequests = new List<RequestReceivedEvent>();
            latency_penalties = new List<double>();
            this.policy = policy;
            this.costmatrix = coststructure;
            this.workloadgen = workloadgen;
            this.stateAtRequestArrival = new List<double>();
        }

        public void Initialize(int numberOfVMs)
        {
            int[] vmstates = costmatrix.GetVMStates();
            foreach (int s in vmstates)
            {
                currentconfig.Add(s, new List<VMInfoBase>());
            }

            for (int i = 1; i <= numberOfVMs; i++)
            {
                VMId v = new VMId(i);
                VMInfoBase b = new VMInfoBase(v, vmstates.Last());
                currentconfig[vmstates.Last()].Add(b);
                vminfos.Add(v, b);
            }



            List<RequestReceivedEvent> events = workloadgen.GetArrivals();
            eventqueue.Initialize(events);
            /*
            RequestReceivedEvent e11 = new RequestReceivedEvent(new VMId(1), 1, 1);
            RequestReceivedEvent e12 = new RequestReceivedEvent(new VMId(2), 10000, 100);
            RequestReceivedEvent e12 = new RequestReceivedEvent(new VMId(1), 3, 1);
            RequestReceivedEvent e13 = new RequestReceivedEvent(new VMId(1), 4, 1);

            RequestReceivedEvent e21 = new RequestReceivedEvent(new VMId(2), 2, 4);
             eventqueue.AddEvent(e12);
            eventqueue.AddEvent(e13);
            eventqueue.AddEvent(e21);
            eventqueue.AddEvent(e12);
            eventqueue.AddEvent(e11);*/

        }


        public void Run()
        {
            EventBase e;
            while ((e = eventqueue.PopEvent()) != null)
            {
                long currentts = Globals.GetCurrentTS();
                Globals.SetCurrentTS(Math.Max(currentts, e.Timestamp));

                long t = HandleEvent(e);
                //Log.debug(PrintConfig(currentconfig));

                long t_finish = e.Timestamp + t; // or could be Globals.Clock + t

                Globals.SetCurrentTS(Math.Max(t_finish, Globals.GetCurrentTS()));
                // Log.debug("event delay: " + (t_finish - e.Timestamp));

            }

        }




        private long HandleEvent(EventBase e)
        {
            long retVal = 0;

            if (e is RequestReceivedEvent)
            {
                RequestReceivedEvent rxe = (RequestReceivedEvent)e;
                SetIsIdleInConfig(rxe.vmid, false);
                AddArrivalTS(rxe.vmid, rxe.Timestamp);
                AddArrivalDuration(rxe.vmid, rxe.Duration);

                ReconfigureEvent r;
                if (lastReconfEvent != null && lastReconfEvent.Item1.Timestamp <= e.Timestamp && e.Timestamp <= lastReconfEvent.Item2)
                {
                    //r = new ReconfigureEvent(lastReconfEvent.Item2);
                    r = new ReconfigureForArrival(lastReconfEvent.Item2, vminfos[rxe.vmid]);
                }
                else
                {
                    //r = new ReconfigureEvent(e.Timestamp);
                    r = new ReconfigureForArrival(e.Timestamp, vminfos[rxe.vmid]);
                }
                eventqueue.AddEvent(r);
                outstandingrequests.Add(rxe);

                stateAtRequestArrival.Add(VMSimulator.GetStateOfVM(vminfos[rxe.vmid], currentconfig));
                retVal = 0;
                Log.debug("Requestreceived Event:"+rxe.vmid+"  arriving at " + rxe.Timestamp + " time-taken " + retVal);

            }
            else if (e is VMIdleEvent)// && ))
            {
                if (e.Timestamp.Equals(vminfos[((VMIdleEvent)e).vmid].IdleEventTimestamp))
                {
                    VMIdleEvent vmie = (VMIdleEvent)e;
                    SetIsIdleInConfig(((VMIdleEvent)e).vmid, true);
                    ReconfigureForDeparture r = new ReconfigureForDeparture(e.Timestamp, vminfos[vmie.vmid]);
                    eventqueue.AddEvent(r);
                    //TODO re-eval
                }
                else
                {

                }
            }
            else if (e is ReconfigureEvent)
            {

                ReconfigureEvent rce = (ReconfigureEvent)e;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                Dictionary<int, List<VMInfoBase>> newconfig = policy.GetNewConfig(this.eventqueue, rce, CopyConfig(currentconfig), vminfos, costmatrix);
                stopwatch.Stop();
                List<VMTransition> transitions = GetTransitions(currentconfig, newconfig);

                retVal = 0;// (long)(stopwatch.Elapsed.TotalMilliseconds * 1000);

                TransitionsEvent tre = new TransitionsEvent(e.Timestamp + retVal, transitions);
                lastReconfEvent = new Tuple<ReconfigureEvent, long>(rce, e.Timestamp + retVal);

                eventqueue.AddEvent(tre);
                Log.debug("Reconfigure Event arriving at " + rce.Timestamp + " time-taken " + retVal);
            }
            else if (e is TransitionsEvent)
            {
                TransitionsEvent txe = (TransitionsEvent)e;
                List<VMTransition> transitions = txe.transitions;
                retVal = PerformTransitions(transitions);

                List<RequestReceivedEvent> correspondingrequests = new List<RequestReceivedEvent>();
                foreach (RequestReceivedEvent r in outstandingrequests)
                {
                    foreach (VMInfoBase b in currentconfig[0])
                    {
                        if (b.vmid.Equals(r.vmid))
                        {
                            correspondingrequests.Add(r);
                        }
                    }
                }

                foreach (RequestReceivedEvent rxe in correspondingrequests)
                {
                    bool val = outstandingrequests.Remove(rxe);
                    if (!val)
                    {
                        Log.debug("Exception in removing outstanding request " + rxe.vmid + "," + rxe.Timestamp + "," + rxe.Duration);
                    }

                    Log.debug("Latency penalty for " + rxe.vmid + " " + (e.Timestamp + retVal - rxe.Timestamp));
                    latency_penalties.Add((e.Timestamp + retVal - rxe.Timestamp) / 1000.000);

                    if ((e.Timestamp + retVal - rxe.Timestamp) / 1000.000 > 2000)
                    {

                    }

                    long idleeventts = e.Timestamp + retVal + vminfos[rxe.vmid].ArrivalDurations.Last() + IDLE_EVENT_THRESHOLD;
                    vminfos[rxe.vmid].IdleEventTimestamp = idleeventts;
                    VMIdleEvent vmie = new VMIdleEvent(idleeventts, rxe.vmid);
                    eventqueue.AddEvent(vmie);
                    Log.debug("idle event for " + rxe.vmid + " at " + idleeventts);
                }
            }


            return retVal;
        }

        private List<VMTransition> GetTransitions(Dictionary<int, List<VMInfoBase>> currentconfig, Dictionary<int, List<VMInfoBase>> newconfig)
        {
            List<VMTransition> retVal = new List<VMTransition>();

            foreach (VMInfoBase v in vminfos.Values)
            {
                int currentstate = GetStateOfVM(v, currentconfig);
                int newstate = GetStateOfVM(v, newconfig);

                if (currentstate != newstate)
                {
                    VMTransition t = new VMTransition(v.vmid, currentstate, newstate);
                    retVal.Add(t);
                }
            }

            return retVal;
        }

        public static int GetStateOfVM(VMInfoBase v, Dictionary<int, List<VMInfoBase>> config)
        {
            foreach (int state in config.Keys)
            {
                if (config[state].Contains(v))
                    return state;
            }

            return -1;
        }


        private long PerformTransitions(List<VMTransition> transitions)
        {
            long retVal = 0;
            lock (currentconfig)
            {
                foreach (VMTransition t in transitions)
                {
                    long t_tr = costmatrix.GetTransitionCost(t.fromstate, t.tostate, currentconfig);
                    retVal = Math.Max(t_tr, retVal);

                    VMInfoBase vminfo = vminfos[t.vmid];
                    bool val = currentconfig[t.fromstate].Remove(vminfo);

                    if (val)
                    {
                        currentconfig[t.tostate].Add(vminfo);
                    }
                    else
                    {

                    }

                    if (currentconfig[0].Count > 250)
                    {

                    }



                }

            }

            return retVal;
        }

        private void SetIsIdleInConfig(VMId vmid, bool idleval)
        {
            lock (currentconfig)
            {
                lock (vminfos)
                {
                    vminfos[vmid].isIdle = idleval;
                }
            }
        }

        private void AddArrivalTS(VMId vmid, long ts)
        {
            lock (currentconfig)
            {
                lock (vminfos)
                {
                    vminfos[vmid].AddArrivalTS(ts);
                }
            }
        }

        private void AddArrivalDuration(VMId vmid, long d)
        {
            lock (currentconfig)
            {
                lock (vminfos)
                {
                    vminfos[vmid].AddArrivalDuration(d);
                }
            }
        }

        private Dictionary<int, List<VMInfoBase>> CopyConfig(Dictionary<int, List<VMInfoBase>> config)
        {
            Dictionary<int, List<VMInfoBase>> retVal = new Dictionary<int, List<VMInfoBase>>();
            foreach (int i in config.Keys)
            {
                retVal.Add(i, new List<VMInfoBase>());
                foreach (VMInfoBase b in config[i])
                {
                    retVal[i].Add(b);
                }
            }
            return retVal;
        }


        private string PrintConfig(Dictionary<int, List<VMInfoBase>> config)
        {
            string retVal = "";

            foreach (int state in config.Keys)
            {
                retVal += state + "->";
                foreach (VMInfoBase v in config[state])
                {
                    retVal += v.vmid + ",";
                }
                retVal += "\n";
            }
            return retVal;
        }

    }
}
