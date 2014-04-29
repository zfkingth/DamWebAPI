using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hammergo.Data
{
    public class DamContextInitializer : DropCreateDatabaseAlways<DamWCFContext>
    {
        protected override void Seed(DamWCFContext context)
        {
            var type = new ApparatusType();
            type.Id = Guid.NewGuid();
            type.TypeName = "第一种类型";

            context.ApparatusTypes.Add(type);
        }
    }
}
