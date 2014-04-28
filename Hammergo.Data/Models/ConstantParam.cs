using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Hammergo.Data
{

    public class ConstantParam : AppParam
    {
        [Required]
        public double Val { get; set; }
    }
}