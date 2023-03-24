using System;
using System.Windows;
using Antlr4.Runtime;
using System.IO;
using System.Windows.Controls;
using Compilador.Components;
using Microsoft.Win32;


namespace Compilador
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        public MainWindow() {
            System.Diagnostics.Debug.WriteLine("System Diagnostics Debug");
            InitializeComponent();
        }

        private void Pantalla_SelectionChanged(object sender, EventArgs e)
        {
            UpdateCursorPosition();
        }
        private void Pantalla_TextChanged(object sender, EventArgs e)
        {
            UpdateCursorPosition();
        }   
        
        private void UpdateCursorPosition()
        {
            
            int index = Pantalla.SelectionStart;
            int line = Pantalla.GetLineIndexFromCharacterIndex(index) + 1;
            int column = index - Pantalla.GetCharacterIndexFromLineIndex(Pantalla.GetLineIndexFromCharacterIndex(index)) + 1;
         
            // Actualiza el texto de un label o de otro TextBox con el número de línea y columna.
            Output.Content = $"Línea: {line} \nColumna: {column}";
        }
        
        private void Run_Button_Click(object? sender, RoutedEventArgs e) {
            
            try
            {
                // Get the text from the TxtBox
                string text = Pantalla.Text;
                
                // debemos guardar lo del textBox en el Txt que se subio al TextBox
                
                
                System.Diagnostics.Debug.WriteLine("\nInformacion tomada del TextBox:  \n" + text + "\n");
                ICharStream input2 = CharStreams.fromString(text);
                AlphaScanner lexer = new AlphaScanner(input2);
                CommonTokenStream tokens = new CommonTokenStream(lexer);
                AlphaParser parser = new AlphaParser(tokens);
                
                // Error Listener
                MyErrorListener errorListener = new MyErrorListener();
                // Add the error listener to the lexer and parser
                lexer.RemoveErrorListeners();
                //lexer.ErrorListeners.Add(errorListener);
                parser.RemoveErrorListeners();
                parser.AddErrorListener(errorListener);
                
                AlphaParser.ProgramContext tree = parser.program();

                // Check for errors
                if (!errorListener.HasErrors)
                {
                    // Crear una instancia de la nueva ventana
                    var consola = new Consola();
                    consola.SalidaConsola.Text = "Compilación exitosa\n" + tree.ToStringTree();
                    System.Diagnostics.Debug.WriteLine("\nImprimiendo Tree en consola " 
                                                       + tree.ToStringTree() + " \n");
                    
                    // Mostrar la ventana
                    consola.Show();
                }else
                {
                    // Crear una instancia de la nueva ventana
                    var consola = new Consola();
                    consola.SalidaConsola.Text = errorListener.ToString();
                    // Mostrar la ventana
                    consola.Show();                    
                }
                

            }
            catch (Exception exception)
            {
                // Crear una instancia de la nueva ventana
                var consola = new Consola();
                consola.SalidaConsola.Text = "No hay archivo";
                consola.SalidaConsola.Text = exception.ToString();
                // Mostrar la ventana
                consola.Show(); 
                System.Diagnostics.Debug.WriteLine(exception);
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
            catch (Exception exception )
            {
                System.Diagnostics.Debug.WriteLine(exception);
                throw;
            }
        }
        private void Exit_Button_Click(object? sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Exit Button Clicked");
            Application.Current.Shutdown();
        }
    }


}
