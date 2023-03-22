using System;
using System.Windows;
using System.Windows.Controls;
using Antlr4.Runtime;


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
            Console.WriteLine("Success!");
            // Get the text from the TxtBox
            //TextBox TxtBox = this.FindControl<TextBox>("TxtBox");
            var text = TxtBox.Text;
            
            try
            {
                ICharStream input = CharStreams.fromString(text);
                AlphaScanner lexer = new AlphaScanner(input);
                CommonTokenStream tokens = new CommonTokenStream(lexer);
                AlphaParser parser = new AlphaParser(tokens);
                parser.program();
                var tree = parser.program();
                // Print the result
                Console.WriteLine("Success!");
                
                //Print the tree
                Console.WriteLine(tree.ToStringTree(parser));

            }
            catch (Exception exception)
            {
                
                Console.WriteLine(exception);
                throw;
            }
        

        }
        private void Build_Button_Click(object sender, RoutedEventArgs e) {

        }
        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            
            Application.Current.Shutdown();
        }
    }
}