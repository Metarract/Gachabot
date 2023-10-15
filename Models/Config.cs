namespace Randobot {
  namespace Models {
    public struct Config {
      public ClientConfig ClientConfig;
      public CommandConfig CommandConfig;
    }

    public struct ClientConfig {
      public string ClientId;
      public string ClientSecret;
      public string BotUsername;
      public string RedirectBaseUri;
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
