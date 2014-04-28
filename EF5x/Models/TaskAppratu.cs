using System;
using System.Collections.Generic;

namespace EF5x.Models
{
    public partial class TaskAppratu
    {
        public System.Guid appCollectionID { get; set; }
        public string appName { get; set; }
        public Nullable<int> Order { get; set; }
        public virtual Apparatus Apparatus { get; set; }
        public virtual AppCollection AppCollection { get; set; }
    }
}
