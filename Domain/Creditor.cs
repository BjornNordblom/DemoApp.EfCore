using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public record Creditor
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public Guid ParentCreditorId { get; init; } = Guid.Empty;

    // public virtual Creditor? ParentCreditor { get; init; }
    // public virtual ICollection<Creditor> SubCreditors { get; init; }
    //public ICollection<Claim> Claims { get; init; } = new List<Claim>();
}

public class CreditorConfiguration : IEntityTypeConfiguration<Creditor>
{
    public void Configure(EntityTypeBuilder<Creditor> builder)
    {
        builder.ToTable("Creditors");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.ParentCreditorId).HasDefaultValue(Guid.Empty);
        //builder.HasMany(x => x.Claims).WithOne().HasForeignKey(x => x.CreditorId);

        //     builder
        //         .HasMany(x => x.SubCreditors)
        //         .WithOne(x => x.ParentCreditor)
        //         .HasForeignKey(x => x.ParentCreditorId);
        //     builder.HasOne(x => x.ParentCreditor).WithMany().HasForeignKey(x => x.ParentCreditorId);
    }
}
