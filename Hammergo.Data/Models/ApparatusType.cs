using System;
using System.Collections.Generic;

namespace Hammergo.Data
{
    public partial class ApparatusType
    {
        public ApparatusType()
        {
            this.Apps = new List<App>();
        }

        public System.Guid ApparatusTypeID { get; set; }
        public string TypeName { get; set; }
        public virtual ICollection<App> Apps { get; set; }
    }
}
