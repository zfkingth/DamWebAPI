using System;
using System.Collections.Generic;

namespace EF5x.Models
{
    public partial class SysUser
    {
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public Nullable<int> roleID { get; set; }
        public virtual Role Role { get; set; }
    }
}
