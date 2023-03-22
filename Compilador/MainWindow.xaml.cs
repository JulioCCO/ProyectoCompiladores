using System;
using System.Windows;
using Antlr4.Runtime;
using System.IO;
using Microsoft.Win32;


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
                        "C:\\Users\\Josue\\Escritorio\\TEC2\\Semestre VIII\\Compi\\ProyectoC#\\ProyectoCompiladores\\Compilador\\Components\\data.text");
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
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Buscar archivo .txt";
            openFileDialog1.ShowDialog();
            string texto = openFileDialog1.FileName;
            try
            {
                if (File.Exists(openFileDialog1.FileName))
                {
                    TextReader leer = new StreamReader(texto);
                    Pantalla.Text = leer.ReadToEnd();
                    leer.Close();
                }
            }
            
            catch (Exception  )
            {
                MessageBox.Show("Error al abrir el archivo de texto");
            }
            

        }
        private void Exit_Button_Click(object? sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}