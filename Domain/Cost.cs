public sealed record Cost
{
    public CostId CostId { get; init; } = CostId.New();

    public int CostDefinitionId { get; init; }
    public CostDefinition CostDefinition { get; init; } = default!;
    public decimal Amount { get; init; }
    public DateTime Date { get; init; }
}

public class CostConfiguration : IEntityTypeConfiguration<Cost>
{
    public void Configure(EntityTypeBuilder<Cost> builder)
    {
        builder.ToTable("Costs");
        builder.HasKey(x => x.CostId);
        //builder.Property(x => x.CostDefinition).IsRequired();
        builder.Property(x => x.Amount).IsRequired();
        builder.Property(x => x.Date).IsRequired();
        builder.HasOne(x => x.CostDefinition).WithMany().HasForeignKey(x => x.CostDefinitionId);
    }
}
