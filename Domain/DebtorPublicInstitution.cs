public sealed class DebtorPublicInstitution : Debtor
{
    public string Name { get; init; } = default!;
    public string? OrganizationalNumber { get; init; }
    public Debtor IdNavigation { get; set; } = null!;
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
