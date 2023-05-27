public record Claim
{
    public Guid Id { get; init; }
    public string ReferenceNo { get; init; } = default!;
    public Guid CreditorId { get; init; }
    public Creditor Creditor { get; init; } = default!;

    protected Claim(Guid id, string referenceNo, Guid creditorId)
    {
        Id = id;
        ReferenceNo = referenceNo;
        CreditorId = creditorId;
    }

    public static Claim Create(string referenceNo, Guid creditorId)
    {
        return new Claim(new ClaimId(), referenceNo, creditorId);
    }

    public virtual IReadOnlyCollection<ClaimDebtor> ClaimDebtors { get; set; } =
        new List<ClaimDebtor>();
}

public class ClaimConfiguration : IEntityTypeConfiguration<Claim>
{
    public void Configure(EntityTypeBuilder<Claim> builder)
    {
        builder.ToTable("Claims");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ReferenceNo).IsRequired();
        builder.HasOne(x => x.Creditor).WithMany().HasForeignKey(x => x.CreditorId);
    }
}
