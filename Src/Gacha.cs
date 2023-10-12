using System.Text.RegularExpressions;
using Gachabot.Models;

namespace Gachabot;
public static class Gacha {
  private static readonly string templateSubstitutionRegex = @"\{\w+\}";

  public static async Task<List<string>> GetItemsFromFile (string filePath) => new(await File.ReadAllLinesAsync(filePath));
  public static List<string> GetFilenamesFromTemplate (string templateString) {
    Log.Debug(templateString);
    Regex r = new(templateSubstitutionRegex);
    MatchCollection m = r.Matches(templateString);
    Log.Debug(m.Count.ToString());
    List<string> filenames = new() { };
    foreach (Match match in m) {
      var formattedString = match.Value[1..^1];
      filenames.Add(formattedString);
    }
    return filenames;
  }

  public static T GetRandomEntry<T> (List<T> list) {
    Random rand = new(Guid.NewGuid().GetHashCode());
    int index = rand.Next(list.Count);
    return list[index];
  }

  public static async Task<string> GetGachaResponse (Config config) {
    try {
      var templateStringCopy = config.Template;
      var filenames = GetFilenamesFromTemplate(config.Template);
      List<List<string>> entryListList = new() { };
      foreach (var filename in filenames) {
        List<string> entryList = await GetItemsFromFile($"{config.ListDirectory}/{filename}");
        entryList.Insert(0, filename); // using filename as first entry to act as a header, for later substitution
        entryListList.Add(entryList);
      }
      entryListList.ForEach(entryList => {
        var chosenEntry = GetRandomEntry(entryList.GetRange(1, entryList.Count - 1)); // exclude our header from our choices
        templateStringCopy = templateStringCopy.Replace($"{{{entryList[0]}}}", chosenEntry); // god this syntax sucks lol just use regular escape characters, dotnet
      });
      return templateStringCopy;
    } catch (Exception ex) {
      Log.Error(ex.Message);
      throw;
    }
  }
}
