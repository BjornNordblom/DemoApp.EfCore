public record Invoice
{
    public Guid Id { get; init; }
    public Guid ClaimItemId { get; init; }
    public ClaimItem ClaimItem { get; init; } = default!;
    public PositiveAmount Amount { get; init; } = PositiveAmount.From(0m);
}

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).IsRequired();
        builder
            .HasOne(x => x.ClaimItem)
            .WithOne()
            .HasForeignKey<Invoice>(x => x.ClaimItemId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
