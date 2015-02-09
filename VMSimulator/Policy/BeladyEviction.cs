using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSimulator
{

    public class BeladyEviction : IPolicy
    {
        public Dictionary<int, List<VMInfoBase>> GetNewConfig(EventQueue queue, ReconfigureEvent e, Dictionary<int, List<VMInfoBase>> currentConfig, Dictionary<VMId, VMInfoBase> vminfos, ICostMatrix costmatrix)
        {

            if (e is ReconfigureForDeparture)
                return currentConfig;

            int topstate = costmatrix.GetVMStates().First();
            //foreach (VMInfoBase v in vminfos.Values)
            {
                VMInfoBase v = ((ReconfigureForArrival)e).vmbase;
                int currentstate = VMSimulator.GetStateOfVM(v, currentConfig);
                if (currentstate != topstate && !v.isIdle)
                {
                    bool val = currentConfig[currentstate].Remove(v);//transition them to top
                    if (!val)
                    {

                    }
                    currentConfig[topstate].Add(v);
                }
            }

            for (int i = 0; i < costmatrix.GetVMStates().Count(); i++)
            {
                int state = costmatrix.GetVMStates()[i];

                if (currentConfig[state].Count > costmatrix.GetVMLimit(state))
                {
                    int n = currentConfig[state].Count - costmatrix.GetVMLimit(state);

                    if(n>1)
                    {

                    }

                    //Stopwatch t = new Stopwatch(); t.Start();
                    List<VMInfoBase> toevict = GetVMsUsedFurthestInFuture(currentConfig[state], n, queue);
                   //t.Stop();
                    //Console.WriteLine("stopwatch=" + t.ElapsedMilliseconds);

                    foreach (VMInfoBase v in toevict)
                    {
                        bool val = currentConfig[state].Remove(v);
                        if (!val)
                        {

                        }
                        currentConfig[costmatrix.GetVMStates()[i + 1]].Add(v);
                    }
                }
            }

            return currentConfig;
        }


        private List<VMInfoBase> GetVMsUsedFurthestInFuture(List<VMInfoBase> VMList, int n, EventQueue queue)
        {
            List<VMInfoBase> retVal = new List<VMInfoBase>();
            List<VMInfoBase> VMs = new List<VMInfoBase>();
            foreach (VMInfoBase v in VMList)
            {
                VMs.Add(v);
            }
            
            PopulateNextUseTs(VMs, queue);


            VMs.Sort(delegate(VMInfoBase a, VMInfoBase b) 
            { 
                return a.nextUseTs.CompareTo(b.nextUseTs); 
            });

            
            for (int i = VMs.Count - 1; i >= 0; i--)
            {
                if (VMs.ElementAt(i).isIdle)
                    retVal.Add(VMs.ElementAt(i));

                if (retVal.Count == n)
                    break;
            }

            if (retVal.Count != n)
                throw new Exception("Eviction failed! ");

            return retVal;
        }


        void PopulateNextUseTs(List<VMInfoBase> VMs, EventQueue queue)
        {
            
            List<VMId> todo = new List<VMId>();
            Dictionary<VMId, VMInfoBase> lookuptable = new Dictionary<VMId, VMInfoBase>();

            foreach (VMInfoBase v in VMs)
            {
                todo.Add(v.vmid);
                lookuptable.Add(v.vmid, v);
            }

            
            int count = queue.Count();

            for(int i=0; i < queue.GetHeap().Count; i++) 
            {
                EventBase e = queue.GetHeap().ElementAt(i);
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
            /*
            while(todo.Count>0 && i<count)
            {
                EventBase e = queue.ElementAt(i);
                i++;
                if (e is RequestReceivedEvent)
                {
                    RequestReceivedEvent rqe = (RequestReceivedEvent)e;
                    if(lookuptable.ContainsKey(rqe.vmid))
                    {
                        lookuptable[rqe.vmid].nextUseTs = e.Timestamp;
                        todo.Remove(rqe.vmid);
                    }
                }

            }*/
            
        }


        


    }
}
/*
  if(n==1)
            {
                CustomVMInfoBase max = null;
                foreach(CustomVMInfoBase v in VMs)
                {
                    if(v.vminfobase.isIdle && max==null)
                        max = v;
                    if (v.vminfobase.isIdle && max.nextUseTs > v.nextUseTs)
                        max = v;
                }

                retVal.Add(max.vminfobase);
                return retVal;
            }
*/