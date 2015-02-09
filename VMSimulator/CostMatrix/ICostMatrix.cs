using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSimulator
{
    public interface ICostMatrix
    {
        int[] GetVMStates();

        long GetTransitionCost(int fromstate, int tostate, Dictionary<int, List<VMInfoBase>> currentconfig);

        int GetVMLimit(int state);//max number of VMs
        

    }
}
