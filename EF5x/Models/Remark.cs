using System;
using System.Collections.Generic;

namespace EF5x.Models
{
    public partial class Remark
    {
        public string appName { get; set; }
        public System.DateTime Date { get; set; }
        public string RemarkText { get; set; }
        public virtual Apparatus Apparatus { get; set; }
    }
}
