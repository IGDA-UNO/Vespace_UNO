using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ensemble
{
    public class Intent
    {
        public string Category { get; set; }
        public string Type { get; set; }
        public bool IntentType { get; set; }
        public string First { get; set; }
        public string Second { get; set;  }

        public Intent(string category, string type, bool intentType, string first, string second)
        {
            this.Category = category;
            this.Type = type;
            this.IntentType = intentType;
            this.First = first;
            this.Second = second;
        }

        public override string ToString()
        {
            String predToString = "";

            if (this.Category != null) { predToString += String.Format("Category: {0}", this.Category.ToString()); }
            if (this.Type != null) { predToString += String.Format(", Type: {0}", this.Type.ToString()); }
            if (this.IntentType != null) { predToString += String.Format(", IntentType: {0}", this.IntentType.ToString()); }
            if (this.First != null) { predToString += String.Format(", First: {0}", this.First); }
            if (this.Second != null) { predToString += String.Format(", Second: {0}", this.Second); }

            return predToString;
        }
    }
}
