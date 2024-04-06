using System.Windows.Controls;
using Globomantics.Windows.UserControls;
using Globomantics.Windows.ViewModels;

namespace Globomantics.Windows.Factories;

public class TodoUserControlFactory
{
    public static UserControl CreateUserControl(ITodoViewModel viewModel)
    {
        UserControl control = viewModel switch
        {
            BugViewModel => new BugControl(viewModel),
            FeatureViewModel => new FeatureControl(viewModel),
            _ => throw new NotImplementedException()
        };

        return control;
    }
}