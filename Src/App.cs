using Gachabot.Models;
using Newtonsoft.Json;

namespace Gachabot;
public class App {
  private Config AppConfig;
  private readonly Bot GachaBot;
  private readonly TwitchAuth _TwitchAuth;

  public App () {
    Log.Info("Initializing");
    AppConfig = GetConfig();
    GachaBot = new(AppConfig);
    _TwitchAuth = new(
      AppConfig.ClientSettings.ClientId,
      AppConfig.ClientSettings.ClientSecret,
      AppConfig.ClientSettings.RedirectUri
    );

    _TwitchAuth.TwitchTokenSet += GachaBot.SetClientCredentials;
  }

  public void Run () {
    while (true) {
      string? inputMessage = Console.ReadLine();
      if (inputMessage == null) continue;
      AdminInputHandler(inputMessage);
    }
  }

  private static Config GetConfig () {
    string configString = File.ReadAllText("./config.dev.json");
    return JsonConvert.DeserializeObject<Config>(configString);
  }

  private void AdminInputHandler (string message) {
    string sanitizedMessage = MessageHandler.SanitizeMessage(message);
    var (command, commMessage) = MessageHandler.SplitMessage(sanitizedMessage);
    // if (!builtin V && !command) return;
    // TODO change these to enums V
    switch (command) {
      case "restart": // TODO make bot singleton and setup restart
        break;
      case "exit":
      case "close":
      case "end":
      case "leave":
        Log.Info("Exiting...");
        Environment.Exit(0);
        break;
      default:
        break;
    }
  }
}
