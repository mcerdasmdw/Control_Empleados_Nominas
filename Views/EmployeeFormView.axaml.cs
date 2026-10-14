using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Planilla_RRHH.ViewModels;

namespace Planilla_RRHH.Views;

public partial class EmployeeFormView : UserControl
{
    public EmployeeFormView()
    {
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}