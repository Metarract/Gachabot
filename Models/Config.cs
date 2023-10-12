namespace Gachabot {
  namespace Models {
    public struct Config {
      public ClientSettings ClientSettings;
      public List<CommandMap> BotCommandMapping;
      public string Template;
    }

    public struct CommandMap {
      public string Input;
      public string Target;
    }

    public struct ClientSettings {
      public string ClientId;
      public string ClientSecret;
      public string BotToken;
      public string BotUsername;

      public string RedirectUri;

      public char CommandCharacter;
      public List<string> TwitchChannels;
    }
  }
}
