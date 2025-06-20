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
        private void New_Click(object sender, RoutedEventArgs e)
        {
            PixelCanvas.Children.Clear();
            mySet.Clear();
            pixels_placed = 0;
            DrawGrid();
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
                        mySet.Add(new[] { (int)data.X, (int)data.Y });
                        pixels_placed++;
                    }
                }
            }
        }
    }   
}
