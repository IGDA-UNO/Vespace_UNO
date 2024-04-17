using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ensemble
{
    public class Binding : Dictionary<string, string>
    {
        public int? _Weight { get; set; }

        public Binding DeepCopy()
        {
            //Debug.WriteLine("binding DeepCopy before: " + this.ToString());
            Binding deepCopy = (Binding)this.MemberwiseClone();

            //foreach(KeyValuePair<string, string> entry in deepCopy)
            //{
            //    deepCopy.Add(entry.Key, String.Copy(entry.Value));
            //}

            //Debug.WriteLine("binding DeepCopy after: " + deepCopy.ToString());

            return deepCopy;
        }

        public override string ToString()
        {
            string value = "";

            foreach (KeyValuePair<string, string> kvp in this)
            {
                value += string.Format("{0}: {1}, ", kvp.Key, kvp.Value);
            }

            if (this._Weight != null)
            {
                value += string.Format("_Weight: {0}, ", this._Weight);
            }

            return value;
        }
    }
}
