using Randobot.Models;
using TwitchLib.Client.Models;

namespace Randobot;
public static class Commands {
  public const string Rando = "rando";
}

public static class MessageHandler {
  private static void FixFor7tv (List<string> argList) {
    /* 
      !! DB40 DC00 removing these two characters because of 7tv
      The 7tv extension attaches some nonsense character(s, depending on encoding) to the end of every other message you send to "get past spam filters"
      these aren't normal whitespace character(s) and as such they don't get trimmed, so we need to remove any elements
      in the list that contain them

      While this doesn't get used in this project, i would be remiss if i did not include it for anyone that wanted to extend this
    */
    var argCount = argList.Count;
    if (argCount > 0) {
      string testString = ((char)0xDB40).ToString() + ((char)0xDC00).ToString();
      argList.RemoveAll(arg => arg == testString);
      Log.Debug("Trimmed characters from 7tv sent message");
    }
  }

  private static string GetCommandMapping (string command, List<CommandMap> mappings) {
    var match = mappings.Find(mapping => mapping.Input.ToLower().Trim() == command.ToLower().Trim());
    return ((match.Target?.Trim() is not null) ? match.Target : command).ToLower().Trim();
  }

  public async static Task<string?> GetCommandResponse (ChatCommand command, CommandConfig commandConfig) {
    FixFor7tv(command.ArgumentsAsList);
    var mappedCommand = GetCommandMapping(command.CommandText, commandConfig.BotCommandMapping);
    return mappedCommand switch {
      Commands.Rando => await Randomizer.GetRandomizedResponse(commandConfig),
      _ => null,
    };
  }
}
