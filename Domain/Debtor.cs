public class Debtor : AggregateRoot
{
    public enum DebtorType
    {
        NaturalPerson = 1,
        Company = 2,
        PublicInstitution = 3
    };

    public Debtor() { }

    public static Debtor Create(
        DebtorType type,
        DebtorCompany? debtorCompany = null,
        DebtorNaturalPerson? debtorNaturalPerson = null,
        DebtorPublicInstitution? debtorPublicInstitution = null
    )
    {
        if (type == DebtorType.NaturalPerson && debtorNaturalPerson is null)
            throw new ArgumentNullException(nameof(debtorNaturalPerson));
        if (type == DebtorType.Company && debtorCompany is null)
            throw new ArgumentNullException(nameof(debtorCompany));
        if (type == DebtorType.PublicInstitution && debtorPublicInstitution is null)
            throw new ArgumentNullException(nameof(debtorPublicInstitution));

        var debtor = new Debtor
        {
            Type = type,
            DebtorCompany = debtorCompany,
            DebtorNaturalPerson = debtorNaturalPerson,
            DebtorPublicInstitution = debtorPublicInstitution
        };

        return debtor;
    }

    public DebtorId DebtorId { get; init; } = DebtorId.New();
    public DebtorType Type { get; init; } = default!;
    public IReadOnlyCollection<ClaimDebtor> DebtorClaims { get; init; } = new List<ClaimDebtor>();

    public DebtorCompany? DebtorCompany { get; set; }

    public DebtorNaturalPerson? DebtorNaturalPerson { get; set; }

    public DebtorPublicInstitution? DebtorPublicInstitution { get; set; }
}

public class DebtorConfiguration : IEntityTypeConfiguration<Debtor>
{
    public void Configure(EntityTypeBuilder<Debtor> builder)
    {
        builder.ToTable("Debtors");
        builder.HasKey(x => x.DebtorId);
        builder.Property(x => x.Type).IsRequired();
    }
}
