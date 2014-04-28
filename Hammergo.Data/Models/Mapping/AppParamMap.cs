using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Hammergo.Data.Mapping
{
    public class AppParamMap : EntityTypeConfiguration<AppParam>
    {
        public AppParamMap()
        {
           

            // Table & Column Mappings
            this.ToTable("AppParam");

            this.Map<ConstantParam>(param => { param.Requires("TypeNum").HasValue(0); });
            this.Map<MessureParam>(param => { param.Requires("TypeNum").HasValue(1); });
            this.Map<CalculateParam>(param => { param.Requires("TypeNum").HasValue(2); });
        }
    }
}
