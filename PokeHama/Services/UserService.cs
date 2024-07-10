using Microsoft.AspNetCore.Components;

namespace PokeHama.Services;

public class UserService
{
    public delegate Task OnUserDataChangedEventHandler();
    public event OnUserDataChangedEventHandler? OnUserDataChanged;

    public async Task InvokeUserDataChanged()
    {
        if (OnUserDataChanged != null)
        {
            await OnUserDataChanged.Invoke();
        }
    }
}