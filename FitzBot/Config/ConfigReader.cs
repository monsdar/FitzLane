using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Json;

namespace FitzBot
{
    class ConfigReader
    {
        List<Lane> laneList = new List<Lane>();

        public ConfigReader(string givenFile)
        {
            //if the file does not exist do not attempt to read anything
            if(!File.Exists(givenFile))
            {
                return;
            }
            
            using (FileStream filestream = File.OpenRead(givenFile))
            {
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(LanesContainer));
                LanesContainer lanesCont = (LanesContainer)jsonSerializer.ReadObject(filestream);
                laneList = lanesCont.laneList;
            }
        }

        public List<Lane> getLanes()
        {
            return laneList;
        }
    }
}
