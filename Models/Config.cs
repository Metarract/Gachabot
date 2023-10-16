using Newtonsoft.Json;

namespace Randobot {
  namespace Models {
    public class Config {
      public ClientConfig ClientConfig;
      public CommandConfig CommandConfig;

      public static Config GetConfig () {
        // ASP.NET config binding/mapping kinda sucks for nested objects imho so i'm keeping this
        string configString = File.ReadAllText($"{AppContext.BaseDirectory}/config.json");
        var configObject = JsonConvert.DeserializeObject<Config>(configString) ?? throw new Exception("Could not get config data, ensure there is a config.json file in the same directory as the executable");
        return configObject;
      }
    }

    public struct ClientConfig {
      public string ClientId;
      public string ClientSecret;
      public string BotUsername;
      public string RedirectBaseUrl;
      public int LocalAppPort;
      public List<string> TwitchChannels;
    }

    public struct CommandConfig {
      public char CommandCharacter;
      public List<CommandMap> BotCommandMapping;
      public string Template;
      public string ListDirectory;
    }

    public struct CommandMap {
      public string Input;
      public string Target;
    }
  }
}
