using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Collections.Generic;
using System.Windows;
using FitzLaneConfig;
using FitzLanePlugin;
using FitzLanePlugin.Interfaces;

namespace FitzLane
{
    public delegate void ConfigureLaneWindowEventHandler(int laneIndex, Lane laneCfg);

    /// <summary>
    /// Interaction logic for ConfigureLaneWindow.xaml
    /// </summary>
    public partial class ConfigureLaneWindow : Window
    {
        public event ConfigureLaneWindowEventHandler OnOk;
        PlayerProviderLoader playerLoader = null;
        private int laneIndex;

        public ConfigureLaneWindow(PlayerProviderLoader givenPlayerLoader, int givenIndex = -1)
        {
            InitializeComponent();

            laneIndex = givenIndex;
            playerLoader = givenPlayerLoader;
            foreach (string name in playerLoader.GetPlayerNames())
            {
                typeComboBox.Items.Add(name);
            }
        }

        private void button_Ok_Click(object sender, RoutedEventArgs e)
        {
            Lane laneCfg = new Lane();
            laneCfg.laneIndex = laneIndex;
            laneCfg.isMainPlayer = (bool)mainPlayerCheckBox.IsChecked;
            laneCfg.ergId = nameTextBox.Text;
            laneCfg.playerType = typeComboBox.Text;


            List<IPlayerProvider> possiblePlayers = playerLoader.GetPlayerProvider();
            foreach (IPlayerProvider provider in possiblePlayers)
            {
                if(provider.IsValidPlayertype(laneCfg.playerType))
                {
                    laneCfg.playerConfig = provider.GetDefaultPlayerConfig();
                }
            }

            OnOk(laneIndex, laneCfg);

            this.Close();
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
