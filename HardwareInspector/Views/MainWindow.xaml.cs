using System.Windows;
using HardwareInspector.ViewModels;

namespace HardwareInspector.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            Closing += (_, _) => vm.Dispose();
        }
    }
}
