using test.Views;

namespace test;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(VisitsPage), typeof(VisitsPage));
    }
}
