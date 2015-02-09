using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSimulator
{
    public class VMId
    {
        private int id ;

        public VMId(int id)
        {
            this.id = id;
        }

        public override bool Equals(object obj)
        {
            if (obj is VMId)
            {
                VMId chid = obj as VMId;
                return this.Equals(chid);
            }
            else
                return false;
        }

        public virtual bool Equals(VMId obj)
        {
            if (this.id.Equals(obj.id))
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public override string ToString()
        {
            return "VM:"+id;
        }


    }
}
