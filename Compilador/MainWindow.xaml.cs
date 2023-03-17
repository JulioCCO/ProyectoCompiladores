using System;
using System.Windows;

namespace Compilador
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        public MainWindow() {
            InitializeComponent();
        }

        private void Run_Button_Click(object sender, RoutedEventArgs e) {
            // Get the text from the TxtBox
            var text = TxtBox.Text;

            MessageBox.Show(text);
            // Write the text to the console
            Console.WriteLine(text);
        }
        private void Build_Button_Click(object sender, RoutedEventArgs e) {

        }
        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }
    }
}