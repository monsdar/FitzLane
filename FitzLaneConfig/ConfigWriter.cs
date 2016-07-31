using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;

namespace FitzLaneConfig
{
    public class ConfigWriter
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
    }
}
