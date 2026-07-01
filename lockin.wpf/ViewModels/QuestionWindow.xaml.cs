using lockin.core.Models;
using lockin.wpf.ViewModels; // Ensure this points to your new ViewModel namespace
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace lockin.wpf.Views // Must match x:Class in XAML
{
    public partial class QuestionWindow : Window
    {
        private readonly QuestionViewModel _viewModel;

        public QuestionWindow(Question question)
        {
            InitializeComponent();
            _viewModel = new QuestionViewModel(question);
            this.DataContext = _viewModel;
        }

        private void Answer_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var selectedAnswer = (Answer)button.Tag;

            if (_viewModel.IsAnswerCorrect(selectedAnswer))
            {
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D32F2F"));
            }
        }
    }
}