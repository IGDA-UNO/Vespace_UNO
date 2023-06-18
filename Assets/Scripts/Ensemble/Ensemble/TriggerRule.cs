using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ensemble
{
    public class TriggerRule : Rule
    {
        public List<string> InCharMsgs { get; set; }
        public List<string> Explanations { get; set; }
    }
}
