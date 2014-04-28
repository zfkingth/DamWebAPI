using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Linq;


namespace Hammergo.Data
{
    public partial class App
    {
        public App()
        {

            this.Remarks = new List<Remark>();
            this.TaskApps = new List<TaskApp>();
            this.AppParams = new List<AppParam>();
        }

        public Guid AppId { get; set; }


        [RegularExpression(@"^[^\s]+$", ErrorMessage = "测点编号不能包括空格、tab、回车等非可见字符")]
        public string AppName { get; set; }


        [RegularExpression(@"^[A-Za-z][A-Za-z0-9]*$", ErrorMessage = "测点的计算名称只能由字母和数字组成，而且必须以字母开头")]
        public string CalculateName { get; set; }
        public Nullable<System.Guid> ProjectPartID { get; set; }
        public Nullable<System.Guid> AppTypeID { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
        public string Z { get; set; }
        public Nullable<System.DateTimeOffset> BuriedTime { get; set; }
        public string OtherInfo { get; set; }
        public virtual ApparatusType ApparatusType { get; set; }
        public virtual ProjectPart ProjectPart { get; set; }
        public virtual ICollection<Remark> Remarks { get; set; }
        public virtual ICollection<TaskApp> TaskApps { get; set; }
        public virtual ICollection<AppParam> AppParams { get; set; }


    }
}
