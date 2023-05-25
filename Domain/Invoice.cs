using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public record Invoice
{
    public Guid Id { get; init; }
    public Guid ClaimItemId { get; init; }
    public ClaimItem ClaimItem { get; init; } = default!;
    public decimal Amount { get; init; }
}

public record CreditInvoice
{
    public Guid Id { get; init; }
    public Guid ClaimItemId { get; init; }
    public ClaimItem ClaimItem { get; init; } = default!;
    public Guid CreditedInvoiceId { get; init; } = Guid.Empty;
    public Invoice? CreditedInvoice { get; init; }
    public decimal CreditedAmount { get; init; }
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

public class CreditInvoiceConfiguration : IEntityTypeConfiguration<CreditInvoice>
{
    public void Configure(EntityTypeBuilder<CreditInvoice> builder)
    {
        builder.ToTable("CreditInvoices");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CreditedAmount).IsRequired();
        builder
            .HasOne(x => x.ClaimItem)
            .WithOne()
            .HasForeignKey<CreditInvoice>(x => x.ClaimItemId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasOne(x => x.CreditedInvoice)
            .WithMany()
            .HasForeignKey(x => x.CreditedInvoiceId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
