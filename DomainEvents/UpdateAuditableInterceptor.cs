internal class UpdateAuditableInterceptor : SaveChangesInterceptor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDateTimeService _dateTimeService;
    private readonly IUserService _userService;

    public UpdateAuditableInterceptor(
        IServiceProvider serviceProvider,
        IDateTimeService dateTimeService,
        IUserService userService
    )
    {
        _serviceProvider = serviceProvider;
        _dateTimeService = dateTimeService;
        _userService = userService;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result
    )
    {
        if (eventData.Context == null)
        {
            throw new Exception("DbContext is null");
        }
        InvokeInterceptorServices(eventData.Context);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        if (eventData.Context == null)
        {
            throw new Exception("DbContext is null");
        }
        InvokeInterceptorServices(eventData.Context);
        return ValueTask.FromResult(result);
    }

    public void InvokeInterceptorServices(DbContext context)
    {
        var entries = context.ChangeTracker.Entries<AggregateRoot>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = _userService.CurrentUserId;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = _userService.CurrentUserId;
                    break;
                case EntityState.Deleted:
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    entry.Entity.DeletedBy = _userService.CurrentUserId;
                    entry.State = EntityState.Modified;
                    break;
            }
        }
    }
}
