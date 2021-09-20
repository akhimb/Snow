using System.Windows.Media;

namespace PrivacySnowDog
{
    class Profile
    {
        public string Name { get; set; }
        public int NumberOfRows { get; set; }
        public int NumberOfColumns { get; set; }
        public int BlurRadiusValue { get; set; }
        public double MagnificationValue { get; set; }
        public double OpacityValue { get; set; }
        public SolidColorBrush ChosenColorValue { get; set; }
    }
}
