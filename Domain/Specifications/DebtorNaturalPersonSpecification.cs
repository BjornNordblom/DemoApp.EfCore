public class IsAdultSpecification : ISpecification<DebtorNaturalPerson>
{
    private readonly IDateTimeService _dateTimeService;

    public IsAdultSpecification(IDateTimeService dateTimeService)
    {
        _dateTimeService = dateTimeService;
    }

    public bool IsSatisfiedBy(DebtorNaturalPerson debtorNaturalPerson)
    {
        var age = debtorNaturalPerson.GetAge(_dateTimeService);
        return age >= 18;
    }

    public Expression<Func<DebtorNaturalPerson, bool>> ToExpression()
    {
        return DebtorNaturalPerson =>
            DebtorNaturalPerson.DateOfBirth <= _dateTimeService.Now.AddYears(18);
    }
}
