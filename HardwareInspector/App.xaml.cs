using System.Windows;

namespace HardwareInspector;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainWindow = new Views.MainWindow
        {
            DataContext = new ViewModels.MainViewModel()
        };
        mainWindow.Show();
    }
}
