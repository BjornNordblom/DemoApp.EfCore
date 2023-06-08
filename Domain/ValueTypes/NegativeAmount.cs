using Vogen;

[ValueObject(
    conversions: Conversions.EfCoreValueConverter
        | Conversions.TypeConverter
        | Conversions.SystemTextJson,
    underlyingType: typeof(decimal)
)]
public readonly partial struct NegativeAmount
{
    public static Validation Validate(decimal value) =>
        value <= 0 ? Validation.Ok : Validation.Invalid("Amount must be negative.");
}
