using System;
using System.Collections.Generic;

namespace Hammergo.Data
{
    public partial class CalculateValue
    {
        public System.Guid ID { get; set; }
        public System.Guid ParamId { get; set; }
        public System.DateTimeOffset Date { get; set; }
        public Nullable<double> Val { get; set; }
        public virtual CalculateParam CalculateParam { get; set; }
    }
}
