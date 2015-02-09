using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSimulator
{

    public class LXCCostMatrix : ICostMatrix
    {

        const long ms = 1000;

        int[] VMStates = { 0, 1 , 2};
        long[][] TransitionCostMatrix = { new long[] { 0, (long)(0.17* ms), (long)(641.64 * ms) }, new long[] { (long)(4.43 * ms), 0, (long)(641.64* ms) }, new long[] { (long)(802.16 * ms), (long)(802.33* ms), 0 } };
        int[] VMLimits = { 250, 300 , 2000 };

        public int[] GetVMStates()
        {
            return this.VMStates;
        }


        public long GetTransitionCost(int fromstate, int tostate, Dictionary<int, List<VMInfoBase>> currentconfig)
        {
            return TransitionCostMatrix[fromstate][tostate];
        }

        public int GetVMLimit(int state)
        {
            return VMLimits[state];
        }
    }
}
