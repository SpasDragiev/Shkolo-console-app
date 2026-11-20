namespace Shkolo_console_app.UI;

// Shared between the key interceptor (HotKeyInput) and the components.
// F1..F12  -> top menu (App.razor)        1..9 -> sub-tabs of the open page (SubTabs.razor)
public class NavigationState
{
    // Raised with the key name: "F1".."F12" or "1".."9".
    public event Action<string>? HotKeyPressed;

    // True while a text box (FormInput) has the focus.
    public volatile bool BoxFocused;

    // A focused text box only accepts typing after the user pressed Tab ("enter the box").
    // It is locked again after every F-key / 1-9 navigation, so the digits keep switching tabs
    // even though a new tab puts its first box in focus automatically.
    public volatile bool TypingUnlocked;

    public bool Typing => BoxFocused && TypingUnlocked;

    public void Raise(string key)
    {
        TypingUnlocked = false;
        HotKeyPressed?.Invoke(key);
    }
}
