using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PixelArtEditor.View.Windows
{
    /// <summary>
    /// Interaction logic for ColorSelectionBox.xaml
    /// </summary>
    public partial class ColorSelectionBox : Window
    {
        PixelArtEditor.MainWindow mainWindow = (PixelArtEditor.MainWindow)Application.Current.MainWindow;

        public ColorSelectionBox()
        {
            InitializeComponent();
        }



        private void Button_Click_Black(object sender, RoutedEventArgs e)
        {
            mainWindow.selected_color = Brushes.Black; 
        }

        private void Button_Click_DarkGray(object sender, RoutedEventArgs e)
        {
            mainWindow.selected_color = Brushes.DarkGray;

        }
        private void Button_Click_LightGray(object sender, RoutedEventArgs e)
        {
            mainWindow.selected_color = Brushes.LightGray;

        }
        private void Button_Click_White(object sender, RoutedEventArgs e)
        {
            mainWindow.selected_color = Brushes.White;

        }
    }
}
