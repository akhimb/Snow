using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;
using System.Linq;
namespace PrivacySnowDog
{
       
    public partial class MainWindow : INotifyPropertyChanged
    {

        #region Properties
        private System.Windows.Forms.NotifyIcon m_notifyIcon;
        private int numberOfRows;
        private int numberOfColumns;
        private int blurRadiusValue;
        private double magnificationValue;
        private double opacityValue;
        private string profileName;
        private SolidColorBrush chosenColor;
        private Profile[] ProfileArray;
        private const int PROFILE_COUNT = 5;
        private int selectedIndexValue=-1;

        public string ProfileName
        {
            get { return profileName; }
            set
            {
                if(value.Length>=3 && ProfileChoices.IndexOf(value)==-1)
                {
                    profileName = value;
                    ProfileNameTextBox.Background = null;
                }
                else
                {
                    ProfileNameTextBox.Background = Brushes.Red;
                    ProfileNameTextBox.Text = ProfileChoices[ProfileCombo.SelectedIndex].ToString();

                }
                NotifyPropertyChanged("ProfileName");
            }
        }

        public int NumberOfRows
        {
            get { return numberOfRows; }
            set {
                numberOfRows = value;
                NotifyPropertyChanged("NumberOfRows");
            }
        }

        public int NumberOfColumns
        {
            get { return numberOfColumns; }
            set
            {
                numberOfColumns = value;
                NotifyPropertyChanged("NumberOfColumns");
            }
        }

        public int BlurRadiusValue
        {
            get { return blurRadiusValue; }
            set
            {
                blurRadiusValue = value;
                NotifyPropertyChanged("BlurRadiusValue");
            }
        }

        public double MagnificationValue
        {
            get { return magnificationValue; }
            set
            {
                magnificationValue = value;
                NotifyPropertyChanged("MagnificationValue");
            }
        }

        public double OpacityValue
        {
            get { return opacityValue; }
            set
            {
                opacityValue = value;
                NotifyPropertyChanged("OpacityValue");
            }
        }

        public SolidColorBrush ChosenColor
        {
            get { return chosenColor; }
            set
            {
                chosenColor = value;
                NotifyPropertyChanged("ChosenColor");
            }
        }

        #endregion


        string myJsonFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Snow.json");
        private List<SolidColorBrush> ColorChoices;
        private List<string> ProfileChoices=new List<string>();
        public MainWindow()
        {

            InitializeComponent();
            FillColors();
            if (ProfileChoices.Count==0)
            {
                ReadJsonFile();
            }
            DataContext = this;
            
            SetNotifyProperties();
        }

        private void SetNotifyProperties()
        {
            m_notifyIcon = new System.Windows.Forms.NotifyIcon();
            m_notifyIcon.BalloonTipText = "The Snow has been minimised. Click the tray icon to show.";
            m_notifyIcon.BalloonTipTitle = "Snow";
            m_notifyIcon.Text = "Snow";
            m_notifyIcon.Icon = Properties.Resources.logo;
            m_notifyIcon.Click += new EventHandler(m_notifyIcon_Click);
        }

        private void FillColors()
        {
            ColorChoices = new List<SolidColorBrush>();
            ColorChoices.Add(Brushes.Snow);
            ColorChoices.Add(Brushes.Black);
            ColorChoices.Add(Brushes.Blue);
            ColorChoices.Add(Brushes.Red);
            ColorChoices.Add(Brushes.Green);
            ColorChoices.Add(Brushes.Yellow);
            ColorChoices.Add(Brushes.Orange);
            ColorChoices.Add(Brushes.Transparent);
            ColorsCombo.ItemsSource = ColorChoices;
        }





        public void RenderElements()
        {
            try
            {
                ReadJsonFile();
                if (MainGrid.Children.Count == 0)
                {
                    CreateOverlays();
                }
                else
                {
                    MainGrid.Children.Clear();
                    CreateOverlays();
                }
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message, "Snow-Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReadJsonFile()
        {
            try
            {
                if (File.Exists(myJsonFile))
                {
                    Profile selectedProfile;
                    var json = File.ReadAllText(myJsonFile);
                    ProfileArray = JsonConvert.DeserializeObject<Profile[]>(json);
                    ProfileChoices = ProfileArray.Select(x => x.Name).ToList();
                    ProfileCombo.ItemsSource = ProfileChoices;
                    if (ProfileCombo.SelectedIndex == -1)
                        ProfileCombo.SelectedIndex = selectedIndexValue;

                    if (isExistItem(ProfileArray))
                        selectedProfile = ProfileArray.Where(x => x.Name == ProfileChoices[ProfileCombo.SelectedIndex].ToString()).FirstOrDefault();
                    else
                    {
                        selectedProfile = new Profile();
                        selectedProfile.NumberOfColumns = ProfileArray[0].NumberOfColumns;
                        selectedProfile.NumberOfRows = ProfileArray[0].NumberOfRows;
                        selectedProfile.BlurRadiusValue = ProfileArray[0].BlurRadiusValue;
                        selectedProfile.MagnificationValue = ProfileArray[0].MagnificationValue;
                        selectedProfile.OpacityValue = ProfileArray[0].OpacityValue;
                        if (ProfileCombo.SelectedIndex != -1)
                            selectedProfile.Name = ProfileChoices[ProfileCombo.SelectedIndex].ToString();
                        else
                            selectedProfile.Name = ProfileChoices[0].ToString();
                        selectedProfile.ChosenColorValue = ColorChoices[ColorsCombo.SelectedIndex];
                    }
                    ProfileName = selectedProfile.Name;
                    this.NumberOfColumns = selectedProfile.NumberOfColumns;
                    this.NumberOfRows = selectedProfile.NumberOfRows;
                    this.BlurRadiusValue = selectedProfile.BlurRadiusValue;
                    this.MagnificationValue = selectedProfile.MagnificationValue;
                    this.OpacityValue = selectedProfile.OpacityValue;
                    this.ChosenColor = selectedProfile.ChosenColorValue;
                    this.ColorsCombo.SelectedIndex = getIndexOfColor(selectedProfile.ChosenColorValue);

                }
                else
                {
                    this.NumberOfRows = 8;
                    this.NumberOfColumns = 8;
                    this.BlurRadiusValue = 45;
                    this.MagnificationValue = 2;
                    this.OpacityValue = 0.8;
                    ProfileArray = new Profile[PROFILE_COUNT];
                    ProfileChoices = new List<string>();
                    for (int i = 0; i < PROFILE_COUNT; i++)
                    {
                        Profile tempProfile = new Profile();
                        tempProfile.Name = "Profile" + i.ToString();
                        ProfileChoices.Add(tempProfile.Name);
                        tempProfile.NumberOfRows = this.NumberOfRows;
                        tempProfile.NumberOfColumns = this.NumberOfColumns;
                        tempProfile.BlurRadiusValue = this.BlurRadiusValue;
                        tempProfile.MagnificationValue = this.MagnificationValue;
                        tempProfile.OpacityValue = this.OpacityValue;
                        if (ColorsCombo.SelectedIndex != -1)
                            tempProfile.ChosenColorValue = ColorChoices[ColorsCombo.SelectedIndex];
                        else
                            tempProfile.ChosenColorValue = ColorChoices[0];
                        ProfileArray[i] = tempProfile;
                    }
                    ProfileCombo.ItemsSource = ProfileChoices;
                    profileName = ProfileChoices[0];
                    using (StreamWriter sw = new StreamWriter(myJsonFile))
                    {
                        string json = JsonConvert.SerializeObject(ProfileArray);
                        sw.Write(json);
                        sw.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
           
        }

        private bool isExistItem(Profile[] values)
        {
            bool isExist = false;
            for (int i = 0; i < values.Length; i++)
            {
                if(!isExist && ProfileCombo.SelectedIndex!=-1)
                {
                    if (values[i] != null && values[i].Name == ProfileChoices[ProfileCombo.SelectedIndex].ToString())
                        isExist = true;
                }
            }
            return isExist;
        }

        private int getIndexOfColor(SolidColorBrush chosenColorValue)
        {
            int indexValue = -1;
            for (int i = 0; i < ColorChoices.Count; i++)
            {
                if(indexValue==-1)
                {
                    if (ColorChoices[i].Color == chosenColorValue.Color)
                        indexValue = i;
                }
            }
            return indexValue;
        }

        private void UpdateJsonFile()
        {
            Profile selectedProfile;
            if (ProfileArray!=null && isExistItem(ProfileArray))
            {
                selectedProfile = ProfileArray.Where(x => x.Name == ProfileChoices[ProfileCombo.SelectedIndex].ToString()).FirstOrDefault();
                selectedProfile.NumberOfRows = this.NumberOfRows;
                selectedProfile.NumberOfColumns = this.NumberOfColumns;
                selectedProfile.BlurRadiusValue = this.BlurRadiusValue;
                selectedProfile.MagnificationValue = this.MagnificationValue;
                selectedProfile.OpacityValue = this.OpacityValue;
                if (ProfileName!=null && ProfileName.Length >= 3 &&  ProfileChoices.IndexOf(ProfileName) == -1)
                    selectedProfile.Name = ProfileName;
                else
                    selectedProfile.Name = ProfileChoices[ProfileCombo.SelectedIndex].ToString();
                selectedProfile.ChosenColorValue = ColorChoices[ColorsCombo.SelectedIndex];
            }
            else
            {
                selectedProfile = new Profile();
                selectedProfile.NumberOfRows = this.NumberOfRows;
                selectedProfile.NumberOfColumns = this.NumberOfColumns;
                selectedProfile.BlurRadiusValue = this.BlurRadiusValue;
                selectedProfile.MagnificationValue = this.MagnificationValue;
                selectedProfile.OpacityValue = this.OpacityValue;
                if (ProfileName != null && ProfileName.Length >= 3 && ProfileChoices.IndexOf(ProfileName) == -1)
                    selectedProfile.Name = ProfileName;
                else
                    selectedProfile.Name = ProfileChoices[ProfileCombo.SelectedIndex].ToString();
                selectedProfile.ChosenColorValue = ColorChoices[ColorsCombo.SelectedIndex];
                ProfileArray[ProfileCombo.SelectedIndex] = selectedProfile;
            }
           
            
            using (StreamWriter sw = new StreamWriter(myJsonFile))
            {
                string json = JsonConvert.SerializeObject(ProfileArray);
                sw.Write(json);
                sw.Flush();
            }
        }

      

        private void CreateOverlays()
        {
            if(Popup1.Visibility==Visibility.Visible)
            {
                MainGrid.Children.Clear();
            }else
            { 
            MainGrid.Rows = NumberOfRows;
            MainGrid.Columns = NumberOfColumns;
            for (int i = 0; i < NumberOfRows; i++)
            {
                for (int j = 0; j < NumberOfColumns; j++)
                {
                    var BControl = new BlurryControls.Controls.BlurryUserControl();
                    BControl.Name = "Control" + i+j;
                    BControl.Background =ChosenColor;
                    BControl.Opacity = OpacityValue;
                    BControl.BlurRadius = BlurRadiusValue;
                    BControl.Magnification = MagnificationValue;
                    BControl.Visibility = Visibility.Visible;
                    BControl.MouseEnter += BControl_MouseEnter;
                    BControl.SetValue(Grid.RowProperty, i);
                    BControl.SetValue(Grid.ColumnProperty, j);

                    MainGrid.Children.Add(BControl);
                }
               
            }
            }
        }

        private void CreateDemoOverlays()
        {
            DemoGrid.Children.Clear();
            DemoGrid.Rows = NumberOfRows;
            DemoGrid.Columns = NumberOfColumns;
            this.ChosenColor = ColorChoices[ColorsCombo.SelectedIndex];
            for (int i = 0; i < NumberOfRows; i++)
            {
                for (int j = 0; j < NumberOfColumns; j++)
                {
                    var BControl = new BlurryControls.Controls.BlurryUserControl();
                    BControl.Name = "Control" + i + j;
                    BControl.Background = ChosenColor;
                    BControl.Opacity = OpacityValue;
                    BControl.BlurRadius = BlurRadiusValue;
                    BControl.Magnification = MagnificationValue;
                    BControl.Visibility = Visibility.Visible;
                    BControl.MouseEnter += BControl_MouseEnter1;
                    BControl.BorderThickness = new Thickness(1.0);
                    BControl.BorderBrush = Brushes.Silver;
                    BControl.SetValue(Grid.RowProperty, i);
                    BControl.SetValue(Grid.ColumnProperty, j);

                    DemoGrid.Children.Add(BControl);
                }

            }
        }

        private void BControl_MouseEnter1(object sender, MouseEventArgs e)
        {
            try
            {
                var BControlSelected = (sender as BlurryControls.Controls.BlurryUserControl);
                for (int i = 0; i < DemoGrid.Children.Count; i++)
                {
                    var BControl = (DemoGrid.Children[i] as BlurryControls.Controls.BlurryUserControl);
                    if (BControl.Name.ToString() != BControlSelected.Name.ToString())
                    {
                        BControl.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        BControl.Visibility = Visibility.Hidden;
                    }

                }
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message, "Snow-Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BControl_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                var BControlSelected = (sender as BlurryControls.Controls.BlurryUserControl);
                for (int i = 0; i < MainGrid.Children.Count; i++)
                {
                    var BControl = (MainGrid.Children[i] as BlurryControls.Controls.BlurryUserControl);
                    if (BControl.Name.ToString() != BControlSelected.Name.ToString())
                    {
                        BControl.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        BControl.Visibility = Visibility.Hidden;
                    }

                }
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message, "Snow-Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            

        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key.ToString()=="Escape")
            {
                this.Close();
            }
        }




        private void Close_Click(object sender, RoutedEventArgs e)
        {
           
            this.Close();
        }



        private void ColorsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //RenderElements();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {

            Window window = (Window)sender;
            window.Topmost = true;
        }

  
        private WindowState m_storedWindowState = WindowState.Maximized;
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                if (m_notifyIcon != null)
                    m_notifyIcon.ShowBalloonTip(2000);
            }
            else
                m_storedWindowState = WindowState;
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CheckTrayIcon();
        }

        void m_notifyIcon_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = m_storedWindowState;
        }

    void CheckTrayIcon()
        {
            ShowTrayIcon(!IsVisible);
        }

        void ShowTrayIcon(bool show)
        {
            if (m_notifyIcon != null)
                m_notifyIcon.Visible = show;
        }


        public event PropertyChangedEventHandler PropertyChanged;


        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Popup_Click(object sender, RoutedEventArgs e)
        {
            if (Popup1.Visibility == Visibility.Visible)
            {
                selectedIndexValue = ProfileCombo.SelectedIndex;
                Popup1.Visibility = Visibility.Hidden;
                ReadJsonFile();
                CreateOverlays();
            }
            else
                 {
                selectedIndexValue = ProfileCombo.SelectedIndex;
                ReadJsonFile();

                CreateDemoOverlays();
                Popup1.Visibility = Visibility.Visible;
                CreateOverlays();
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
                this.ProfileName = ProfileChoices[ProfileCombo.SelectedIndex].ToString();
                this.ChosenColor = ColorChoices[ColorsCombo.SelectedIndex];
                UpdateJsonFile();

                if (File.Exists(myJsonFile))
                {
                    MessageBox.Show("Values resetted successfully", "Snow-Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    Popup1.Visibility = Visibility.Hidden;
                    RenderElements();
                }
               
            }
            catch (Exception e1)
            {

                MessageBox.Show(e1.Message, "Snow-Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateJsonFile();

                if (File.Exists(myJsonFile))
                {
                    MessageBox.Show("Values updated successfully", "Snow-Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    Popup1.Visibility = Visibility.Hidden;
                    RenderElements();
                    System.Windows.Forms.Application.DoEvents();
                }
            }
            catch (Exception e1)
            {

                MessageBox.Show(e1.Message, "Snow-Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ProfileCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
          //  ReadJsonFile();
            RenderElements();
            ProfileTextBox.Text =ProfileChoices[ ProfileCombo.SelectedIndex].ToUpper();
        }

        private void TryButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DemoGrid.Children.Count == 0)
                {
                    CreateDemoOverlays();
                }
                else
                {
                    DemoGrid.Children.Clear();
                    CreateDemoOverlays();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
