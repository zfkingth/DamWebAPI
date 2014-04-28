using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EF5x.Models.Mapping
{
    public class CalculateValueMap : EntityTypeConfiguration<CalculateValue>
    {
        public CalculateValueMap()
        {
            // Primary Key
            this.HasKey(t => new { t.calculateParamID, t.Date });

            // Properties
            // Table & Column Mappings
            this.ToTable("CalculateValue");
            this.Property(t => t.calculateParamID).HasColumnName("calculateParamID");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.Val).HasColumnName("Val");

            // Relationships
            this.HasRequired(t => t.CalculateParam)
                .WithMany(t => t.CalculateValues)
                .HasForeignKey(d => d.calculateParamID);

        }
    }
}
