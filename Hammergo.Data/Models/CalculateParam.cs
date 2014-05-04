using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Hammergo.Data
{
    public class CalculateParam:AppParam
    {
        public CalculateParam()
        {
            this.CalculateValues = new List<CalculateValue>();
            this.Formulae = new List<Formula>();
        }

 
       
        public byte CalcOrder { get; set; }

        public virtual ICollection<CalculateValue> CalculateValues { get; set; }
        public virtual ICollection<Formula> Formulae { get; set; }
    }
}