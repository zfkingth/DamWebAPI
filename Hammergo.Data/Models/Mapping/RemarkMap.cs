using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Hammergo.Data.Mapping
{
    public class RemarkMap : EntityTypeConfiguration<Remark>
    {
        public RemarkMap()
        {
            // Primary Key
            this.HasKey(t => new {t.ID});

            // Properties


            this.Property(t => t.RemarkText)
                .HasMaxLength(80);

            // Table & Column Mappings
            this.ToTable("Remark");
            this.Property(t => t.AppId).HasColumnName("AppId");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.RemarkText).HasColumnName("RemarkText");

            // Relationships
            this.HasRequired(t => t.App)
                .WithMany(t => t.Remarks)
                .HasForeignKey(d => d.AppId);

        }
    }
}
