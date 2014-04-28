using System;
using System.Collections.Generic;

namespace EF5x.Models
{
    public partial class MessureValue
    {
        public System.Guid messureParamID { get; set; }
        public System.DateTime Date { get; set; }
        public Nullable<double> Val { get; set; }
        public virtual MessureParam MessureParam { get; set; }
    }
}
