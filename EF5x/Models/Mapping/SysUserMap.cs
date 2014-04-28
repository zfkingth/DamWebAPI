using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EF5x.Models.Mapping
{
    public class SysUserMap : EntityTypeConfiguration<SysUser>
    {
        public SysUserMap()
        {
            // Primary Key
            this.HasKey(t => t.UserName);

            // Properties
            this.Property(t => t.UserName)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.PasswordHash)
                .HasMaxLength(200);

            this.Property(t => t.Salt)
                .HasMaxLength(20);

            this.Property(t => t.Question)
                .HasMaxLength(50);

            this.Property(t => t.Answer)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("SysUser");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.PasswordHash).HasColumnName("PasswordHash");
            this.Property(t => t.Salt).HasColumnName("Salt");
            this.Property(t => t.Question).HasColumnName("Question");
            this.Property(t => t.Answer).HasColumnName("Answer");
            this.Property(t => t.roleID).HasColumnName("roleID");

            // Relationships
            this.HasOptional(t => t.Role)
                .WithMany(t => t.SysUsers)
                .HasForeignKey(d => d.roleID);

        }
    }
}
