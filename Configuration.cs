using Dalamud.Configuration;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using Dalamud.Game.Text;

namespace VioletTTS;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;
    public bool WindowsNarrator { get; set; } = false;
    public bool ElevenLabsAPI { get; set; } = false;
    public string CharacterName { get; set; } = "Violet Ebontide";

    public List<String> Channels = [];

    public bool Say { get; set; } = true;
    public bool CrossLinkShell1 { get; set; } = true;

    public bool Party { get; set; } = false;
    public bool CrossParty { get; set; } = false;
    public bool Alliance { get; set; } = false;
    public bool FreeCompany { get; set; } = false;
    public bool CrossLinkShell4 { get; set; } = false;

    // The below exists just to make saving less cumbersome
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
