using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class Claim : AggregateRoot
{
    public ClaimId Id { get; init; }
    public string ReferenceNo { get; init; } = default!;
    public Guid CreditorId { get; init; }
    public Creditor Creditor { get; init; } = default!;

    protected Claim(ClaimId id, string referenceNo, Guid creditorId)
    {
        Id = id;
        ReferenceNo = referenceNo;
        CreditorId = creditorId;
    }

    public static Claim Create(string referenceNo, Guid creditorId)
    {
        return new Claim(new ClaimId(), referenceNo, creditorId);
    }
}

public class ClaimConfiguration : IEntityTypeConfiguration<Claim>
{
    public void Configure(EntityTypeBuilder<Claim> builder)
    {
        builder.ToTable("Claims");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new ClaimId(x));
        builder.Property(x => x.ReferenceNo).IsRequired();
    }
}
