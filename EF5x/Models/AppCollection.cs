using System;
using System.Collections.Generic;

namespace EF5x.Models
{
    public partial class AppCollection
    {
        public AppCollection()
        {
            this.TaskAppratus = new List<TaskAppratu>();
        }

        public System.Guid AppCollectionID { get; set; }
        public int taskTypeID { get; set; }
        public string CollectionName { get; set; }
        public string Description { get; set; }
        public Nullable<int> Order { get; set; }
        public virtual TaskType TaskType { get; set; }
        public virtual ICollection<TaskAppratu> TaskAppratus { get; set; }
    }
}
