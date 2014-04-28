using System;
using System.Collections.Generic;

namespace EF5x.Models
{
    public partial class Role
    {
        public Role()
        {
            this.SysUsers = new List<SysUser>();
        }

        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public Nullable<byte> Power { get; set; }
        public virtual ICollection<SysUser> SysUsers { get; set; }
    }
}
