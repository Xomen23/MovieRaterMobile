namespace MovieRaterApp.Controls;

public partial class PasswordEntry : ContentView
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(PasswordEntry),
        default(string),
        BindingMode.TwoWay);

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder),
        typeof(string),
        typeof(PasswordEntry),
        default(string));

    private bool _isPasswordHidden = true;

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public PasswordEntry()
    {
        InitializeComponent();
        UpdateToggleIcon();
    }

    private void OnToggleClicked(object? sender, EventArgs e)
    {
        _isPasswordHidden = !_isPasswordHidden;
        PasswordField.IsPassword = _isPasswordHidden;
        UpdateToggleIcon();
    }

    private void UpdateToggleIcon()
    {
        ToggleButton.Text = _isPasswordHidden ? "👁" : "🙈";
    }
}
