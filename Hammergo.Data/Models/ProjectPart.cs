using System;
using System.Collections.Generic;

namespace Hammergo.Data
{
    public partial class ProjectPart
    {
        public ProjectPart()
        {
            this.Apps = new List<App>();
        }

        public System.Guid ProjectPartID { get; set; }
        public string PartName { get; set; }
        public Nullable<System.Guid> ParentPart { get; set; }
        public virtual ICollection<App> Apps { get; set; }
    }
}
