using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public record Claim
{
    public Guid Id { get; init; }
    public string ReferenceNo { get; init; }
    public Guid CreditorId { get; init; }
    public Creditor Creditor { get; init; } = default!;
    public ICollection<ClaimDebtor> ClaimDebtors { get; init; }
    public ICollection<ClaimItem> ClaimItems { get; init; }
}

public class ClaimConfiguration : IEntityTypeConfiguration<Claim>
{
    public void Configure(EntityTypeBuilder<Claim> builder)
    {
        builder.ToTable("Claims");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ReferenceNo).IsRequired();
        builder.HasMany(x => x.ClaimItems).WithOne().HasForeignKey(x => x.ClaimId);
        builder
            .HasOne(x => x.Creditor)
            .WithMany()
            .HasForeignKey(x => x.CreditorId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasMany(x => x.ClaimDebtors).WithOne().HasForeignKey(x => x.ClaimId);
    }
}
