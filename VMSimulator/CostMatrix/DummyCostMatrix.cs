using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSimulator
{
    public class DummyCostMatrix : ICostMatrix
    {
        int[] VMStates = { 0, 1, 2 };
        long[][] TransitionCostMatrix = { new long[] { 0, 1000000, 1000000 }, new long[] { 1000000, 0, 1000000 } , new long[] {1000000,1000000,0} };
        int[] VMLimits = { 2, 1, 3 };

        //int[] VMStates = { 0, 1, 2 };
        //long[][] TransitionCostMatrix = { new long[] { 0, 1000000, 2000000 }, new long[] { 1000000, 0, 1000000 }, new long[] { 2000000, 1000000, 0 } };
        //int[] VMLimits = { 2, 2, 3 };
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
