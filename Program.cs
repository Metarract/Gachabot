using Randobot.Models;

namespace Randobot;
internal class Program {
  public static void Main (string[] args) {
    try {
      Log.Info("Beginning Setup Procedures");
      var builder = WebApplication.CreateBuilder(args);
      var appConfig = Config.GetConfig();

      builder.Services.AddSingleton(new Bot(appConfig));
      builder.Services.AddSingleton(new TwitchAuth(
        appConfig.ClientConfig.ClientId,
        appConfig.ClientConfig.ClientSecret,
        appConfig.ClientConfig.RedirectBaseUrl
      ));

      var app = builder.Build();

      var bot = app.Services.GetService<Bot>();
      var auth = app.Services.GetService<TwitchAuth>();

      if (auth is null || bot is null) throw new Exception("Bot/TwitchAuth objects not injected properly");
      auth.TwitchTokenSet += bot.SetClientCredentials;
      bot.CheckCredValidity += auth.AttemptTokenRefresh;

      #region endpoints
      app.MapGet("/authorize", (TwitchAuth twitchAuth) => {
        Log.Info("Requesting User authorization...");
        Results.Redirect(twitchAuth.GetAuthorizationUrl());
      });

      app.MapGet("/token", async (string? code, string? scope, string? state, string? error, Bot bot, TwitchAuth twitchAuth) => {
        try {
          if (state != twitchAuth.StateString) {
            Log.Warn("State string did not match, possible request forgery");
            return Results.BadRequest();
          }
          if (error != null) {
            Log.Warn("Could not authorize");
            Log.Warn(error);
            return Results.Unauthorized();
          }
          if (code == null) {
            Log.Warn("Did not recieve a code from response");
            return Results.BadRequest();
          }

          Log.Info("User authorized token for Bot Client");
          await twitchAuth.GetToken(code);
          Log.Info("User Auth Token Generated");
          return Results.Content("Success: You may now close this window");
        } catch (Exception ex) {
          Log.Error(ex.Message);
          return Results.StatusCode(500);
        }
      });
      #endregion

      app.Urls.Add($"http://localhost:{appConfig.ClientConfig.LocalAppPort}");
      app.Run();
    } catch (Exception ex) {
      Log.Fatal(ex.Message);
      Log.Fatal("Press any key to exit...");
      Console.ReadKey();
      Environment.Exit(1);
    }
  }
}
