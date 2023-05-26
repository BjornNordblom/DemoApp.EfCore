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

    public ICollection<Debtor> Debtors { get; init; }
}

public class ClaimConfiguration : IEntityTypeConfiguration<Claim>
{
    public void Configure(EntityTypeBuilder<Claim> builder)
    {
        builder.ToTable("Claims");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new ClaimId(x));
        builder.Property(x => x.ReferenceNo).IsRequired();
        builder
            .HasMany(x => x.Debtors)
            .WithMany(x => x.Claims)
            .UsingEntity<ClaimDebtor>(
                x => x.HasOne(x => x.Debtor).WithMany().HasForeignKey(x => x.DebtorId),
                x => x.HasOne(x => x.Claim).WithMany().HasForeignKey(x => x.ClaimId),
                x =>
                {
                    x.ToTable("ClaimDebtors");
                    x.HasKey(x => new { x.ClaimId, x.DebtorId });
                    x.Property(x => x.Involvement).IsRequired();
                }
            );
        // builder.HasOne(x => x.Creditor).WithMany().HasForeignKey(x => x.CreditorId);
        // builder.HasMany(x => x.ClaimsDebtors).WithOne().HasForeignKey(x => x.ClaimId);
        //builder.HasMany(x => x.ClaimDebtors).WithOne().HasForeignKey(x => x.ClaimId);
        //builder.HasOne(x => x.Creditor).WithMany().HasForeignKey(x => x.CreditorId);
    }
}
