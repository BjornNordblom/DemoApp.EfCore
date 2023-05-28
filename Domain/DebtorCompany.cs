public sealed class DebtorCompany : Debtor
{
    public string Name { get; init; } = default!;
    public string? OrganizationalNumber { get; init; }
    //    public Debtor IdNavigation { get; set; } = null!;
}

public class DebtorCompanyConfiguration : IEntityTypeConfiguration<DebtorCompany>
{
    public void Configure(EntityTypeBuilder<DebtorCompany> builder)
    {
        builder.ToTable("DebtorCompanies");
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.OrganizationalNumber).IsRequired(false);
        // builder
        //     .HasOne(d => d.IdNavigation)
        //     .WithOne(p => p.DebtorCompany)
        //     .HasForeignKey<DebtorCompany>(d => d.Id);
    }
}
