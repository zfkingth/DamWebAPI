using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Hammergo.Data
{
    public abstract class AppParam
    {
  
        [Key]
        public Guid ParamId { get; set; }

        public Guid AppId { get; set; }

        [Required]
        [MaxLength(20)]
        public string ParamName { get; set; }

        [Required]
        [MaxLength(10)]
        [RegularExpression(@"^[A-Za-z][A-Za-z0-9]{0,5}$", ErrorMessage = "参数符号只能由字母和数字组成，而且必须以字母开头,最大长度为6")]
        public string ParamSymbol { get; set; }

        [MaxLength(10)]
        public string UnitSymbol { get; set; }

        [Range(0, 10, ErrorMessage = "小数位数只支持从0到10")]
        public byte PrecisionNum { get; set; }

        [Required]
        public byte Order { get; set; }
        public string Description { get; set; }

        public virtual App App { get; set; }


    }


}