using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSimulator
{
    public interface IWorkloadGen
    {
        List<RequestReceivedEvent> GetArrivals();
    }
}
