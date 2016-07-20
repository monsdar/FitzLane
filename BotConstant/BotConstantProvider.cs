using System;
using FitzLanePlugin.Interfaces;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

class BotConstantProvider : IPlayerProvider
{
    public IPlayer GetPlayer(string config)
    {
        return new BotConstant(config);
    }

    public string GetDefaultPlayerConfig()
    {
        BotConstantConfig botCfg = new BotConstantConfig();
        MemoryStream memStream = new MemoryStream();
        DataContractJsonSerializer botSerializer = new DataContractJsonSerializer(typeof(BotConstantConfig));
        botSerializer.WriteObject(memStream, botCfg);
        return Encoding.UTF8.GetString(memStream.ToArray());
    }

    public string GetPlayerName()
    {
        return "BotConstant";
    }

    public bool IsValidPlayertype(string playerType)
    {
        if(playerType == "BotConstant")
        {
            return true;
        }
        return false;
    }
}
