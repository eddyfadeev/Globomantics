using System.Windows.Controls;
using Globomantics.Windows.ViewModels;

namespace Globomantics.Windows.UserControls;

/// <summary>
/// Interaction logic for BugControl.xaml
/// </summary>
public partial class BugControl : UserControl
{
    public BugControl(IViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;

        ParentTodo.SelectedIndex = -1;
    }
}