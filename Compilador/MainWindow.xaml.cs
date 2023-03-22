using System;
using System.Windows;
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

        private void Run_Button_Click(object? sender, RoutedEventArgs e) {
            Console.WriteLine("Success!");
            // Get the text from the TxtBox
            // object TxtBox = this.FindName("TxtBox");
            // var text = TxtBox.ToString();
            try
            {
                ICharStream input =
                    CharStreams.fromPath(
                        "C:\\Users\\jjuli\\RiderProjects\\MiniCSharp\\MiniCSharp\\Components\\data.txt");
                AlphaScanner lexer = new AlphaScanner(input);
                CommonTokenStream tokens = new CommonTokenStream(lexer);
                AlphaParser parser = new AlphaParser(tokens);
                parser.program();
                AlphaParser.ProgramContext tree = parser.program();
                // Print the result
                Console.Write("Success!");

                //Print the tree
                Console.Write(tree.ToStringTree(parser));
            }
            catch (Exception exception)
            {
                Console.Write("Error!");
                Console.Write(exception);
                throw;
            }
            
        }
        private void Build_Button_Click(object? sender, RoutedEventArgs e) {
                 //aqui va el codigo para cargar el data.txt en el txtbox
                   
        }
        private void Exit_Button_Click(object? sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}