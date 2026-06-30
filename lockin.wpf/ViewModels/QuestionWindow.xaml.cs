using lockin.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace lockin.wpf.Views
{
    public partial class QuestionWindow : Window
    {
        private string _correctAnswer;

        public QuestionWindow(Question question)
        {
            InitializeComponent();

            // 1. Set the question text
            QuestionTextBlock.Text = question.QuestionText;
            _correctAnswer = question.CorrectAnswer;

            // 2. Load the answers into a list
            var options = new List<string>
            {
                question.CorrectAnswer,
                question.Option1,
                question.Option2,
                question.Option3
            };

            // 3. Shuffle the list randomly so the correct answer moves around
            var shuffledOptions = options.OrderBy(x => Guid.NewGuid()).ToList();

            // 4. Assign the shuffled text to your UI buttons
            BtnA.Content = shuffledOptions[0];
            BtnB.Content = shuffledOptions[1];
            BtnC.Content = shuffledOptions[2];
            BtnD.Content = shuffledOptions[3];
        }

        // HERE IS THE MISSING METHOD YOUR XAML WAS BEGGING FOR:
        private void Answer_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            string selectedAnswer = clickedButton.Content.ToString();

            if (selectedAnswer == _correctAnswer)
            {
                // They got it right. Release the hostage.
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                // They got it wrong. Turn the button Red (#D32F2F) and punish them.
                clickedButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D32F2F"));
                MessageBox.Show("Wrong answer. You are still locked in.", "Incorrect");
            }
        }
    }
}