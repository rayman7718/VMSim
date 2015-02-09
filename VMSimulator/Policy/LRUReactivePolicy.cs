using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSimulator
{
    class LRUReactivePolicy : IPolicy
    {
        public Dictionary<int, List<VMInfoBase>> GetNewConfig(EventQueue queue, ReconfigureEvent e, Dictionary<int, List<VMInfoBase>> currentConfig, Dictionary<VMId, VMInfoBase> vminfos, ICostMatrix costmatrix)
        {

            if (e is ReconfigureForDeparture)
                return currentConfig;

            int topstate = costmatrix.GetVMStates().First();
            //foreach (VMInfoBase v in vminfos.Values)
            {
                VMInfoBase v = ((ReconfigureForArrival)e).vmbase;
                int currentstate = VMSimulator.GetStateOfVM(v,currentConfig);
                if (currentstate != topstate && !v.isIdle)
                {
                    bool val = currentConfig[currentstate].Remove(v);//transition them to top
                    if (!val)
                    {

                    }
                    currentConfig[topstate].Add(v);
                }
            }

            for ( int i=0; i < costmatrix.GetVMStates().Count(); i++)
            {
                int state = costmatrix.GetVMStates()[i];

                if (currentConfig[state].Count > costmatrix.GetVMLimit(state))
                {
                    int n = currentConfig[state].Count - costmatrix.GetVMLimit(state);
                    List<VMInfoBase> toevict = GetLeastRecentlyUsed(currentConfig[state], n);

                    foreach(VMInfoBase v in toevict)
                    {
                        bool val = currentConfig[state].Remove(v);
                        if(!val)
                        {

                        }
                        currentConfig[costmatrix.GetVMStates()[i+1]].Add(v);
                    }
                }
            }

            return currentConfig;
        }


        private List<VMInfoBase> GetLeastRecentlyUsed(List<VMInfoBase> VMList, int n)
        {
            List<VMInfoBase> retVal = new List<VMInfoBase>();


            VMList.Sort(
                delegate(VMInfoBase a, VMInfoBase b)
                {
                    if (b.ArrivalTimestamp.Count == 0 && a.ArrivalTimestamp.Count > 0)
                    {
                        return 1;
                    }

                    if (a.ArrivalTimestamp.Count == 0 && b.ArrivalTimestamp.Count == 0)
                    {
                        return 0;
                    }

                    if (a.ArrivalTimestamp.Count == 0 && b.ArrivalTimestamp.Count > 0)
                    {
                        return -1;
                    }

                    return a.ArrivalTimestamp.Last().CompareTo(b.ArrivalTimestamp.Last());
                }
                );


            for (int i = 1; i <= VMList.Count; i++)
            {
                if(VMList.ElementAt(i-1).isIdle)
                    retVal.Add(VMList.ElementAt(i - 1));

                if (retVal.Count == n)
                    break;
            }

            if (retVal.Count != n)
                throw new Exception("Eviction failed! ");

            return retVal;
        }

    }
}
