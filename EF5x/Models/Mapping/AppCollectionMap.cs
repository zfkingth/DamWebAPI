using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EF5x.Models.Mapping
{
    public class AppCollectionMap : EntityTypeConfiguration<AppCollection>
    {
        public AppCollectionMap()
        {
            // Primary Key
            this.HasKey(t => t.AppCollectionID);

            // Properties
            this.Property(t => t.CollectionName)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.Description)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("AppCollection");
            this.Property(t => t.AppCollectionID).HasColumnName("AppCollectionID");
            this.Property(t => t.taskTypeID).HasColumnName("taskTypeID");
            this.Property(t => t.CollectionName).HasColumnName("CollectionName");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Order).HasColumnName("Order");

            // Relationships
            this.HasRequired(t => t.TaskType)
                .WithMany(t => t.AppCollections)
                .HasForeignKey(d => d.taskTypeID);

        }
    }
}
