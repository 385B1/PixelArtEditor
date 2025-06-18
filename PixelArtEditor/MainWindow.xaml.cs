using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
        private int pixelSize = 13;
        private int pixels_placed = 0;
        private HashSet<int[]> mySet = new HashSet<int[]>(new IntArrayComparer());


        public MainWindow() // when the application starts
        {

            InitializeComponent();
            DrawGrid();
            // Console.WriteLine("Skibidi");
        }
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e) // when the mouse is clicked on the canvas
        {
            Point position = e.GetPosition(PixelCanvas); // get the position of the mouse click (relative to the canvas)
            int x = (int)(position.X / pixelSize) * pixelSize; // calculate the x coordinate of the pixel
            int y = (int)(position.Y / pixelSize) * pixelSize; // calculate the y coordinate of the pixel
            int[] cooridantes = { x, y };
            if (mySet.Contains(cooridantes) == false) // Provjerava jel su koordinate vec u setu
            {



                Rectangle pixel = new Rectangle // create a rectangle to represent the pixel
                {
                    Width = pixelSize, // set the width of the rectangle
                    Height = pixelSize, // set the height of the rectangle
                    Fill = Brushes.Red, // set the color of the rectangle
                    Stroke = Brushes.Black, // set the border color of the rectangle
                    StrokeThickness = 0 // set the border thickness of the rectangle
                };
                Canvas.SetLeft(pixel, x); // set the x coordinate of the rectangle
                Canvas.SetTop(pixel, y); // set the y coordinate of the rectangle
                PixelCanvas.Children.Add(pixel); // add the rectangle to the canvas
                tb1.Text = pixels_placed.ToString(); // Ovo sluzi samo za debuganje, ako je pixel vec postavljen i ovaj broj se ne mijenja, to znaci da ne stavlja novi pixel na taj vec postojeci pixel
                pixels_placed++;
                mySet.Add(cooridantes);
            }
        }


        private void DrawGrid()
        {

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

        internal class IntArrayComparer : IEqualityComparer<int[]>
        {
            public bool Equals(int[] x, int[] y)
            {
                if (x == null || y == null)
                    return x == y;
                return x.SequenceEqual(y);
            }

            public int GetHashCode(int[] obj)
            {
                if (obj == null) return 0;
                unchecked
                {
                    int hash = 17;
                    foreach (int item in obj)
                        hash = hash * 31 + item;
                    return hash;
                }
            }

        }
    }
}