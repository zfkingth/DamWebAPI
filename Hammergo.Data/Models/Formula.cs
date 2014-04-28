using System;
using System.Collections.Generic;

namespace Hammergo.Data
{
    public partial class Formula
    {
        public System.Guid FormulaID { get; set; }
        public System.Guid ParamId { get; set; }
        public System.DateTimeOffset StartDate { get; set; }
        public System.DateTimeOffset EndDate { get; set; }
        public string FormulaExpression { get; set; }
        //该数据会自动计算，用户对该值的写入是无效的
        public byte CalculateOrder { get; set; }
        public virtual CalculateParam CalculateParam { get; set; }
    }
}
