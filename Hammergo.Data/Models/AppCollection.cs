using System;
using System.Collections.Generic;

namespace Hammergo.Data
{
    public partial class AppCollection
    {
        public AppCollection()
        {
            this.TaskApps = new List<TaskApp>();
        }

        public System.Guid AppCollectionID { get; set; }
        public int TaskTypeID { get; set; }
        public string CollectionName { get; set; }
        public string Description { get; set; }
        public Nullable<int> Order { get; set; }
        public Nullable<System.Guid> ParentCollection { get; set; }
        public Nullable<double> SUM { get; set; }
        public Nullable<double> MAX { get; set; }
        public Nullable<double> MIN { get; set; }
        public Nullable<double> AVG { get; set; }
        public Nullable<int> CNT { get; set; }
        public virtual TaskType TaskType { get; set; }
        public virtual ICollection<TaskApp> TaskApps { get; set; }
    }
}
