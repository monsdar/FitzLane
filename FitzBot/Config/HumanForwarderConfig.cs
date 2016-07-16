using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace FitzBot
{
    [DataContract]
    class HumanForwarderConfig
    {
        [DataMember]
        public string name = "";
    }
}
