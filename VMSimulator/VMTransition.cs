using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSimulator
{
    public class VMTransition
    {
        public VMId vmid { get; private set;  }
        public int fromstate { get; private set; }
        public int tostate { get; private set; }


        public VMTransition(VMId vmid, int fromstate, int tostate)
        {
            this.vmid = vmid;
            this.fromstate = fromstate;
            this.tostate = tostate;
        }

    }
}
