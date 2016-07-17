using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Json;

namespace FitzBot
{
    class ConfigWriter
    {
        public ConfigWriter(string givenFile)
        {}

        public static void WriteConfig(string givenFile, List<Lane> givenLanes)
        {
            LanesContainer lanesCont = new LanesContainer();
            lanesCont.laneList = givenLanes;

            if (File.Exists(givenFile))
            {
                File.Delete(givenFile);
            }
            using (FileStream filestream = File.Create(givenFile))
            {
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(LanesContainer));
                jsonSerializer.WriteObject(filestream, lanesCont);
            }
        }

        public static void WriteDefaultConfig(string givenFile)
        {
            Lane lane0 = getDefaultBotCfg("Bot1", 0, 120, 20);
            Lane lane1 = getDefaultBotCfg("Bot2", 1, 119, 22);
            Lane lane2 = getDefaultBotCfg("Ingo", 2, 121, 18, true);
            Lane lane3 = getDefaultBotCfg("Bot3", 3, 121, 21);
            Lane lane4 = getDefaultBotCfg("Bot4", 4, 115, 15);

            List<Lane> lanes = new List<Lane>();
            lanes.Add(lane0);
            lanes.Add(lane1);
            lanes.Add(lane2);
            lanes.Add(lane3);
            lanes.Add(lane4);
            WriteConfig(givenFile, lanes);
        }

        private static Lane getDefaultBotCfg(string name, int laneIndex, uint pace=120, uint spm=20, bool isMainPlayer=false)
        {
            Lane newLane = new Lane();
            newLane.laneIndex = laneIndex;
            newLane.isMainPlayer = isMainPlayer;
            newLane.playerType = typeof(BotConstant).Name;
            newLane.ergId = name;

            BotConstantConfig botCfg = new BotConstantConfig();
            botCfg.name = name;
            botCfg.pace = pace;
            botCfg.spm = spm;

            MemoryStream memStream = new MemoryStream();
            DataContractJsonSerializer botSerializer = new DataContractJsonSerializer(typeof(BotConstantConfig));
            botSerializer.WriteObject(memStream, botCfg);
            newLane.playerConfig = Encoding.UTF8.GetString(memStream.ToArray());

            return newLane;
        }
    }
}
