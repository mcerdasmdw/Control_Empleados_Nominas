using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Planilla_RRHH.ViewModels;

namespace Planilla_RRHH.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}