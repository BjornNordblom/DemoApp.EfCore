public record DebtorNaturalPerson : Debtor
{
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string? PersonalNumber { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public virtual Debtor IdNavigation { get; set; } = null!;

    public int? GetAge(IDateTimeService dateTimeService)
    {
        if (DateOfBirth is null)
        {
            return null;
        }
        var today = dateTimeService.Now;
        var dateOfBirth = DateOfBirth.Value;
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth > today.AddYears(-age))
        {
            age--;
        }
        return age;
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
