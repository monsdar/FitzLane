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
    /// <summary>
    /// Interaction logic for PlayerItem.xaml
    /// </summary>
    public partial class PlayerItem : UserControl
    {
        IBot player = null;

        public PlayerItem(IBot givenPlayer)
        {
            InitializeComponent();
            DataContext = this;
            player = givenPlayer;
        }

        public string PlayerName
        {
            get { return player.GetErg().name; }
        }


        public string PlayerType
        {
            get { return player.Name; }
        }

        public string Distance
        {
            get { return "Distance: " + player.GetErg().distance.ToString("#.00") + " m"; }
        }

        public string Pace
        {
            get { return "Pace: " + player.GetErg().paceInSecs.ToString("#.00") + " sec/500m"; }
        }
    }
}
