using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Hammergo.Data.Mapping
{
    public class AppCollectionMap : EntityTypeConfiguration<AppCollection>
    {
        public AppCollectionMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.CollectionName)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.Description)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("AppCollection");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TaskTypeID).HasColumnName("TaskTypeID");
            this.Property(t => t.CollectionName).HasColumnName("CollectionName");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Order).HasColumnName("Order");
            this.Property(t => t.ParentCollection).HasColumnName("ParentCollection");
            this.Property(t => t.SUM).HasColumnName("SUM");
            this.Property(t => t.MAX).HasColumnName("MAX");
            this.Property(t => t.MIN).HasColumnName("MIN");
            this.Property(t => t.AVG).HasColumnName("AVG");
            this.Property(t => t.CNT).HasColumnName("CNT");

            // Relationships
            this.HasRequired(t => t.TaskType)
                .WithMany(t => t.AppCollections)
                .HasForeignKey(d => d.TaskTypeID);

        }
    }
}
