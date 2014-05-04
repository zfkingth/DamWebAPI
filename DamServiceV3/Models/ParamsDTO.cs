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

        public ICollection<AppParam> AddedParams { get; set; }
        public ICollection<AppParam> UpdatedParams { get; set; }
        public ICollection<AppParam> DeletedParams { get; set; }

        public ICollection<Formula> AddedFormulae { get; set; }
        public ICollection<Formula> UpdatedFormulae { get; set; }
        public ICollection<Formula> DeletedFormulae { get; set; }

    }
}