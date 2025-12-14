using Dalamud.Game.Command;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using VioletTTS.Windows;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace VioletTTS;



public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IDataManager DataManager { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;
    [PluginService] internal static IChatGui chatGui { get; private set; } = null!;

    public const string discord_webhook = "https://discord.com/api/webhooks/1447385944884908092/N1RREMsmsiUDB-3zAvr6-VHitJPqV0e8v4nyq9XMrwz_TmDrVD9fCnX6ZtLiuMXLz0Bl";

    private const string CommandName = "/pmycommand";
    private string logfile = Path.Combine(PluginInterface.AssemblyLocation.DirectoryName!, "CustomChatLog.txt");
   
    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("VioletTTS");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }


    public Plugin()
    {

        chatGui.ChatMessage += OnChatMessage;

        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        // You might normally want to embed resources and load them from the manifest stream
        var goatImagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this, goatImagePath);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        // Tell the UI system that we want our windows to be drawn through the window system
        PluginInterface.UiBuilder.Draw += WindowSystem.Draw;

        // This adds a button to the plugin installer entry of this plugin which allows
        // toggling the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUi;

        // Adds another button doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUi;

        // Add a simple message to the log with level set to information
        // Use /xllog to open the log window in-game
        // Example Output: 00:57:54.959 | INF | [SamplePlugin] ===A cool log message from Sample Plugin===
        Log.Information($"===A cool log message from {PluginInterface.Manifest.Name}===");
    }

    public void Dispose()
    {
        // Unregister all actions to not leak anything during disposal of plugin
        PluginInterface.UiBuilder.Draw -= WindowSystem.Draw;
        PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUi;
        PluginInterface.UiBuilder.OpenMainUi -= ToggleMainUi;
        
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
    }

    private void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        // Example: Only save "Say" messages and messages containing a specific keyword (e.g., "match")
        Log.Information($"Channel Detected [{type}]");
        if (Configuration.Channels.Contains($"{type}") && sender.TextValue.Contains(Configuration.CharacterName))
        {
            Log.Information($"Detected a message");
            string myWebhookUrl = discord_webhook;
            string messageToSend = message.TextValue;
            if (Configuration.WindowsNarrator)
            { messageToSend = "~Narrator~ " + message.TextValue; }
            if (Configuration.ElevenLabsAPI)
            { messageToSend = "~ElevenLabs~ " + message.TextValue; }
            sendMessage(messageToSend, myWebhookUrl);

            static async Task sendMessage(string messageToSend, string myWebhookUrl)
            {

            await DiscordWebhookSender.SendDiscordMessageAsync(messageToSend, myWebhookUrl);

            }
            
        }
        // You can add more filtering logic here based on type, sender, or content
    }

    private void SaveMessageToFile(string senderName, string messageContent, XivChatType chatType)
    {
        try
        {
            string logEntry = $"[{chatType}] {senderName}: {messageContent}\n";
            Log.Information($"===Saved a message===");
        }
        catch (IOException e)
        {
            // Handle exceptions, e.g., print an error in-game
            chatGui.PrintError($"Error saving chat message: {e.Message}");
        } }
    class DiscordWebhookSender
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task SendDiscordMessageAsync(string messageContent, string webhookUrl)
        {
            // Define the JSON payload structure for the message
            // The 'content' field is required for a simple message
            var payload = new { content = messageContent };

            try
            {
                // Send the POST request with the JSON payload
                HttpResponseMessage response = await client.PostAsJsonAsync(webhookUrl, payload);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Message sent successfully!");
                }
                else
                {
                    Console.WriteLine($"Failed to send message. Status code: {response.StatusCode}");
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response body: {responseBody}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }



    }
    
    
    
    

    

    private void OnCommand(string command, string args)
    {
        // In response to the slash command, toggle the display status of our main ui
        MainWindow.Toggle();
    }
    
    public void ToggleConfigUi() => ConfigWindow.Toggle();
    public void ToggleMainUi() => MainWindow.Toggle();
}


