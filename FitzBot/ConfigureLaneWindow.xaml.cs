using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using FitzLaneManager.Config;
using FitzLaneManager.Bots;

namespace FitzLaneManager
{
    public delegate void ConfigureLaneWindowEventHandler(int laneIndex, Lane laneCfg);

    /// <summary>
    /// Interaction logic for ConfigureLaneWindow.xaml
    /// </summary>
    public partial class ConfigureLaneWindow : Window
    {
        public event ConfigureLaneWindowEventHandler OnOk;
        private int laneIdx;

        public ConfigureLaneWindow(int laneIndex = -1)
        {
            laneIdx = laneIndex;

            InitializeComponent();

            typeComboBox.Items.Add(typeof(BotConstant).Name);
        }

        private void button_Ok_Click(object sender, RoutedEventArgs e)
        {
            Lane laneCfg = new Lane();
            laneCfg.laneIndex = laneIdx;
            laneCfg.isMainPlayer = (bool)mainPlayerCheckBox.IsChecked;
            laneCfg.ergId = nameTextBox.Text;
            laneCfg.playerType = typeComboBox.Text;

            if (laneCfg.playerType == typeof(BotConstant).Name)
            {
                BotConstantConfig botCfg = new BotConstantConfig();
                MemoryStream memStream = new MemoryStream();
                DataContractJsonSerializer botSerializer = new DataContractJsonSerializer(typeof(BotConstantConfig));
                botSerializer.WriteObject(memStream, botCfg);
                laneCfg.playerConfig = Encoding.UTF8.GetString(memStream.ToArray());
            }

            OnOk(laneIdx, laneCfg);

            this.Close();
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
