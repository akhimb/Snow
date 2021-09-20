using System;
using System.Windows;
using System.IO;
using Newtonsoft.Json;

namespace SnowTrial1
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings
    {
        public int NumberOfRows { get; set; }
        public int NumberOfColumns { get; set; }
        public int BlurRadiusValue { get; set; }
        public double MagnificationValue { get; set; }
        public double OpacityValue { get; set; }

        
        string myJsonFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Snow.json");
        public Settings()
        {
            InitializeComponent();
            DataContext = this;
            try
            {
                if (File.Exists(myJsonFile))
                {
                    var json = File.ReadAllText(myJsonFile);
                    Variables values = JsonConvert.DeserializeObject<Variables>(json);
                    this.NumberOfRows = values.NumberOfRows;
                    this.NumberOfColumns = values.NumberOfColumns;
                    this.BlurRadiusValue = values.BlurRadiusValue;
                    this.MagnificationValue = values.MagnificationValue;
                    this.OpacityValue = values.OpacityValue;
                }
                else
                {
                    this.NumberOfRows = 8;
                    this.NumberOfColumns = 8;
                    this.BlurRadiusValue = 45;
                    this.MagnificationValue = 2;
                    this.OpacityValue = 0.8;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message,"Snow-Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
         
            
            
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var tempJsonClass = new Variables();
                tempJsonClass.NumberOfRows = this.NumberOfRows;
                tempJsonClass.NumberOfColumns = this.NumberOfColumns;
                tempJsonClass.BlurRadiusValue = this.BlurRadiusValue;
                tempJsonClass.MagnificationValue = this.MagnificationValue;
                tempJsonClass.OpacityValue = this.OpacityValue;
                using (StreamWriter sw = new StreamWriter(myJsonFile))
                {
                    string json = JsonConvert.SerializeObject(tempJsonClass);
                    sw.Write(json);
                    sw.Flush();
                }

                if (File.Exists(myJsonFile))
                {
                    MessageBox.Show("Values updated successfully","Snow-Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception e1)
            {

                MessageBox.Show(e1.Message, "Snow-Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.NumberOfRows = 8;
                this.NumberOfColumns = 8;
                this.BlurRadiusValue = 45;
                this.MagnificationValue = 2;
                this.OpacityValue = 0.8;
                MessageBox.Show("Values resetted successfully", "Snow-Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception e1)
            {

                MessageBox.Show(e1.Message, "Snow-Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key.ToString() == "Escape")
            {
                this.Close();
            }
        }
    }
}
