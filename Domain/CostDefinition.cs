public sealed record CostDefinition
{
    public int Id { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string Description { get; init; } = default!;
    public bool Enabled { get; init; } = default!;
    public decimal BaseAmount { get; init; }
    public CostVATType VATType { get; init; }

    public enum CostVATType
    {
        None,
        VAT
    }
}

public class CostDefinitionConfiguration : IEntityTypeConfiguration<CostDefinition>
{
    public void Configure(EntityTypeBuilder<CostDefinition> builder)
    {
        builder.ToTable("CostDefinitions");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasAlternateKey(x => x.Name);
        builder.Property(x => x.Description).IsRequired();
        builder.Property(x => x.BaseAmount).IsRequired();
        builder.Property(x => x.VATType).IsRequired();
    }
}
