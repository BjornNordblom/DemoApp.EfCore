public record Debtor
{
    public enum DebtorType
    {
        NaturalPerson = 1,
        Company = 2,
        PublicInstitution = 3
    };

    public DebtorId Id { get; init; } = DebtorId.New();
    public DebtorType Type { get; init; } = default!;
    public virtual IReadOnlyCollection<ClaimDebtor> DebtorClaims { get; init; } =
        new List<ClaimDebtor>();

    public virtual DebtorCompany? DebtorCompany { get; set; }

    public virtual DebtorNaturalPerson? DebtorNaturalPerson { get; set; }

    public virtual DebtorPublicInstitution? DebtorPublicInstitution { get; set; }
}

public class DebtorConfiguration : IEntityTypeConfiguration<Debtor>
{
    public void Configure(EntityTypeBuilder<Debtor> builder)
    {
        builder.ToTable("Debtors");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).IsRequired();
    }
}
