using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FitzBot
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
