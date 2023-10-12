using Gachabot.Models;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Models;

namespace Gachabot;
public sealed class Bot {
  private readonly TwitchClient Client;
  private readonly Config ClientConfig;
  private bool ClientConfigured = false;

  private ConnectionCredentials? ClientCreds;
  public delegate void CheckCredValidityHandler ();
  public event CheckCredValidityHandler? CheckCredValidity;

  public Bot (Config appConfig) {
    ClientConfig = appConfig;
    Client = GetClient();
    Log.Info("Bot Client initialized");
    Log.Info("Awaiting token initialization");
  }

  #region bot setup / configuration
  private static TwitchClient GetClient () {
    ClientOptions clientOptions = new() {
      MessagesAllowedInPeriod = 20,
      ThrottlingPeriod = TimeSpan.FromSeconds(30)
    };

    WebSocketClient customClient = new(clientOptions);
    TwitchClient newClient = new(customClient);

    return newClient;
  }

  public void SetClientCredentials (string newToken) {
    ClientCreds = new(ClientConfig.ClientSettings.BotUsername, newToken);
    if (!ClientConfigured) ConfigureBotClient();
    if (!Client.IsConnected) {
      try {
        Log.Info("Setting creds and attempting to connect Bot Client...");
        Client.SetConnectionCredentials(ClientCreds);
        Client.Connect();
      } catch (Exception ex) {
        Log.Error("Ran into issues while attempting to reconnect, you may need to restart");
        Log.Error(ex.Message);
      }
    } else {
      Log.Info("Received new credential authorization, disconnecting temporarily to reup credentials...");
      Client.Disconnect();
    }
  }

  private void ConfigureBotClient () {
    Client.AddChatCommandIdentifier(ClientConfig.ClientSettings.CommandCharacter);
    Client.OnLog += OnClientLog;
    Client.OnConnected += OnClientConnected;
    Client.OnDisconnected += OnClientDisconnected;
    Client.OnConnectionError += OnClientConnectionError;
    Client.OnJoinedChannel += OnClientJoined;
    Client.OnChatCommandReceived += OnClientCommandReceived;

    Client.Initialize(ClientCreds);

    ClientConfigured = true;
    Log.Info("Bot configuration completed");
  }

  private void JoinChannels () => ClientConfig.ClientSettings.TwitchChannels.ForEach(channel => Client.JoinChannel(channel));
  #endregion

  #region bot client event handlers
  private void OnClientLog (object? sender, OnLogArgs e) {
    Log.Info($"[IRC][{e.BotUsername}] - {e.Data}");
  }

  private void OnClientConnected (object? sender, OnConnectedArgs e) {
    Log.Info("Bot Client Connected");
    JoinChannels();
  }

  private void OnClientDisconnected (object? sender, OnDisconnectedEventArgs e) {
    Log.Info("Bot Client Disconnected");
    CheckCredValidity?.Invoke();
  }

  private void OnClientConnectionError (object? sender, OnConnectionErrorArgs e) {
    Log.Info("Ran into connection issues, checking credential validity...");
    CheckCredValidity?.Invoke();
  }

  private void OnClientJoined (object? sender, OnJoinedChannelArgs e) {
    Log.Info($"Bot joined channel: {e.Channel}");
  }

  private async void OnClientCommandReceived (object? sender, OnChatCommandReceivedArgs e) {
    var response = await MessageHandler.GetCommandResponse(e.Command, ClientConfig);
    if (response != null) Client.SendMessage(e.Command.ChatMessage.Channel, response);
  }
  #endregion
}

