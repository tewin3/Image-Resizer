using Microsoft.Win32;
using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Windows;

namespace ImageResizerApp
{
    public partial class MainWindow : Window
    {
        private string selectedImagePath = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (dialog.ShowDialog() == true)
            {
                selectedImagePath = dialog.FileName;
                MessageBox.Show("Selected: " + selectedImagePath);
            }
        }

        private void BtnResize_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedImagePath))
            {
                MessageBox.Show("Please select an image first.");
                return;
            }

            if (!int.TryParse(txtWidth.Text, out int width) ||
                !int.TryParse(txtHeight.Text, out int height))
            {
                MessageBox.Show("Invalid width or height");
                return;
            }

            try
            {
                using (var image = Image.Load(selectedImagePath))
                {
                    image.Mutate(x => x.Resize(width, height));

                    string newPath = System.IO.Path.Combine(
                        System.IO.Path.GetDirectoryName(selectedImagePath),
                        "resized_" + System.IO.Path.GetFileName(selectedImagePath)
                    );

                    image.Save(newPath);

                    MessageBox.Show("Image saved to:\n" + newPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}