internal sealed class UserService : IUserService
{
    private readonly Func<Guid> _currentUserId;

    public Guid CurrentUserId => _currentUserId();
    public static Guid System => Guid.Parse("5eaf000d-fade-fa11-1eaf-101c0feebabe");

    public UserService()
    {
        _currentUserId = () => System;
    }

    public UserService(Func<Guid> currentUserIdDelegate)
    {
        _currentUserId = currentUserIdDelegate;
    }
}
