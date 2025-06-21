using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;




namespace PixelArtEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private int pixelSize = 13;
        private int pixels_placed = 0;
        private bool isDrawing = false;
        private HashSet<int[]> mySet = new HashSet<int[]>(new IntArrayComparer());
        private readonly DispatcherTimer _timer = new();
        private readonly Brush[] _colors = new Brush[]

        {
        Brushes.Red, Brushes.Green, Brushes.Blue, Brushes.Orange, Brushes.Purple};
        private int _colorIndex = 0; // Index for the current color in the array
        public Brush selected_color = Brushes.Red; // Default boja

        public MainWindow() // when the application starts
        {

            InitializeComponent();
            DrawGrid();
            PixelCanvas.MouseDown += Canvas_MouseDown;
            PixelCanvas.MouseMove += Canvas_MouseMove;
            PixelCanvas.MouseUp += Canvas_MouseUp;
        }
        private void DrawPixel(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(PixelCanvas); // get the position of the mouse click (relative to the canvas)
            int x = (int)(position.X / pixelSize) * pixelSize; // calculate the x coordinate of the pixel
            int y = (int)(position.Y / pixelSize) * pixelSize; // calculate the y coordinate of the pixel
            int[] cooridantes = { x, y };
            if (mySet.Contains(cooridantes) == false && e.ButtonState == e.LeftButton) // Provjerava jel su koordinate vec u setu
            {
                CheckColor();



                Rectangle pixel = new Rectangle // create a rectangle to represent the pixel
                {
                    Width = pixelSize, // set the width of the rectangle
                    Height = pixelSize, // set the height of the rectangle
                    Fill = selected_color, // set the color of the rectangle
                    Stroke = Brushes.Black, // set the border color of the rectangle
                    StrokeThickness = 0 // set the border thickness of the rectangle
                };
                Canvas.SetLeft(pixel, x); // set the x coordinate of the rectangle
                Canvas.SetTop(pixel, y); // set the y coordinate of the rectangle
                PixelCanvas.Children.Add(pixel); // add the rectangle to the canvas
                pixels_placed++;
                mySet.Add(cooridantes);
            }
        }
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e) 
        {
            isDrawing = true;
            DrawPixel(sender, e);

        }
        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e) 
        {
            isDrawing = false;
        }
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing && e.LeftButton == MouseButtonState.Pressed)
            {
                // Convert MouseEventArgs to MouseButtonEventArgs before passing to DrawPixel
                var mouseButtonEventArgs = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left)
                {
                    RoutedEvent = MouseLeftButtonDownEvent,
                    Source = e.Source
                };
                DrawPixel(sender, mouseButtonEventArgs);
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
        private void CheckColor() //Provjerava boju selektiranu na color pickeru i onda je primjenjuje na brush
        {
            var picker_selected_color = ColorPickerButton.SelectedColor;

            if (picker_selected_color.HasValue)
            {
                selected_color = new SolidColorBrush(picker_selected_color.Value);
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