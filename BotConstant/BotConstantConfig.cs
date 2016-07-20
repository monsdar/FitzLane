using System.Runtime.Serialization;

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