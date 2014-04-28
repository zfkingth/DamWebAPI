using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EF5x.Models.Mapping
{
    public class ConstantParamMap : EntityTypeConfiguration<ConstantParam>
    {
        public ConstantParamMap()
        {
            // Primary Key
            this.HasKey(t => t.ConstantParamID);

            // Properties
            this.Property(t => t.appName)
                .HasMaxLength(20);

            this.Property(t => t.ParamName)
                .HasMaxLength(20);

            this.Property(t => t.ParamSymbol)
                .HasMaxLength(10);

            this.Property(t => t.UnitSymbol)
                .HasMaxLength(10);

            this.Property(t => t.Description)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ConstantParam");
            this.Property(t => t.ConstantParamID).HasColumnName("ConstantParamID");
            this.Property(t => t.appName).HasColumnName("appName");
            this.Property(t => t.ParamName).HasColumnName("ParamName");
            this.Property(t => t.ParamSymbol).HasColumnName("ParamSymbol");
            this.Property(t => t.UnitSymbol).HasColumnName("UnitSymbol");
            this.Property(t => t.PrecisionNum).HasColumnName("PrecisionNum");
            this.Property(t => t.Order).HasColumnName("Order");
            this.Property(t => t.Val).HasColumnName("Val");
            this.Property(t => t.Description).HasColumnName("Description");

            // Relationships
            this.HasOptional(t => t.Apparatus)
                .WithMany(t => t.ConstantParams)
                .HasForeignKey(d => d.appName);

        }
    }
}
