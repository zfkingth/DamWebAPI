using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Hammergo.Data.Mapping
{
    public class FormulaMap : EntityTypeConfiguration<Formula>
    {
        public FormulaMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.FormulaExpression)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("Formula");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ParamId).HasColumnName("ParamId");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.FormulaExpression).HasColumnName("FormulaExpression");
            this.Property(t => t.CalculateOrder).HasColumnName("CalculateOrder");

            // Relationships
            this.HasRequired(t => t.CalculateParam)
                .WithMany(t => t.Formulae)
                .HasForeignKey(d => d.ParamId);

        }
    }
}
