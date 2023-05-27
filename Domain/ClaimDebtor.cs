public record ClaimDebtor
{
    public enum DebtorInvolvement
    {
        Primary = 1,
        Secondary = 2,
        Guarantor = 3,
        CoDebtor = 4,
        CoSigner = 5
    };

    public Guid ClaimId { get; init; }
    public Claim Claim { get; init; } = default!;
    public Guid DebtorId { get; init; }
    public Debtor Debtor { get; init; } = default!;
    public DebtorInvolvement Involvement { get; init; } = default!;
}

public class ClaimDebtorConfiguration : IEntityTypeConfiguration<ClaimDebtor>
{
    public void Configure(EntityTypeBuilder<ClaimDebtor> builder)
    {
        builder.ToTable("ClaimDebtors");
        builder.HasKey(x => new { x.ClaimId, x.DebtorId });
        builder.Property(x => x.Involvement).IsRequired();
        builder.HasOne(d => d.Claim).WithMany(p => p.ClaimDebtors).HasForeignKey(d => d.ClaimId);
        builder.HasOne(d => d.Debtor).WithMany(p => p.DebtorClaims).HasForeignKey(d => d.DebtorId);
    }
}
