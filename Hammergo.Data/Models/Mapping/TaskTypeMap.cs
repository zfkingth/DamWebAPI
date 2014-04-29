using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Hammergo.Data.Mapping
{
    public class TaskTypeMap : EntityTypeConfiguration<TaskType>
    {
        public TaskTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TypeName)
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("TaskType");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TypeName).HasColumnName("TypeName");
        }
    }
}
