using System;
using System.Collections.Generic;

namespace EF5x.Models
{
    public partial class CalculateParam
    {
        public CalculateParam()
        {
            this.CalculateValues = new List<CalculateValue>();
        }

        public System.Guid CalculateParamID { get; set; }
        public string appName { get; set; }
        public string ParamName { get; set; }
        public string ParamSymbol { get; set; }
        public string UnitSymbol { get; set; }
        public Nullable<byte> PrecisionNum { get; set; }
        public Nullable<byte> Order { get; set; }
        public string CalculateExpress { get; set; }
        public Nullable<byte> CalculateOrder { get; set; }
        public string Description { get; set; }
        public virtual Apparatus Apparatus { get; set; }
        public virtual ICollection<CalculateValue> CalculateValues { get; set; }
    }
}
