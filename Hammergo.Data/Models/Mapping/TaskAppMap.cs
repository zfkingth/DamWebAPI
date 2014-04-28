using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Hammergo.Data.Mapping
{
    public class TaskAppMap : EntityTypeConfiguration<TaskApp>
    {
        public TaskAppMap()
        {
            // Primary Key
            this.HasKey(t => new {t.ID});

            // Properties
  

            // Table & Column Mappings
            this.ToTable("TaskApp");
            this.Property(t => t.AppCollectionID).HasColumnName("AppCollectionID");
            this.Property(t => t.AppId).HasColumnName("AppId");
            this.Property(t => t.Order).HasColumnName("Order");

            // Relationships
            this.HasRequired(t => t.App)
                .WithMany(t => t.TaskApps)
                .HasForeignKey(d => d.AppId);
            this.HasRequired(t => t.AppCollection)
                .WithMany(t => t.TaskApps)
                .HasForeignKey(d => d.AppCollectionID);

        }
    }
}
