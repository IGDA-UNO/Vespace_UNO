using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ensemble
{
    public class Cast : List<string>
    {
        public Cast DeepCopy()
        {
            Cast copy = new Cast();
            foreach (string character in this)
            {
                copy.Add(character);
            }
            return copy;
        }

        public override string ToString()
        {
            string helper = "";
            for(int i = 0; i < base.Count; i++){
                helper += base[i] + "\n";
            }
            return helper;
        }
    }
}
