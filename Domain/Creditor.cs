public record Creditor
{
    public CreditorId CreditorId { get; init; } = CreditorId.New();
    public string Name { get; init; } = default!;
    public virtual CreditorId? ParentCreditorId { get; init; } = null;
    public virtual Creditor? ParentCreditor { get; init; }

    public virtual IReadOnlyCollection<Creditor> SubCreditors { get; init; } = new List<Creditor>();
    public virtual IReadOnlyCollection<Claim> Claims { get; init; } = new List<Claim>();
}

public class CreditorConfiguration : IEntityTypeConfiguration<Creditor>
{
    public void Configure(EntityTypeBuilder<Creditor> builder)
    {
        builder.ToTable("Creditors");
        builder.HasKey(x => x.CreditorId);
        builder.Property(x => x.Name).IsRequired();
        builder
            .HasOne(x => x.ParentCreditor)
            .WithMany(x => x.SubCreditors)
            .HasForeignKey(x => x.ParentCreditorId);
        builder.HasMany(x => x.Claims).WithOne(x => x.Creditor).HasForeignKey(x => x.CreditorId);
    }
}
