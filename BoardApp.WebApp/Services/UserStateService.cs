using BoardApp.WebApp.Models;

namespace BoardApp.WebApp.Services;

public class UserStateService
{
    private ApplicationUser? _currentUser;
    public event Action? OnUserChanged;

    public ApplicationUser? CurrentUser
    {
        get => _currentUser;
        set
        {
            if (_currentUser != value)
            {
                _currentUser = value;
                NotifyUserChanged();
            }
        }
    }

    public void UpdateUser(ApplicationUser? user)
    {
        CurrentUser = user;
    }

    public void UpdateProfileImage(string? profileImageUrl)
    {
        if (_currentUser != null)
        {
            _currentUser.ProfileImageUrl = profileImageUrl;
            NotifyUserChanged();
        }
    }

    private void NotifyUserChanged()
    {
        OnUserChanged?.Invoke();
    }
}