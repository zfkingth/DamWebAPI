using System;
using System.Collections.Generic;

namespace EF5x.Models
{
    public partial class CalculateValue
    {
        public System.Guid calculateParamID { get; set; }
        public System.DateTime Date { get; set; }
        public Nullable<double> Val { get; set; }
        public virtual CalculateParam CalculateParam { get; set; }
    }
}
