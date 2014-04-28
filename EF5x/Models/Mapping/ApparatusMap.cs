using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EF5x.Models.Mapping
{
    public class ApparatusMap : EntityTypeConfiguration<Apparatus>
    {
        public ApparatusMap()
        {
            // Primary Key
            this.HasKey(t => t.AppName);

            // Properties
            this.Property(t => t.AppName)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.CalculateName)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.X)
                .HasMaxLength(50);

            this.Property(t => t.Y)
                .HasMaxLength(50);

            this.Property(t => t.Z)
                .HasMaxLength(50);

            this.Property(t => t.OtherInfo)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("Apparatus");
            this.Property(t => t.AppName).HasColumnName("AppName");
            this.Property(t => t.CalculateName).HasColumnName("CalculateName");
            this.Property(t => t.ProjectPartID).HasColumnName("ProjectPartID");
            this.Property(t => t.AppTypeID).HasColumnName("AppTypeID");
            this.Property(t => t.X).HasColumnName("X");
            this.Property(t => t.Y).HasColumnName("Y");
            this.Property(t => t.Z).HasColumnName("Z");
            this.Property(t => t.BuriedTime).HasColumnName("BuriedTime");
            this.Property(t => t.OtherInfo).HasColumnName("OtherInfo");

            // Relationships
            this.HasOptional(t => t.ApparatusType)
                .WithMany(t => t.Apparatus)
                .HasForeignKey(d => d.AppTypeID);
            this.HasOptional(t => t.ProjectPart)
                .WithMany(t => t.Apparatus)
                .HasForeignKey(d => d.ProjectPartID);

        }
    }
}
