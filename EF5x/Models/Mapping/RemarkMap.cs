using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EF5x.Models.Mapping
{
    public class RemarkMap : EntityTypeConfiguration<Remark>
    {
        public RemarkMap()
        {
            // Primary Key
            this.HasKey(t => new { t.appName, t.Date });

            // Properties
            this.Property(t => t.appName)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.RemarkText)
                .HasMaxLength(80);

            // Table & Column Mappings
            this.ToTable("Remark");
            this.Property(t => t.appName).HasColumnName("appName");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.RemarkText).HasColumnName("RemarkText");

            // Relationships
            this.HasRequired(t => t.Apparatus)
                .WithMany(t => t.Remarks)
                .HasForeignKey(d => d.appName);

        }
    }
}
