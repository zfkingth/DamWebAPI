using Hammergo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DamServiceV3.Models
{
    public class ParamsDTO
    {
        //app id
        public Guid Id { get; set; }

        //public List<AppParam> AddedParams { get; set; }
        //public List<AppParam> UpdatedParams { get; set; }
        //public List<AppParam> DeletedParams { get; set; }

        public List<Formula> AddedFormulae { get; set; }
        public List<Formula> UpdatedFormulae { get; set; }
        public List<Formula> DeletedFormulae { get; set; }
    }
}