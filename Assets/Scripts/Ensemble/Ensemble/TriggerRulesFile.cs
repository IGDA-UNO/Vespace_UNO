using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ensemble
{
    internal class TriggerRulesFile
    {
        public string fileName;
        public string type;
        public List<Rule> rules;
        public List<Rule> inactiveRules;
    }
}
