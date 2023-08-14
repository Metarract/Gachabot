using Newtonsoft.Json;
using Gachabot.Models;

namespace Gachabot;
public class App {
  public App() {
    Console.WriteLine("Initializing");
    var config = GetConfig();
    Bot bot = new(config);
  }

  public void Run() {
    while (true) {
      string? inputMessage = Console.ReadLine();
      if (inputMessage == null) continue;
      AdminInputHandler(inputMessage);
    }
  }

  private static Config GetConfig() {
    string configString = File.ReadAllText("./config.dev.json");
    return JsonConvert.DeserializeObject<Config>(configString);
  }

  private void AdminInputHandler(string message) {
    var sanitizedMessage = message.Trim().ToLower();
    if (sanitizedMessage.StartsWith("!")) sanitizedMessage = sanitizedMessage.Remove(0, 1);
    switch (sanitizedMessage) {
      case "exit":
      case "close":
      case "end":
      case "leave":
        Console.WriteLine("Exiting...");
        Environment.Exit(0);
        break;
      default:
        break;
    }
  }
}
