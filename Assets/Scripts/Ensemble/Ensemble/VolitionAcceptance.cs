using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ensemble
{
    public class VolitionAcceptance
    {
        public bool Accepted { get; set; }

        public int? Weight { get; set; }
        
        public List<Predicate> ReasonsWhy { get; set; }
    }
}
