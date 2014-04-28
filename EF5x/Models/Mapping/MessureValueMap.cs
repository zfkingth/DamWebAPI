using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EF5x.Models.Mapping
{
    public class MessureValueMap : EntityTypeConfiguration<MessureValue>
    {
        public MessureValueMap()
        {
            // Primary Key
            this.HasKey(t => new { t.messureParamID, t.Date });

            // Properties
            // Table & Column Mappings
            this.ToTable("MessureValue");
            this.Property(t => t.messureParamID).HasColumnName("messureParamID");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.Val).HasColumnName("Val");

            // Relationships
            this.HasRequired(t => t.MessureParam)
                .WithMany(t => t.MessureValues)
                .HasForeignKey(d => d.messureParamID);

        }
    }
}
