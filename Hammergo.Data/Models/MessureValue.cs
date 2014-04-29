using System;
using System.Collections.Generic;

namespace Hammergo.Data
{
    public partial class MessureValue
    {
        public System.Guid Id { get; set; }
        public System.Guid ParamId { get; set; }
        public System.DateTimeOffset Date { get; set; }
        public Nullable<double> Val { get; set; }
        public virtual MessureParam MessureParam { get; set; }
    }
}
