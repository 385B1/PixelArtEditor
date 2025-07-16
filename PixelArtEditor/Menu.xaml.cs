using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace PixelArtEditor
{
    public class RectangleData
    {
        public double X { get; set; }
        public double Y { get; set; }
        // public double Width { get; set; }
        // public double Height { get; set; }
        public string FillColor { get; set; }
    }
    public partial class MainWindow
    {
        public void Clear()
        {
            PixelCanvas.Children.Clear();
            mySet.Clear();
            pixels_placed = 0;
            DrawGrid();
        }
        private void New_Click(object sender, RoutedEventArgs e)
        {
            Clear();
            step_count = -1; // Reset step count
            for (int i = 0; i < 20; i++) // Initialize the list with empty sets
            {
                pixels[i] = new HashSet<Pixel?>(new PixelComparer());
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            LoadCanvasFromFile();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveCanvasToFile();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Closes the app
        }

        private void Res_64(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Everything drawn will be cleared. Do you want to proceed",
            "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                PixelCanvas.Children.Clear();
                mySet.Clear();
                pixels_placed = 0;
                pixelSize = 6.5;
                DrawGrid();
            }
        }

        private void Res_32(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Everything drawn will be cleared. Do you want to proceed",
            "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                PixelCanvas.Children.Clear();
                mySet.Clear();
                pixels_placed = 0;
                pixelSize = 13;
                DrawGrid();
            }
        }

        private void Res_16(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Everything drawn will be cleared. Do you want to proceed",
            "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                PixelCanvas.Children.Clear();
                mySet.Clear();
                pixels_placed = 0;
                pixelSize = 26;
                DrawGrid();
            }
        }

        private void Custom(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Everything drawn will be cleared. Do you want to proceed",
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                CustomResolution customResolution = new CustomResolution();
                customResolution.Owner = this; // Set the owner of the custom resolution window to this main window
                customResolution.WindowStartupLocation = WindowStartupLocation.CenterOwner; // Center the custom resolution window over the main window
                customResolution.ShowDialog(); // Show the custom resolution window as a dialog
            }
        }

        private void SaveCanvasToFile()
        {
            var dataList = new List<RectangleData>();

            foreach (var child in PixelCanvas.Children)
            {
                if (child is Rectangle rect)
                {
                    var color = (rect.Fill as SolidColorBrush)?.Color.ToString() ?? "#00000000";
                    dataList.Add(new RectangleData
                    {
                        X = Canvas.GetLeft(rect),
                        Y = Canvas.GetTop(rect),
                        FillColor = color
                    });
                }
            }
            string json = JsonSerializer.Serialize(dataList, new JsonSerializerOptions { WriteIndented = true });

            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                Title = "Save Pixel Data",
                FileName = "pixel_data.json"
            };

            if (saveDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveDialog.FileName, json);
            }
        }

        private void PlaceChildren()
        {
            foreach (var Pixel in pixels[step_count]) // Iterate through the previous pixels
            {
                Rectangle pixel = new Rectangle // create a rectangle to represent the pixel
                {
                    Width = pixelSize, // set the width of the rectangle
                    Height = pixelSize, // set the height of the rectangle
                    Fill = Pixel.Color, // set the color of the rectangle
                    Stroke = Brushes.Black, // set the border color of the rectangle
                    StrokeThickness = 0 // set the border thickness of the rectangle
                };
                Canvas.SetLeft(pixel, Pixel.X); // set the x coordinate of the rectangle
                Canvas.SetTop(pixel, Pixel.Y); // set the y coordinate of the rectangle
                PixelCanvas.Children.Add(pixel); // add the rectangle to the canvas
                pixels_placed++;
                mySet.Add(new double[] { Pixel.X, Pixel.Y });
            }
        }

        private void SaveAsPng_Click(object sender, RoutedEventArgs e)
        {


            PixelCanvas.Children.Clear();
            mySet.Clear();
            pixels_placed = 0;

            PlaceChildren(); // Place the children on the canvas before rendering

            PixelCanvas.Measure(new Size(PixelCanvas.ActualWidth, PixelCanvas.ActualHeight));
            PixelCanvas.Arrange(new Rect(new Size(PixelCanvas.ActualWidth, PixelCanvas.ActualHeight)));

            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                (int)PixelCanvas.ActualWidth, 
                (int)PixelCanvas.ActualHeight, 
                96d, 96d, 
                PixelFormats.Pbgra32);


            renderBitmap.Render(PixelCanvas);

            Clear();

            PlaceChildren();

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "PNG files (*.png)|*.png|All files (*.*)|*.*",
                Title = "Save Pixel Art as PNG",
                FileName = "pixel_art.png"
            };

            if (saveDialog.ShowDialog() == true)
            {
                using (FileStream file = File.Create(saveDialog.FileName))
                {
                    encoder.Save(file);
                }
            }
        }

        private void LoadCanvasFromFile()
        {
            OpenFileDialog openDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*"
            };

            if (openDialog.ShowDialog() == true)
            {
                string json = File.ReadAllText(openDialog.FileName);
                var dataList = JsonSerializer.Deserialize<List<RectangleData>>(json);
                if (dataList != null)
                {
                    PixelCanvas.Children.Clear();
                    mySet.Clear();
                    pixels_placed = 0;
                    DrawGrid();
                    foreach (var data in dataList)
                    {
                        Rectangle pixel = new Rectangle
                        {
                            Width = pixelSize,
                            Height = pixelSize,
                            Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(data.FillColor),
                            Stroke = Brushes.Black,
                            StrokeThickness = 0
                        };
                        Canvas.SetLeft(pixel, data.X);
                        Canvas.SetTop(pixel, data.Y);
                        PixelCanvas.Children.Add(pixel);
                        mySet.Add(new[] { (double)data.X, (double)data.Y });
                        pixels_placed++;
                    }
                }
            }
        }
    }   
}
