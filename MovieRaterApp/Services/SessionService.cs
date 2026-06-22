using MovieRaterApp.Models;

namespace MovieRaterApp.Services;

public class SessionService
{
    private const string UserIdKey = "session_user_id";
    private const string UsernameKey = "session_username";
    private const string FirstNameKey = "session_first_name";
    private const string LastNameKey = "session_last_name";

    public void SaveUser(User user)
    {
        Preferences.Set(UserIdKey, user.Id ?? string.Empty);
        Preferences.Set(UsernameKey, user.Username ?? string.Empty);
        Preferences.Set(FirstNameKey, user.FirstName ?? string.Empty);
        Preferences.Set(LastNameKey, user.LastName ?? string.Empty);
    }

    public User? GetSavedUser()
    {
        var userId = Preferences.Get(UserIdKey, string.Empty);
        if (string.IsNullOrWhiteSpace(userId))
            return null;

        return new User
        {
            Id = userId,
            Username = Preferences.Get(UsernameKey, string.Empty),
            FirstName = Preferences.Get(FirstNameKey, string.Empty),
            LastName = Preferences.Get(LastNameKey, string.Empty)
        };
    }

    public void ClearSession()
    {
        Preferences.Remove(UserIdKey);
        Preferences.Remove(UsernameKey);
        Preferences.Remove(FirstNameKey);
        Preferences.Remove(LastNameKey);
    }
}
