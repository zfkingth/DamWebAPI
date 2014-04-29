using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Hammergo.Data.Mapping
{
    public class ProjectPartMap : EntityTypeConfiguration<ProjectPart>
    {
        public ProjectPartMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.PartName)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ProjectPart");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.PartName).HasColumnName("PartName");
            this.Property(t => t.ParentPart).HasColumnName("ParentPart");
        }
    }
}
