using System.Windows;
using lockin.wpf.ViewModels;

namespace lockin.wpf
{
    public partial class MainWindow : Window
    {
        // Constructor Injection handles this cleanly
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();

            // This is the line that links the visual XAML to your C# ViewModel logic
            this.DataContext = viewModel;
        }
    }
}