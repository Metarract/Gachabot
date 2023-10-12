using TwitchLib.Api;
using TwitchLib.Api.Auth;
using TwitchLib.Api.Core.Enums;

namespace Gachabot;
public class TwitchAuth {
  private readonly Random random = new();
  public string StateString;

  public string? Token;
  private string? Refresh;
  private const string RTOKEN_PATH = "./cachefile";

  public delegate void TwitchTokenSetHandler (string newToken);
  public event TwitchTokenSetHandler? TwitchTokenSet;

  private readonly static TwitchAPI api = new();
  public string RedirectUri;

  public TwitchAuth (string clientId, string clientSecret, string redirectUri) {
    RedirectUri = redirectUri;
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
      RedirectUri,
      api.Settings.Scopes,
      true,
      StateString,
      api.Settings.ClientId
    );
  }

  private async void RegisterRefresh (int expiry) => await Task.Delay(expiry).ContinueWith(obj => RefreshToken());
  
  private async void AttemptGetRefreshFromLocalCache () {
    if (!File.Exists(RTOKEN_PATH)) return;
    var content = await File.ReadAllTextAsync(RTOKEN_PATH);
    if (content == null || content.Length == 0) return;
    Refresh = content;
    Log.Info("Checking bot initialization...");
    while (TwitchTokenSet?.GetInvocationList().Length == 0) {
      await Task.Delay(500);
    }
    Log.Info("Bot initialized, continuing");
    await RefreshToken();
  }

  public async Task GetToken (string authCode) => SetAuthFromResponse(await api.Auth.GetAccessTokenFromCodeAsync(authCode, api.Settings.Secret, RedirectUri, api.Settings.ClientId));
  private async Task RefreshToken () => SetAuthFromResponse(await api.Auth.RefreshAuthTokenAsync(Refresh, api.Settings.Secret, api.Settings.ClientId));

  private void SetAuthFromResponse (AuthCodeResponse res) => SetAuthVals(res.AccessToken, res.RefreshToken, res.ExpiresIn);
  private void SetAuthFromResponse (RefreshResponse res) => SetAuthVals(res.AccessToken, res.RefreshToken, res.ExpiresIn);
  private async void SetAuthVals (string t, string r, int e) {
    Token = t;
    Refresh = r;
    RegisterRefresh(e);
    await File.WriteAllTextAsync(RTOKEN_PATH, Refresh);
    TwitchTokenSet?.Invoke(Token);
  }

  #region State String
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
