using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace ImageResizerApp
{
    public partial class MainWindow : Window
    {
        private List<string> selectedImages = new();
        private string outputFolder = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        // Select Images
        private void BtnSelectImages_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Images|*.jpg;*.png;*.bmp"
            };

            if (dialog.ShowDialog() == true)
            {
                LoadImages(dialog.FileNames.ToList());
            }
        }

        // Load images + preview
        private void LoadImages(List<string> files)
        {
            selectedImages = files;

            if (selectedImages.Count > 0)
            {
                imgPreview.Source = new BitmapImage(new Uri(selectedImages[0]));
            }
        }

        // Select Output Folder (WPF-safe workaround)
        private void BtnSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                CheckFileExists = false,
                ValidateNames = false,
                FileName = "Select Folder"
            };

            if (dialog.ShowDialog() == true)
            {
                outputFolder = System.IO.Path.GetDirectoryName(dialog.FileName);
                System.Windows.MessageBox.Show("Output: " + outputFolder);
            }
        }

        // Drag & Drop
        private void Window_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                var files = ((string[])e.Data.GetData(System.Windows.DataFormats.FileDrop)).ToList();
                LoadImages(files);
            }
        }

        // Resize All
        private void BtnResize_Click(object sender, RoutedEventArgs e)
        {
            if (selectedImages.Count == 0)
            {
                System.Windows.MessageBox.Show("Select images first");
                return;
            }

            if (string.IsNullOrEmpty(outputFolder))
            {
                System.Windows.MessageBox.Show("Select output folder");
                return;
            }

            if (!int.TryParse(txtWidth.Text, out int targetWidth) ||
                !int.TryParse(txtHeight.Text, out int targetHeight))
            {
                System.Windows.MessageBox.Show("Invalid dimensions");
                return;
            }

            string format = (cmbFormat.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString();
            int quality = (int)sliderQuality.Value;

            foreach (var path in selectedImages)
            {
                using (var image = SixLabors.ImageSharp.Image.Load(path))
                {
                    int newWidth = targetWidth;
                    int newHeight = targetHeight;

                    // Aspect ratio lock
                    if (chkAspect.IsChecked == true)
                    {
                        double ratio = Math.Min(
                            (double)targetWidth / image.Width,
                            (double)targetHeight / image.Height
                        );

                        newWidth = (int)(image.Width * ratio);
                        newHeight = (int)(image.Height * ratio);
                    }

                    image.Mutate(x => x.Resize(newWidth, newHeight));

                    string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                    string newPath = System.IO.Path.Combine(
                        outputFolder,
                        $"{fileName}_resized.{format}"
                    );

                    // Save with format
                    if (format == "jpg")
                    {
                        image.Save(newPath, new JpegEncoder { Quality = quality });
                    }
                    else
                    {
                        image.Save(newPath);
                    }
                }
            }

            System.Windows.MessageBox.Show("Done!");
        }
    }
}