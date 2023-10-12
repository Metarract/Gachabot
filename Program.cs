using Gachabot.Models;
using Newtonsoft.Json;

namespace Gachabot;
internal class Program {
  public static void Main (string[] args) {
    try {
      var builder = WebApplication.CreateBuilder(args);
      var appConfig = GetConfig();

      builder.Services.AddSingleton(new Bot(appConfig));
      builder.Services.AddSingleton(new TwitchAuth(
        appConfig.ClientSettings.ClientId,
        appConfig.ClientSettings.ClientSecret,
        appConfig.ClientSettings.RedirectUri
      ));

      var app = builder.Build();

      var bot = app.Services.GetService<Bot>();
      var auth = app.Services.GetService<TwitchAuth>();

      if (auth is null || bot is null) throw new Exception("Bot/Twitch Auth not injected properly");
      auth.TwitchTokenSet += bot.SetClientCredentials;

      app.MapGet("/authorize", (TwitchAuth twitchAuth) => Results.Redirect(twitchAuth.GetAuthorizationUrl()));
      app.MapGet("/token", async (string? code, string? scope, string? state, string? error, Bot bot, TwitchAuth twitchAuth) => {
        if (error != null) return Results.Unauthorized();
        if (state != twitchAuth.StateString) return Results.BadRequest();
        if (code == null) return Results.StatusCode(500);
        Log.Info("Authorized");
        await twitchAuth.GetToken(code);
        Log.Info("User Auth Token Generated");
        return Results.Content("Success: You may now close this window");
      });

      app.Run("http://localhost:3001");
    } catch (Exception ex) {
      Log.Fatal(ex.Message);
      throw;
    }
  }

  private static Config GetConfig () {
    // ASP.NET config binding/mapping kinda sucks so i'm keeping this
    // TODO pull environment from... environment
    string configString = File.ReadAllText("./config.Development.json");
    return JsonConvert.DeserializeObject<Config>(configString);
  }
}
