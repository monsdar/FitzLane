using System.ComponentModel;
using System.Windows.Controls;
using FitzLanePlugin.Interfaces;
using System.Windows;

namespace FitzLane
{
    /// <summary>
    /// Interaction logic for PlayerItem.xaml
    /// </summary>
    public partial class PlayerItem : UserControl, INotifyPropertyChanged
    {
        IPlayer player = null;
        bool isMainPlayer = false;

        public PlayerItem(IPlayer givenPlayer, bool isMainPlayer=false)
        {
            InitializeComponent();

            DataContext = this;
            player = givenPlayer;
            this.isMainPlayer = isMainPlayer;
        }

        public void Update(IPlayer givenPlayer, bool isMainPlayer=false)
        {
            player = givenPlayer;
            this.isMainPlayer = isMainPlayer;
            RaisePropertyChanged("PlayerName");
            RaisePropertyChanged("PlayerType");
            RaisePropertyChanged("MainPlayer");
            RaisePropertyChanged("Distance");
            RaisePropertyChanged("Pace");
        }

        public string ErgId
        {
            get { return player.GetErg().ergId; }
        }
        public string PlayerName
        {
            get { return player.GetErg().name; }
        }
        public string PlayerType
        {
            get { return player.Name; }
        }
        public string MainPlayer
        {
            get
            {
                if (isMainPlayer)
                { return "Main Player"; }
                else
                { return ""; }
            }
        }
        public string Distance
        {
            get { return "Distance: " + player.GetErg().distance.ToString("#.00") + " m"; }
        }
        public string Pace
        {
            get { return "Pace: " + player.GetErg().paceInSecs.ToString("#.00") + " sec/500m"; }
        }

        //-- INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
