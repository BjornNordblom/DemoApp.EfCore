using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
}

public record DebtorNaturalPerson
{
    public Guid Id { get; init; }
    public Guid DebtorId { get; init; }
    public Debtor Debtor { get; init; } = default!;
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string? PersonalNumber { get; init; }
}

public record DebtorCompany
{
    public Guid Id { get; init; }
    public Guid DebtorId { get; init; }
    public Debtor Debtor { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string? OrganizationalNumber { get; init; }
}

public record DebtorPublicInstitution
{
    public Guid Id { get; init; }
    public Guid DebtorId { get; init; }
    public Debtor Debtor { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string? OrganizationalNumber { get; init; }
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
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FirstName).IsRequired();
        builder.Property(x => x.LastName).IsRequired();
        builder.Property(x => x.PersonalNumber).IsRequired(false);
        builder.HasOne(x => x.Debtor).WithOne().HasForeignKey<DebtorNaturalPerson>(x => x.DebtorId);
    }
}

public class DebtorCompanyConfiguration : IEntityTypeConfiguration<DebtorCompany>
{
    public void Configure(EntityTypeBuilder<DebtorCompany> builder)
    {
        builder.ToTable("DebtorCompanies");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.OrganizationalNumber).IsRequired(false);
        builder.HasOne(x => x.Debtor).WithOne().HasForeignKey<DebtorCompany>(x => x.DebtorId);
    }
}

public class DebtorPublicInstitutionConfiguration
    : IEntityTypeConfiguration<DebtorPublicInstitution>
{
    public void Configure(EntityTypeBuilder<DebtorPublicInstitution> builder)
    {
        builder.ToTable("DebtorPublicInstitutions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.OrganizationalNumber).IsRequired(false);
        builder
            .HasOne(x => x.Debtor)
            .WithOne()
            .HasForeignKey<DebtorPublicInstitution>(x => x.DebtorId);
    }
}
