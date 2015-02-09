using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSimulator
{
    public interface IPolicy
    {
        Dictionary<int, List<VMInfoBase>> GetNewConfig(EventQueue queue, ReconfigureEvent e, Dictionary<int, List<VMInfoBase>> currentConfig, Dictionary<VMId, VMInfoBase> vminfos, ICostMatrix costmatrix);
    }
}
