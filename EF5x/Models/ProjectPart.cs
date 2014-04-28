using System;
using System.Collections.Generic;

namespace EF5x.Models
{
    public partial class ProjectPart
    {
        public ProjectPart()
        {
            this.Apparatus = new List<Apparatus>();
        }

        public System.Guid ProjectPartID { get; set; }
        public string PartName { get; set; }
        public Nullable<System.Guid> ParentPart { get; set; }
        public virtual ICollection<Apparatus> Apparatus { get; set; }
    }
}
