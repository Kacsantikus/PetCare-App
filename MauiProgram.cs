using test.Data;
using test.ViewModel;
using test.Views;
using System.IO;


namespace test;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        string dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "petcare.db3");

        builder.Services.AddSingleton(new AppDatabase(dbPath));

        // ViewModel + Page
        builder.Services.AddSingleton<PetsViewModel>();
        builder.Services.AddSingleton<PetsPage>();
        builder.Services.AddTransient<VisitsViewModel>();
        builder.Services.AddTransient<VisitsPage>();


        return builder.Build();
    }
}
