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
    public partial class MainWindow
    {
        public string texto = "";
        public MainWindow() {
            System.Diagnostics.Debug.WriteLine("System Diagnostics Debug");
            InitializeComponent();
        }
        
        private void Add_Tab_Button_Click(object sender, EventArgs e)
        {
                        
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.ShowDialog();
            texto = openFileDialog1.FileName;
            try
            {
                if (File.Exists(openFileDialog1.FileName))
                {
                    
                    TabItem tabItem = new TabItem();
                    tabItem.Header = openFileDialog1.SafeFileName;
                    

                    // Crear una instancia de TextBox
                    TextBox nuevoTextBox = new TextBox();
                    nuevoTextBox.TextChanged += Pantalla_TextChanged;
                    nuevoTextBox.SelectionChanged += Pantalla_SelectionChanged;
                    nuevoTextBox.FontSize=14;
                    nuevoTextBox.FontFamily = new System.Windows.Media.FontFamily("Consolas");
                    nuevoTextBox.Background = System.Windows.Media.Brushes.Azure;
                    nuevoTextBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                    nuevoTextBox.VerticalAlignment = VerticalAlignment.Stretch;
                    nuevoTextBox.Width = 700;
                    nuevoTextBox.Height = 350;
                    nuevoTextBox.TextWrapping = TextWrapping.Wrap;
                    nuevoTextBox.AcceptsTab = true;
                    nuevoTextBox.AcceptsReturn = true;
                    nuevoTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

                    TextReader leer = new StreamReader(texto);
                    nuevoTextBox.Text = leer.ReadToEnd();
                    leer.Close();
                    
                    // Agregar el TextBox a la nueva pestaña
                    tabItem.Content = nuevoTextBox;
                    //nuevaPestana.Items.Add(tabItem);

                    // Agregar la nueva pestaña al control de pestañas
                    Tab.Items.Add(tabItem);
                    
                }
            }
            catch (Exception exception )
            {
                System.Diagnostics.Debug.WriteLine(exception);
                throw;
            }

        }
        
        private void closeButton_Click(object sender, EventArgs e)
        {
            // Obtener el TabItem seleccionado
            TabItem? item = Tab.SelectedItem as TabItem;

            if (item?.Name == "Principal")
            {
                // Si es el TabItem que no se puede eliminar, mostrar un mensaje de advertencia
                MessageBox.Show("Este TabItem no se puede eliminar.", "Advertencia");
            }
            else
            {
                // Eliminar la pestaña seleccionada
                Tab.Items.Remove(Tab.SelectedItem);
            }
            
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
        
        private void Remove_Tab_Button_Click(object sender, EventArgs e)
        {
            // Eliminar la pestaña seleccionada
            Tab.Items.Remove(Tab.SelectedItem);
        }
        
        public void Upload_File_Button_Click(object? sender, RoutedEventArgs e) {
            
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            //openFileDialog1.Title = "Buscar archivo .txt";
            openFileDialog1.ShowDialog();
            texto = openFileDialog1.FileName;
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
        
        private void Run_Button_Click(object? sender, RoutedEventArgs e) {
            
            string text = Pantalla.Text;
            using (StreamWriter writer = new StreamWriter(texto))
            {
                writer.WriteLine(text);
            }
            try
            {
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
                lexer.RemoveErrorListeners();
                lexer.AddErrorListener(errorListener);
                
                AlphaParser.ProgramContext tree = parser.program();

                // Check for errors
                if (errorListener.HasErrors() == false)
                {
                    
                    // Crear una instancia de la nueva ventana
                    var consola = new Consola();
                    consola.SalidaConsola.Text = "Compilación exitosa\n" + tree.ToStringTree(parser);
                    System.Diagnostics.Debug.WriteLine("\nImprimiendo Tree en consola " 
                                                       + tree.ToStringTree(parser) + " \n");
                    
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
        
        private void Exit_Button_Click(object? sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Exit Button Clicked");
            Application.Current.Shutdown();
        }
    }
    
}
