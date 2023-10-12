using TwitchLib.Api;
using TwitchLib.Api.Auth;
using TwitchLib.Api.Core.Enums;

namespace Gachabot;
public class TwitchAuth {
  private readonly Random random = new();
  public string StateString;

  public string? Token;
  private string? Refresh;
  private long Expiry;
  private static long UnixSecNow => new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
  private const string RTOKEN_PATH = "./cachefile";

  public delegate void TwitchTokenSetHandler (string newToken);
  public event TwitchTokenSetHandler? TwitchTokenSet;

  private readonly static TwitchAPI api = new();
  public string BaseUri;

  public TwitchAuth (string clientId, string clientSecret, string redirectBaseUri) {
    BaseUri = redirectBaseUri;
    api.Settings.ClientId = clientId;
    api.Settings.Secret = clientSecret;

    api.Settings.Scopes = new List<AuthScopes>() { };

    api.Settings.Scopes.Add(AuthScopes.Chat_Read);
    api.Settings.Scopes.Add(AuthScopes.Chat_Edit);
    StateString = GetNewStateString();

    AttemptGetRefreshFromLocalCache();
  }

  public string GetAuthorizationUrl () {
    ResetStateString();
    return api.Auth.GetAuthorizationCodeUrl(
      BaseUri,
      api.Settings.Scopes,
      true,
      StateString,
      api.Settings.ClientId
    );
  }

  public async void AttemptTokenRefresh () {
    try {
      if (UnixSecNow > Expiry) {
        Log.Info("Token expired, attempting refresh...");
        await RefreshToken();
      } else if (Token != null) {
        TwitchTokenSet?.Invoke(Token);
      } else {
        Log.Warn("Token state unknown, attempting refresh from cache...");
        AttemptGetRefreshFromLocalCache();
      }
    } catch (Exception ex) {
      Log.Error("An error occurred while attempting to resolve Token state");
      Log.Error(ex.Message);
      throw;
    }
  }

  private async void AttemptGetRefreshFromLocalCache () {
    Log.Info("Checking Token Cache...");
    if (!File.Exists(RTOKEN_PATH)) {
      Log.Warn("Token Cache not found");
      Log.Info($"Please visit {BaseUri}/authorize to get a new token for the bot");
      return;
    }
    var content = await File.ReadAllTextAsync(RTOKEN_PATH);
    if (content == null || content.Length == 0) return;
    Log.Info("Token Cache loaded");
    Refresh = content;
    Log.Info("Checking Bot initialization...");
    while (TwitchTokenSet?.GetInvocationList().Length == 0) {
      await Task.Delay(500);
    }
    Log.Info("Bot initialized, continuing");
    await RefreshToken();
  }

  public async Task GetToken (string authCode) => SetAuthFromResponse(await api.Auth.GetAccessTokenFromCodeAsync(authCode, api.Settings.Secret, $"{BaseUri}/token", api.Settings.ClientId));
  private async Task RefreshToken () => SetAuthFromResponse(await api.Auth.RefreshAuthTokenAsync(Refresh, api.Settings.Secret, api.Settings.ClientId));

  private void SetAuthFromResponse (AuthCodeResponse res) => SetAuthVals(res.AccessToken, res.RefreshToken, res.ExpiresIn);
  private void SetAuthFromResponse (RefreshResponse res) => SetAuthVals(res.AccessToken, res.RefreshToken, res.ExpiresIn);
  private async void SetAuthVals (string t, string r, int e) {
    Token = t;
    Refresh = r;
    Expiry = e + UnixSecNow;
    await File.WriteAllTextAsync(RTOKEN_PATH, Refresh);
    TwitchTokenSet?.Invoke(Token);
  }

  #region State String
  // this is used to ensure the token response we received is tied to the oauth request we made, so we don't send our client secret to a malicious third party
  private string GenerateRandomString (int length) {
    //alphanumeric string to pull a random character from
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    return new string(
      Enumerable.Repeat(chars, length)
      .Select(s => s[random.Next(s.Length)])
      .ToArray());
  }

  private string GetNewStateString () => GenerateRandomString(32);
  private void ResetStateString () => StateString = GetNewStateString();
  #endregion
}
