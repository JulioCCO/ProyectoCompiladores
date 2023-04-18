using System;
using System.Windows;
using Antlr4.Runtime;
using System.IO;
using System.Windows.Controls;
using Compilador.Components;
using generated;
using Microsoft.Win32;


namespace Compilador
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public string txtPath = "";
        MyErrorListener _errorListener = new MyErrorListener();
        MyErrorParser _errorParser = new MyErrorParser();
        MyDefaultErrorStrategy _errorStrategy = new MyDefaultErrorStrategy();
        bool textoAbierto = false;

        public MainWindow()
        {
            System.Diagnostics.Debug.WriteLine("System Diagnostics Debug");
            InitializeComponent();
        }

        private void Add_Tab_Button_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.ShowDialog();
            txtPath = openFileDialog1.FileName;
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
                    nuevoTextBox.FontSize = 14;
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

                    TextReader leer = new StreamReader(txtPath);
                    nuevoTextBox.Text = leer.ReadToEnd();
                    leer.Close();

                    // Agregar el TextBox a la nueva pestaña
                    tabItem.Content = nuevoTextBox;
                    //nuevaPestana.Items.Add(tabItem);

                    // Agregar la nueva pestaña al control de pestañas
                    Tab.Items.Add(tabItem);
                }
            }
            catch (Exception exception)
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
            int column = index -
                Pantalla.GetCharacterIndexFromLineIndex(Pantalla.GetLineIndexFromCharacterIndex(index)) + 1;

            // Actualiza el texto de un label o de otro TextBox con el número de línea y columna.
            Output.Content = $"Línea: {line} \nColumna: {column}";
        }


        public void Upload_File_Button_Click(object? sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            //openFileDialog1.Title = "Buscar archivo .txt";
            openFileDialog1.ShowDialog();
            txtPath = openFileDialog1.FileName;
            try
            {
                if (File.Exists(openFileDialog1.FileName))
                {
                    TextReader leer = new StreamReader(txtPath);
                    Pantalla.Text = leer.ReadToEnd();
                    leer.Close();
                }

                textoAbierto = true;
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception);
                throw;
            }
        }

        private void Run_Button_Click(object? sender, RoutedEventArgs e)
        {
            if (textoAbierto)
            {
                string text = Pantalla.Text;
                StreamWriter writer = new StreamWriter(txtPath);
                writer.WriteLine(text);
                writer.Close();
                _errorParser = new MyErrorParser();
                _errorListener = new MyErrorListener();
                _errorStrategy = new MyDefaultErrorStrategy();
                try
                {   
                    
                    System.Diagnostics.Debug.WriteLine("\nInformacion tomada del TextBox: \n" + text + "\n");
                    ICharStream input = CharStreams.fromString(text);
                    AlphaScanner lexer = new AlphaScanner(input);
                    CommonTokenStream tokens = new CommonTokenStream(lexer);
                    AlphaParser parser = new AlphaParser(tokens);

                    // // Add the error listener to the lexer and parser

                    lexer.RemoveErrorListeners();
                    lexer.AddErrorListener(_errorListener);

                    parser.RemoveErrorListeners();
                    parser.AddErrorListener(_errorParser);
                    parser.ErrorHandler = _errorStrategy;

                    AlphaParser.ProgramContext tree = parser.program();
                    AContextual context = new AContextual();
                    // Check for errors

                    var consola = new Consola();
                    System.Diagnostics.Debug.WriteLine("\nImprimiendo ErrorListener en consola "
                                                       + _errorListener + " \n");
                    System.Diagnostics.Debug.WriteLine("\nImprimiendo ErrorParser en consola "
                                                       + _errorParser + " \n");

                    if (_errorParser.HasErrors() == false && _errorListener.HasErrors() == false)
                    {
                        // Crear una instancia de la nueva ventana
                        consola.SalidaConsola.Text =
                            "Compilación exitosa\n\n" + tree.ToStringTree(parser) + "\n\n" +"Path del archivo:" + txtPath + "\n\n" +
                                                           "Visitando el arbol: " + context.Visit(tree) + " \n";
                        
                        System.Diagnostics.Debug.WriteLine("\nImprimiendo Tree en consola "
                                                           + tree.ToStringTree(parser) + " \n" +
                                                           "Visitando el arbol:" + context.Visit(tree) + " \n"
                                                           );
                        
                        
                        // Mostrar la ventana
                        consola.Show();
                    }
                    else
                    {
                        // Crear una instancia de la nueva ventana
                        consola.SalidaConsola.Text = "Compilación fallida\nErrores de escaner: \n"
                                                     + _errorListener + "\nErrores de parser: \n" + _errorParser;
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
                    System.Diagnostics.Debug.WriteLine(exception);
                    consola.Show();
                }
            }
            else
            {
                MessageBox.Show("No hay archivo cargado.", "Advertencia");
            }
        }

        private void Exit_Button_Click(object? sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Exit Button Clicked");
            Application.Current.Shutdown();
        }
    }
}