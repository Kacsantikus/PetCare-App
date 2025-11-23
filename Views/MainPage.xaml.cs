using test.ViewModel;

namespace test.Views;

public partial class MainPage : ContentPage
{
    private readonly DashboardViewModel _viewModel;

    public MainPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }
}
