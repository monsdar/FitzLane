using System.Windows;
using System.Windows.Controls;

namespace FitzLane
{
    public delegate void PlayerAddClickedEventHandler(int laneIndex);

    /// <summary>
    /// Interaction logic for PlayerItemEmpty.xaml
    /// </summary>
    public partial class PlayerItemEmpty : UserControl
    {
        public event PlayerAddClickedEventHandler OnAdd;
        private int laneIdx;

        public PlayerItemEmpty(int laneIndex = -1)
        {
            laneIdx = laneIndex;

            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnAdd(laneIdx);
        }
    }
}
