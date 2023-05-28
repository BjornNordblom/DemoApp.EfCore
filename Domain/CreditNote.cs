public record CreditNote
{
    public Guid CreditNoteId { get; init; }
    public Guid ClaimItemId { get; init; }
    public ClaimItem ClaimItem { get; init; } = default!;
    public Guid? CreditedInvoiceId { get; init; }
    public Invoice? CreditedInvoice { get; init; }
    public NegativeAmount Amount { get; init; } = NegativeAmount.Zero;
}

public class CreditNoteConfiguration : IEntityTypeConfiguration<CreditNote>
{
    public void Configure(EntityTypeBuilder<CreditNote> builder)
    {
        builder.ToTable("CreditNotes");
        builder.HasKey(x => x.CreditNoteId);
        builder.Property(x => x.Amount).IsRequired();
        builder
            .HasOne(x => x.ClaimItem)
            .WithOne()
            .HasForeignKey<CreditNote>(x => x.ClaimItemId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasOne(x => x.CreditedInvoice)
            .WithMany()
            .HasForeignKey(x => x.CreditedInvoiceId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
