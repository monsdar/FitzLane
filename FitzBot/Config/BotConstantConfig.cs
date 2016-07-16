using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace FitzBot
{
    [DataContract]
    class BotConstantConfig
    {
        [DataMember]
        public string name = "";

        [DataMember]
        public uint pace = 120;

        [DataMember]
        public uint spm = 20;
    }
}
