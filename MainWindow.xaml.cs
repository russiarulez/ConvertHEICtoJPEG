using ImageMagick;
using Ookii.Dialogs.Wpf;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace ConvertHEICtoJPEG
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnRun_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtFolderPath.Text))
            {
                btnSelFolder.IsEnabled = false;
                btnRun.IsEnabled = false;

                Progress<int> progress = new Progress<int>(value =>
                {
                    progressBar.Value = value;
                    lblPercentage.Content = $"{value}%";
                });

                //Get all HEIC images in selected directory
                string[] arrFiles = Directory.GetFiles(txtFolderPath.Text, "*.heic", SearchOption.AllDirectories);

                if (arrFiles.Length > 0)
                {
                    await Task.Run(() => ConvertImages(arrFiles, progress));

                    lblPercentage.Content = "Completed!";

                    btnSelFolder.IsEnabled = true;
                    btnRun.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show("Couldn't find any HEIC images in selected folder.", "No HEIC images found", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        // Get path to the folder with HEIC images
        private void btnSelFolder_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
            if (dlg.ShowDialog(this).GetValueOrDefault())
            {
                txtFolderPath.Text = dlg.SelectedPath;
            }
        }

        void ConvertImages(string[] arrFiles, IProgress<int> progress)
        {
            int counter = 1;
            int totalCount = arrFiles.Length;

            foreach (string filePath in arrFiles)
            {
                using (MagickImage image = new MagickImage(filePath))
                {
                    // Sets the output format to jpeg
                    image.Format = MagickFormat.Jpeg;
                    image.Write(filePath + ".jpeg");

                    var percentComplete = (counter * 100) / totalCount;
                    progress.Report(percentComplete);
                }
                counter++;
            }
        }
    }
}
