using System;
using System.Collections.Generic;

namespace Hammergo.Data
{
    public partial class TaskApp
    {
        public Guid Id { get; set; }
        public System.Guid AppCollectionID { get; set; }
        public Guid AppId { get; set; }
        public Nullable<int> Order { get; set; }
        public virtual App App { get; set; }
        public virtual AppCollection AppCollection { get; set; }
    }
}
