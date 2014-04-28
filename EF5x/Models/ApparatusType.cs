using System;
using System.Collections.Generic;

namespace EF5x.Models
{
    public partial class ApparatusType
    {
        public ApparatusType()
        {
            this.Apparatus = new List<Apparatus>();
        }

        public System.Guid ApparatusTypeID { get; set; }
        public string TypeName { get; set; }
        public virtual ICollection<Apparatus> Apparatus { get; set; }
    }
}
