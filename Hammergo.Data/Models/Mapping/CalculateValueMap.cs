using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Hammergo.Data.Mapping
{
    public class CalculateValueMap : EntityTypeConfiguration<CalculateValue>
    {
        public CalculateValueMap()
        {
            // Primary Key
            this.HasKey(t => new {t.ID });

            // Properties
            // Table & Column Mappings
            this.ToTable("CalculateValue");
            this.Property(t => t.ParamId).HasColumnName("ParamId");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.Val).HasColumnName("Val");

            // Relationships
            this.HasRequired(t => t.CalculateParam)
                .WithMany(t => t.CalculateValues)
                .HasForeignKey(d => d.ParamId);

        }
    }
}
