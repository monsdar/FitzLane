using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace FitzBot
{
    [DataContract]
    public class Lane
    {
        /// <summary>
        ///     Unique index of lane this configuration is for.
        /// </summary>
        [DataMember]
        public int index = 0;

        /// <summary>
        ///     This flag defines if the player shall be focused by the camera.
        /// </summary>
        [DataMember]
        public bool isMainPlayer = false;

        /// <summary>
        ///     ID of ergometer, i.e. it must fit to the ergId in network message.
        /// </summary>
        [DataMember]
        public string ergId = "";

        /// <summary>
        ///     Defines a dependency to another Lane.
        /// </summary>
        [DataMember]
        public string parentId = "";

        /// <summary>
        ///     Type of player, i.e., what kind of instance should be created.
        /// </summary>
        [DataMember]
        public string playerType = "";

        /// <summary>
        ///     Configuration specific to the used playerType.
        /// </summary>
        [DataMember]
        public string playerConfig;
    }



    [DataContract]
    public class LanesContainer
    {
        [DataMember]
        public List<Lane> laneList = new List<Lane>();
    }
}
