public record Claim
{
    public ClaimId Id { get; init; } = ClaimId.New();
    public string ReferenceNo { get; init; } = default!;
    public CreditorId CreditorId { get; init; }
    public Creditor Creditor { get; init; } = default!;

    public Claim() { }

    public Claim(ClaimId id, string referenceNo, CreditorId creditorId)
    {
        Id = id;
        ReferenceNo = referenceNo;
        CreditorId = creditorId;
    }

    public static Claim Create(string referenceNo, CreditorId creditorId)
    {
        return new Claim(ClaimId.New(), referenceNo, creditorId);
    }

    public virtual ICollection<ClaimDebtor> ClaimDebtors { get; set; } = new List<ClaimDebtor>();
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
