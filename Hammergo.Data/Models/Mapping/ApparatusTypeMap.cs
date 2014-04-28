using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Hammergo.Data.Mapping
{
    public class ApparatusTypeMap : EntityTypeConfiguration<ApparatusType>
    {
        public ApparatusTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.ApparatusTypeID);

            // Properties
            this.Property(t => t.TypeName)
                .IsRequired()
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("ApparatusType");
            this.Property(t => t.ApparatusTypeID).HasColumnName("ApparatusTypeID");
            this.Property(t => t.TypeName).HasColumnName("TypeName");
        }
    }
}
