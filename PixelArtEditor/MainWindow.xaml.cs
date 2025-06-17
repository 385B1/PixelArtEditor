using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PixelArtEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private int pixelSize = 20;
        public MainWindow() // when the application starts
        {
            InitializeComponent();
            DrawGrid();
            // Console.WriteLine("Skibidi");
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e) // when the mouse is clicked on the canvas
        {

        }


        private void DrawGrid() {
            
            for (int x = 0; x <= PixelCanvas.Width; x += pixelSize) // draw vertical lines
            {
                Line verticalLine = new Line
                {
                    X1 = x, // first point X coordinate
                    Y1 = 0, // first point Y coordinate
                    X2 = x, // second point X coordinate
                    Y2 = PixelCanvas.Height, // second point Y coordinate
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.5

                };
                PixelCanvas.Children.Add(verticalLine); // add the line to the canvas

            }

            for (int y = 0; y <= PixelCanvas.Height; y += pixelSize) // draw horizontal lines
            {
                Line horizontalLine = new Line
                {
                    X1 = 0,
                    Y1 = y,
                    X2 = PixelCanvas.Width,
                    Y2 = y,
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.5
                };
                PixelCanvas.Children.Add(horizontalLine);
            }
        
        }
    }
}