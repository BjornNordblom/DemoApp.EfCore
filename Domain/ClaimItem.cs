public record ClaimItem
{
    public enum ClaimType
    {
        Invoice = 1,
        CreditNote = 2
    }

    public Guid ClaimItemId { get; init; }
    public ClaimId ClaimId { get; init; }
    public Claim Claim { get; init; } = default!;
    public string ReferenceNo { get; init; } = default!;
    public ClaimType Type { get; init; } = default!;
}

public class ClaimItemConfiguration : IEntityTypeConfiguration<ClaimItem>
{
    public void Configure(EntityTypeBuilder<ClaimItem> builder)
    {
        builder.ToTable("ClaimItems");
        builder.HasKey(x => x.ClaimItemId);
        builder.Property(x => x.ReferenceNo).IsRequired();
        builder.Property(x => x.Type).IsRequired();
    }
}
