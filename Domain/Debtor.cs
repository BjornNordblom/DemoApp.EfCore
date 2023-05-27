public record Debtor
{
    public enum DebtorType
    {
        NaturalPerson = 1,
        Company = 2,
        PublicInstitution = 3
    };

    public Guid Id { get; init; }
    public DebtorType Type { get; init; } = default!;
    public virtual IReadOnlyCollection<ClaimDebtor> DebtorClaims { get; init; } =
        new List<ClaimDebtor>();

    public virtual DebtorCompany? DebtorCompany { get; set; }

    public virtual DebtorNaturalPerson? DebtorNaturalPerson { get; set; }

    public virtual DebtorPublicInstitution? DebtorPublicInstitution { get; set; }
}

public record DebtorNaturalPerson : Debtor
{
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string? PersonalNumber { get; init; }
    public virtual Debtor IdNavigation { get; set; } = null!;
}

public record DebtorCompany : Debtor
{
    public string Name { get; init; } = default!;
    public string? OrganizationalNumber { get; init; }
    public virtual Debtor IdNavigation { get; set; } = null!;
}

public record DebtorPublicInstitution : Debtor
{
    public string Name { get; init; } = default!;
    public string? OrganizationalNumber { get; init; }
    public virtual Debtor IdNavigation { get; set; } = null!;
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

public class DebtorNaturalPersonConfiguration : IEntityTypeConfiguration<DebtorNaturalPerson>
{
    public void Configure(EntityTypeBuilder<DebtorNaturalPerson> builder)
    {
        builder.ToTable("DebtorNaturalPersons");
        builder.Property(x => x.FirstName).IsRequired();
        builder.Property(x => x.LastName).IsRequired();
        builder.Property(x => x.PersonalNumber).IsRequired(false);
        builder
            .HasOne(d => d.IdNavigation)
            .WithOne(p => p.DebtorNaturalPerson)
            .HasForeignKey<DebtorNaturalPerson>(d => d.Id);
    }
}

public class DebtorCompanyConfiguration : IEntityTypeConfiguration<DebtorCompany>
{
    public void Configure(EntityTypeBuilder<DebtorCompany> builder)
    {
        builder.ToTable("DebtorCompanies");
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.OrganizationalNumber).IsRequired(false);
        builder
            .HasOne(d => d.IdNavigation)
            .WithOne(p => p.DebtorCompany)
            .HasForeignKey<DebtorCompany>(d => d.Id);
    }
}

public class DebtorPublicInstitutionConfiguration
    : IEntityTypeConfiguration<DebtorPublicInstitution>
{
    public void Configure(EntityTypeBuilder<DebtorPublicInstitution> builder)
    {
        builder.ToTable("DebtorPublicInstitutions");
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.OrganizationalNumber).IsRequired(false);
        builder
            .HasOne(d => d.IdNavigation)
            .WithOne(p => p.DebtorPublicInstitution)
            .HasForeignKey<DebtorPublicInstitution>(d => d.Id);
    }
}
