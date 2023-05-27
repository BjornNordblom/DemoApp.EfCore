using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class ClaimIdConverter : ValueConverter<ClaimId, Guid>
{
    public ClaimIdConverter()
        : base(v => v.Value, v => new ClaimId(v)) { }
}
