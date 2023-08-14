using System.Collections.Generic;

namespace Gachabot {
  namespace Models {
    public struct Config {
      public ClientSettings clientSettings;
      public List<CommandMap> botCommandMapping;
    }

    public struct CommandMap {
      public string Input;
      public string Target;
    }

    public struct ClientSettings {
      public string botToken;
      public string botUsername;
      public List<string> twitchChannels;
    }
  }
}
