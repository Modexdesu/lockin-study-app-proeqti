using System.Windows;
using lockin.wpf.ViewModels;

namespace lockin.wpf
{
    public partial class MainWindow : Window
    {
       private readonly MainViewModel _viewModel;
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();

            _viewModel= viewModel;
            this.DataContext = _viewModel;
            this.Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

           _viewModel.LoadTopicsAsync();


    }
}
}