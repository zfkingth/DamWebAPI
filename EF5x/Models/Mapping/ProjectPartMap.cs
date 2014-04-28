using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EF5x.Models.Mapping
{
    public class ProjectPartMap : EntityTypeConfiguration<ProjectPart>
    {
        public ProjectPartMap()
        {
            // Primary Key
            this.HasKey(t => t.ProjectPartID);

            // Properties
            this.Property(t => t.PartName)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ProjectPart");
            this.Property(t => t.ProjectPartID).HasColumnName("ProjectPartID");
            this.Property(t => t.PartName).HasColumnName("PartName");
            this.Property(t => t.ParentPart).HasColumnName("ParentPart");
        }
    }
}
