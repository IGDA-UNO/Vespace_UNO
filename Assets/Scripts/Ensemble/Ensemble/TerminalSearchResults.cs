using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ensemble
{
    public class TerminalSearchResults
    {
        public bool TerminalsAtThisLevel { get; set; }
        public Action BoundTerminal { get; set; }
    }
}
