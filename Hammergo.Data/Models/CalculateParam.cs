using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hammergo.Data
{
    public class CalculateParam:AppParam
    {
        public CalculateParam()
        {
            this.CalculateValues = new List<CalculateValue>();
            this.Formulae = new List<Formula>();
        }
        public virtual ICollection<CalculateValue> CalculateValues { get; set; }
        public virtual ICollection<Formula> Formulae { get; set; }
    }
}