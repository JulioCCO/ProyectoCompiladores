using System;
using System.Windows;

namespace CompiNF;

public partial class Input : Window
{
    public string valorDeEntrada { get; private set; }
    public Input(string nombre)
    {
        Title = "Entrada: " + nombre;
        InitializeComponent();
    }
    private void Aceptar_Click(object sender, EventArgs e)
    {
        valorDeEntrada = Entrada.Text;
        Entrada.Clear();
        Close();
    }
}