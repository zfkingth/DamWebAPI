using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Hammergo.Data.Mapping
{
    public class TaskTypeMap : EntityTypeConfiguration<TaskType>
    {
        public TaskTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.TaskTypeID);

            // Properties
            this.Property(t => t.TaskTypeID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TypeName)
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("TaskType");
            this.Property(t => t.TaskTypeID).HasColumnName("TaskTypeID");
            this.Property(t => t.TypeName).HasColumnName("TypeName");
        }
    }
}
