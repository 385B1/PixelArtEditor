﻿using System.ComponentModel;
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
        public double pixelSize = 13;
        private int pixels_placed = 0;
        private bool isDrawing = false;
        private HashSet<double[]> mySet = new HashSet<double[]>(new DoubleArrayComparer());
        private readonly DispatcherTimer _timer = new();
        private string draw_state = "draw"; // Deafult state za canvas je draw
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
        private void Create_Pixel(double[] cooridantes, double x, double y) // Kod za kreiranje pixela
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
        private void Delete_Pixel(double[] cooridantes, double x, double y) // Kod za brisanje pixela
        {
            foreach (var child in PixelCanvas.Children)
            {
                if (child is Rectangle rectangle)
                {
                    double left = Canvas.GetLeft(rectangle);
                    double top = Canvas.GetTop(rectangle);
                    if (left == x && top == y)
                    {
                        PixelCanvas.Children.Remove(rectangle);
                        mySet.Remove(cooridantes);
                        break;
                    }
                }
            }
        }
        private List<Rectangle> GetConnectedRectangles(double startX, double startY)
        {
            // DFS alogritam za listu svih spojenih rectangleova, ovo je Copilot napisao.
            var result = new List<Rectangle>();
            var visited = new HashSet<(double, double)>();
            var queue = new Queue<(double, double)>();

            // Get the starting rectangle and its color
            Rectangle startRect = null;
            Color? targetColor = null;
            foreach (var child in PixelCanvas.Children)
            {
                if (child is Rectangle rect)
                {
                    double left = Canvas.GetLeft(rect);
                    double top = Canvas.GetTop(rect);
                    if (left == startX && top == startY)
                    {
                        startRect = rect;
                        if (rect.Fill is SolidColorBrush brush)
                            targetColor = brush.Color;
                        break;
                    }
                }
            }
            if (startRect == null || targetColor == null)
                return result;

            queue.Enqueue((startX, startY));
            visited.Add((startX, startY));

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();

                // Find the rectangle at (x, y)
                Rectangle currentRect = null;
                foreach (var child in PixelCanvas.Children)
                {
                    if (child is Rectangle rect)
                    {
                        double left = Canvas.GetLeft(rect);
                        double top = Canvas.GetTop(rect);
                        if (left == x && top == y)
                        {
                            currentRect = rect;
                            break;
                        }
                    }
                }
                if (currentRect == null)
                    continue;

                result.Add(currentRect);

                // Check 4 neighbors (up, down, left, right)
                double[] dx = { -pixelSize, pixelSize, 0, 0 };
                double[] dy = { 0, 0, -pixelSize, pixelSize };

                for (int i = 0; i < 4; i++)
                {
                    double nx = x + dx[i];
                    double ny = y + dy[i];
                    if (visited.Contains((nx, ny)))
                        continue;

                    // Find neighbor rectangle and check color
                    foreach (var child in PixelCanvas.Children)
                    {
                        if (child is Rectangle neighborRect)
                        {
                            double left = Canvas.GetLeft(neighborRect);
                            double top = Canvas.GetTop(neighborRect);
                            if (left == nx && top == ny && neighborRect.Fill is SolidColorBrush neighborBrush && neighborBrush.Color == targetColor)
                            {
                                queue.Enqueue((nx, ny));
                                visited.Add((nx, ny));
                                break;
                            }
                        }
                    }
                }
            }

            return result;
        }
        private void Fill_Bucket(double x, double y, Brush color)
        {
            Color? targetColor = null;
            if (color is SolidColorBrush solidBrush)
                targetColor = solidBrush.Color;

            if (targetColor == null)
                return;
            Brush color_selected = Brushes.Transparent;
            if (ColorPickerButton.SelectedColor.HasValue)
            {
                color_selected = new SolidColorBrush(ColorPickerButton.SelectedColor.Value);
            }

            List<Rectangle> neighbors = GetConnectedRectangles(x, y);
            foreach (var child in neighbors)
            {
                    double left = Canvas.GetLeft(child);
                    double top = Canvas.GetTop(child);
                    double[] cooridantes = { left, top };
                    PixelCanvas.Children.Remove(child);
                    mySet.Remove(cooridantes);
            }
            foreach (var child in neighbors)
            {
                double[] coordinates = { Canvas.GetLeft(child), Canvas.GetTop(child) };
                Create_Pixel(coordinates, coordinates[0], coordinates[1]);
            }
            
        }
        private void DrawPixel(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(PixelCanvas); // get the position of the mouse click (relative to the canvas)
            double x = (int)(position.X / pixelSize) * pixelSize; // calculate the x coordinate of the pixel
            double y = (int)(position.Y / pixelSize) * pixelSize; // calculate the y coordinate of the pixel
            double[] cooridantes = { x, y };
            Brush rectangleColor = GetRectangleColor(x, y);

            if (draw_state == "draw")
            {
                if (mySet.Contains(cooridantes) == false && e.ButtonState == e.LeftButton) // Provjerava jel su koordinate vec u setu
                {
                    Create_Pixel(cooridantes, x, y);
                }
                else if (mySet.Contains(cooridantes) && e.ButtonState == e.LeftButton)
                {
                    Delete_Pixel(cooridantes, x, y);
                }
                Create_Pixel(cooridantes, x, y);

            }
            else if (draw_state == "erase") 
            {
                Delete_Pixel(cooridantes, x, y);
            }
            else if (draw_state == "bucket_fill")
            {
                Fill_Bucket(x, y, rectangleColor);
                Fill_Bucket(x, y, rectangleColor);

                // Dva puta radim call zato sto ako samo jedanput iz
                // nekog razloga ne radi, ovo je najbolji bug fix koji sam napravio u zivotu...

            }
        }
    
    


        private Brush GetRectangleColor(double x, double y)
        {
            foreach (var child in PixelCanvas.Children) {
            if (child is Rectangle rectangle)
                {
                    double left = Canvas.GetLeft(rectangle);
                    double top = Canvas.GetTop(rectangle);
                    if (left == x && top == y)
                    {
                        return rectangle.Fill; // Return the color of the rectangle
                    }
                }
            }
            return null;
        }
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e) 
        {
            isDrawing = true;
            DrawPixel(sender, e);
            if (draw_state == "color_dropper") // Logika za stavljanje kliknute boje na ColorPickerButton
            {
                Brush get_pixel_color = ColorDisplay.Fill;
                if (get_pixel_color is SolidColorBrush solidColorBrush)
                {
                    Color color = solidColorBrush.Color;
                    ColorPickerButton.SelectedColor = color;
                }
                if (get_pixel_color != null)
                {
                    Debug.WriteLine("Color of the pixel selected: " + get_pixel_color.ToString());
                }
            }
            
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
            else if (draw_state== "color_dropper")
            {
                Point position = e.GetPosition(PixelCanvas); 
                double x = (int)(position.X / pixelSize) * pixelSize;
                double y = (int)(position.Y / pixelSize) * pixelSize;
                Brush rectangleColor = GetRectangleColor(x, y);
                ColorDisplay.Fill = rectangleColor;
            }
        }


        private void DrawGrid()
        {

            for (double x = 0; x <= PixelCanvas.Width; x += pixelSize) // draw vertical lines
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

            for (double y = 0; y <= PixelCanvas.Height; y += pixelSize) // draw horizontal lines
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
        internal class DoubleArrayComparer : IEqualityComparer<double[]>
        {
            public bool Equals(double[] x, double[] y)
            {
                if (x == null || y == null)
                    return x == y;
                return x.SequenceEqual(y);
            }

            public int GetHashCode(double[] obj)
            {
                if (obj == null) return 0;
                unchecked
                {
                    int hash = 17;
                    foreach (double item in obj)
                        hash = hash * 31 + (int)item;
                    return hash;
                }
            }

        }

        private void Eraser_Click(object sender, RoutedEventArgs e)
        {
            draw_state = "erase";
            DebugTB.Text = "Erase mode selected";
        }

        private void Brush_Click(object sender, RoutedEventArgs e)
        {
            draw_state = "draw";
            DebugTB.Text = "Draw mode selected";
        }

        private void ColorDropper_Click(object sender, RoutedEventArgs e)
        {
            draw_state = "color_dropper";
            DebugTB.Text = "Color dropper mode selected";
        }

        private void BucketFill_Click(object sender, RoutedEventArgs e)
        {
            draw_state= "bucket_fill";
            DebugTB.Text = "Bucket fill mode selected";
        }
    }
}