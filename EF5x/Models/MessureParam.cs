using System;
using System.Collections.Generic;

namespace EF5x.Models
{
    public partial class MessureParam
    {
        public MessureParam()
        {
            this.MessureValues = new List<MessureValue>();
        }

        public System.Guid MessureParamID { get; set; }
        public string appName { get; set; }
        public string ParamName { get; set; }
        public string ParamSymbol { get; set; }
        public string UnitSymbol { get; set; }
        public Nullable<byte> PrecisionNum { get; set; }
        public Nullable<byte> Order { get; set; }
        public string Description { get; set; }
        public virtual Apparatus Apparatus { get; set; }
        public virtual ICollection<MessureValue> MessureValues { get; set; }
    }
}
