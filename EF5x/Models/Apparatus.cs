using System;
using System.Collections.Generic;

namespace EF5x.Models
{
    public partial class Apparatus
    {
        public Apparatus()
        {
            this.CalculateParams = new List<CalculateParam>();
            this.ConstantParams = new List<ConstantParam>();
            this.MessureParams = new List<MessureParam>();
            this.Remarks = new List<Remark>();
            this.TaskAppratus = new List<TaskAppratu>();
        }

        public string AppName { get; set; }
        public string CalculateName { get; set; }
        public Nullable<System.Guid> ProjectPartID { get; set; }
        public Nullable<System.Guid> AppTypeID { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
        public string Z { get; set; }
        public Nullable<System.DateTime> BuriedTime { get; set; }
        public string OtherInfo { get; set; }
        public virtual ApparatusType ApparatusType { get; set; }
        public virtual ProjectPart ProjectPart { get; set; }
        public virtual ICollection<CalculateParam> CalculateParams { get; set; }
        public virtual ICollection<ConstantParam> ConstantParams { get; set; }
        public virtual ICollection<MessureParam> MessureParams { get; set; }
        public virtual ICollection<Remark> Remarks { get; set; }
        public virtual ICollection<TaskAppratu> TaskAppratus { get; set; }
    }
}
