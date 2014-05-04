using DamServiceV3.Test.DamServiceRef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamServiceV3.Test.DTO
{
    public class ParamsDTO
    {
        //app id
        public Guid Id { get; set; }

        public List<AppParam> AddedParams { get; set; }
        public List<AppParam> UpdatedParams { get; set; }
        public List<AppParam> DeletedParams { get; set; }

        public List<Formula> AddedFormulae { get; set; }
        public List<Formula> UpdatedFormulae { get; set; }
        public List<Formula> DeletedFormulae { get; set; }
    }
}
