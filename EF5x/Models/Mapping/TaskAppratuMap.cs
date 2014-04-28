using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EF5x.Models.Mapping
{
    public class TaskAppratuMap : EntityTypeConfiguration<TaskAppratu>
    {
        public TaskAppratuMap()
        {
            // Primary Key
            this.HasKey(t => new { t.appCollectionID, t.appName });

            // Properties
            this.Property(t => t.appName)
                .IsRequired()
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("TaskAppratus");
            this.Property(t => t.appCollectionID).HasColumnName("appCollectionID");
            this.Property(t => t.appName).HasColumnName("appName");
            this.Property(t => t.Order).HasColumnName("Order");

            // Relationships
            this.HasRequired(t => t.Apparatus)
                .WithMany(t => t.TaskAppratus)
                .HasForeignKey(d => d.appName);
            this.HasRequired(t => t.AppCollection)
                .WithMany(t => t.TaskAppratus)
                .HasForeignKey(d => d.appCollectionID);

        }
    }
}
