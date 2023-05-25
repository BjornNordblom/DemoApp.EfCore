using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public record Claim
{
    public Guid Id { get; init; }
    public string ReferenceNo { get; init; }
    public Guid CreditorId { get; init; }
    public Creditor Creditor { get; init; } = default!;
}

public class ClaimConfiguration : IEntityTypeConfiguration<Claim>
{
    public void Configure(EntityTypeBuilder<Claim> builder)
    {
        builder.ToTable("Claims");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ReferenceNo).IsRequired();
    }
}
