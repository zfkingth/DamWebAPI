using System;
using System.Collections.Generic;

namespace Hammergo.Data
{
    public partial class TaskType
    {
        public TaskType()
        {
            this.AppCollections = new List<AppCollection>();
        }

        public int TaskTypeID { get; set; }
        public string TypeName { get; set; }
        public virtual ICollection<AppCollection> AppCollections { get; set; }
    }
}
