using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ensemble
{
    public class EnglishData
    {
        public string EnglishRule { get; set; }
        public string RuleName { get; set; }
        public int Weight { get; set; }
        public string Origin { get; set; }

        public EnglishData ShallowCopy()
        {
            return (EnglishData)this.MemberwiseClone();
        }

        public EnglishData DeepCopy()
        {
            EnglishData deepCopy = (EnglishData)this.MemberwiseClone();

            deepCopy.EnglishRule = String.Copy(EnglishRule);
            deepCopy.RuleName = String.Copy(RuleName);
            deepCopy.Origin = String.Copy(Origin);

            return deepCopy;
        }
    }
}
