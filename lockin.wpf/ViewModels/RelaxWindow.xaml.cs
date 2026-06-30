using System;
using System.Windows;
using System.Windows.Controls;

namespace lockin.wpf.Views
{
    public partial class RelaxWindow : Window
    {
        // These properties will be read by the ViewModel
        public TimeSpan SelectedDuration { get; private set; }
        public bool IsCompletelyStopped { get; private set; }

        public RelaxWindow()
        {
            InitializeComponent();
        }

        private void BtnDuration_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            // Extract the minutes from the button's Tag property
            int minutes = int.Parse(btn.Tag.ToString());
            SelectedDuration = TimeSpan.FromMinutes(minutes);

            this.DialogResult = true; // Signals that a choice was made
            this.Close();
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            IsCompletelyStopped = true;
            this.DialogResult = true;
            this.Close();
        }
    }
}