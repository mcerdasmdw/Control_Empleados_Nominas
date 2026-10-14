using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Planilla_RRHH.ViewModels;

namespace Planilla_RRHH.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}