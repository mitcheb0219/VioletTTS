using Dalamud.Bindings.ImGui;
using Dalamud.Game.Text;
using Dalamud.Interface.Windowing;
using Serilog;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace VioletTTS.Windows;

public class ConfigWindow : Window, IDisposable
{
    private readonly Configuration configuration;

    // We give this window a constant ID using ###.
    // This allows for labels to be dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public ConfigWindow(Plugin plugin) : base("Violet TTS Configuration###With a constant ID")
    {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(350, 325);
        SizeCondition = ImGuiCond.Always;

        configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void Draw()
    {
        // Can't ref a property, so use a local copy
        var configValue = configuration.CharacterName;
        if (ImGui.InputText("Character Name", ref configValue))
        {
            configuration.CharacterName = configValue;
            // Can save immediately on change if you don't want to provide a "Save and Close" button
            configuration.Save();
        }

        var narrator = configuration.WindowsNarrator;
        if (ImGui.RadioButton("Windows Narrator", ref narrator, true))
        {
            configuration.WindowsNarrator = narrator;
            configuration.ElevenLabsAPI = false;
            configuration.Save();
        }

        var elevenlabs = configuration.ElevenLabsAPI;
        if (ImGui.RadioButton("Eleven Labs API", ref elevenlabs, true))
        {
            configuration.ElevenLabsAPI = elevenlabs;
            configuration.WindowsNarrator = false;
            configuration.Save();
        }

        var say = configuration.Say;
        if (ImGui.Checkbox("Say", ref say))
        {
            configuration.Say = say;
            if (configuration.Say)
            {
                configuration.Channels.Add($"{XivChatType.Say}");
            }
            else
            {
                configuration.Channels.Remove($"{XivChatType.Say}");
            }
            // Can save immediately on change if you don't want to provide a "Save and Close" button
            configuration.Save();
            Log.Information("Say is " + say);
        }
        
        var cwl1 = configuration.CrossLinkShell1;
        if (ImGui.Checkbox("Cross World Linkshell 1", ref cwl1))
        {
            configuration.CrossLinkShell1 = cwl1;
            if (configuration.CrossLinkShell1)
            {
                configuration.Channels.Add($"{XivChatType.CrossLinkShell1}");
            }
            else
            {
                configuration.Channels.Remove($"{XivChatType.CrossLinkShell1}");
            }
            // Can save immediately on change if you don't want to provide a "Save and Close" button
            configuration.Save();
            Log.Information("CWL1 is " + cwl1);
        }
        
        var cwl4 = configuration.CrossLinkShell4;
        if (ImGui.Checkbox("Cross World Linkshell 4", ref cwl4))
        {
            configuration.CrossLinkShell4 = cwl4;
            if (configuration.CrossLinkShell4)
            {
                configuration.Channels.Add($"{XivChatType.CrossLinkShell4}");
            }
            else
            {
                configuration.Channels.Remove($"{XivChatType.CrossLinkShell4}");
            }
            // Can save immediately on change if you don't want to provide a "Save and Close" button
            configuration.Save();
            Log.Information("CWL4 is " + cwl4);
        }
       
        var party = configuration.Party;
        if (ImGui.Checkbox("Party", ref party))
        {
            configuration.Party = party;
            if (configuration.Party)
            {
                configuration.Channels.Add($"{XivChatType.Party}");
            }
            else
            {
                configuration.Channels.Remove($"{XivChatType.Party}");
            }
            // Can save immediately on change if you don't want to provide a "Save and Close" button
            configuration.Save();
            Log.Information("Party is " + party);
        }
        
        var crossparty = configuration.CrossParty;
        if (ImGui.Checkbox("CrossParty", ref crossparty))
        {
            configuration.CrossParty = crossparty;
            if (configuration.CrossParty)
            {
                configuration.Channels.Add($"{XivChatType.CrossParty}");
            }
            else
            {
                configuration.Channels.Remove($"{XivChatType.CrossParty}");
            }
            // Can save immediately on change if you don't want to provide a "Save and Close" button
            configuration.Save();
            Log.Information("CrossParty is " + crossparty);
        }
        
        var alliance = configuration.Alliance;
        if (ImGui.Checkbox("Alliance", ref alliance))
        {
            configuration.Alliance = alliance;
            if (configuration.Alliance)
            {
                configuration.Channels.Add($"{XivChatType.Alliance}");
            }
            else
            {
                configuration.Channels.Remove($"{XivChatType.Alliance}");
            }
            // Can save immediately on change if you don't want to provide a "Save and Close" button
            configuration.Save();
            Log.Information("Alliance is " + alliance);
        }
        
        var fc = configuration.FreeCompany;
        if (ImGui.Checkbox("FreeCompany", ref fc))
        {
            configuration.FreeCompany = fc;
            if (configuration.FreeCompany)
            {
                configuration.Channels.Add($"{XivChatType.FreeCompany}");
            }
            else
            {
                configuration.Channels.Remove($"{XivChatType.FreeCompany}");
            }
            // Can save immediately on change if you don't want to provide a "Save and Close" button
            configuration.Save();
            Log.Information("FreeCompany is " + fc);
        }

        }
    }


