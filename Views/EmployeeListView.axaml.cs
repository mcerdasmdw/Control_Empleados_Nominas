using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Planilla_RRHH.ViewModels;

namespace Planilla_RRHH.Views;

public partial class EmployeeListView : UserControl
{
    public EmployeeListView()
    {
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}