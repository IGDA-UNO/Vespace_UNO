using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ensemble
{
    public class Phrase
    {
        public string Text { get; set; }
        public string Label { get; set; }
        public string Meta { get; set; }
        public List<Phrase> Diagram { get; set; }
    }
}
