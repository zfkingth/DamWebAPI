using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EF5x.Models.Mapping
{
    public class CalculateParamMap : EntityTypeConfiguration<CalculateParam>
    {
        public CalculateParamMap()
        {
            // Primary Key
            this.HasKey(t => t.CalculateParamID);

            // Properties
            this.Property(t => t.appName)
                .HasMaxLength(20);

            this.Property(t => t.ParamName)
                .HasMaxLength(20);

            this.Property(t => t.ParamSymbol)
                .HasMaxLength(10);

            this.Property(t => t.UnitSymbol)
                .HasMaxLength(10);

            this.Property(t => t.CalculateExpress)
                .HasMaxLength(100);

            this.Property(t => t.Description)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("CalculateParam");
            this.Property(t => t.CalculateParamID).HasColumnName("CalculateParamID");
            this.Property(t => t.appName).HasColumnName("appName");
            this.Property(t => t.ParamName).HasColumnName("ParamName");
            this.Property(t => t.ParamSymbol).HasColumnName("ParamSymbol");
            this.Property(t => t.UnitSymbol).HasColumnName("UnitSymbol");
            this.Property(t => t.PrecisionNum).HasColumnName("PrecisionNum");
            this.Property(t => t.Order).HasColumnName("Order");
            this.Property(t => t.CalculateExpress).HasColumnName("CalculateExpress");
            this.Property(t => t.CalculateOrder).HasColumnName("CalculateOrder");
            this.Property(t => t.Description).HasColumnName("Description");

            // Relationships
            this.HasOptional(t => t.Apparatus)
                .WithMany(t => t.CalculateParams)
                .HasForeignKey(d => d.appName);

        }
    }
}
