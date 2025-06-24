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

namespace PixelArtEditor
{
    /// <summary>
    /// Interaction logic for CustomResolution.xaml
    /// </summary>
    public partial class CustomResolution : Window
    {
        public CustomResolution()
        {
            InitializeComponent();
        }

        private void Set_value(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
           

            if (int.TryParse(Input.Text, out int value)) {
                if (value >= 1 && value <= 200)
                {
                    mainWindow.pixelSize = 416 / (double)value;

                    mainWindow.Clear();

                    this.Close(); // Closes the window
                }
                
            }
        }

    }
}
