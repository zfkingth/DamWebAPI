using System;
using System.Collections.Generic;

namespace Hammergo.Data
{
    public partial class Remark
    {
        public Guid Id { get; set; }
        public Guid AppId { get; set; }
        public System.DateTimeOffset Date { get; set; }
        public string RemarkText { get; set; }
        public virtual App App { get; set; }
    }
}
