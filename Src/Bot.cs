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
  private bool BotInitialized = false;

  private ConnectionCredentials? ClientCreds;

  public Bot (Config appConfig) {
    ClientConfig = appConfig;
    Client = GetClient();
    Log.Info("Bot started");
    Log.Info("Awaiting token initialization (visit /authorize to get a new token for the bot, or wait for local cache)");
  }

  #region bot setup
  private static TwitchClient GetClient () {
    ClientOptions clientOptions = new() {
      MessagesAllowedInPeriod = 750,
      ThrottlingPeriod = TimeSpan.FromSeconds(30)
    };

    WebSocketClient customClient = new(clientOptions);
    TwitchClient newClient = new(customClient);

    return newClient;
  }

  public void SetClientCredentials (string tempToken) {
    ClientCreds = new(ClientConfig.ClientSettings.BotUsername, tempToken);
    if (BotInitialized) return;
    InitBotClient();
  }

  private void InitBotClient () {
    Client.AddChatCommandIdentifier(ClientConfig.ClientSettings.CommandCharacter);
    Client.OnLog += OnClientLog;
    Client.OnConnected += OnClientConnected;
    Client.OnDisconnected += OnClientDisconnected;
    Client.OnConnectionError += OnClientConnectionError;
    Client.OnJoinedChannel += OnClientJoined;
    Client.OnChatCommandReceived += OnClientCommandReceived;

    ConnectionCredentials credentials = new(ClientConfig.ClientSettings.BotUsername, ClientConfig.ClientSettings.BotToken);
    Client.Initialize(credentials);
    Client.Connect();

    BotInitialized = true;
    Log.Info("Bot initialized");
  }

  private void JoinChannels () => ClientConfig.ClientSettings.TwitchChannels.ForEach(channel => Client.JoinChannel(channel));
  #endregion

  #region client events
  private void OnClientLog (object? sender, OnLogArgs e) {
    Log.Info($"[BOT] [{e.BotUsername}] - {e.Data}");
  }

  private void OnClientConnected (object? sender, OnConnectedArgs e) {
    Log.Info("[BOT] CLIENT CONNECTED");
    JoinChannels();
  }

  private void OnClientDisconnected (object? sender, OnDisconnectedEventArgs e) {
    Log.Info("Client Disconnected");
    try {
      Log.Info("Setting new creds and attempting to reconnect...");
      Client.SetConnectionCredentials(ClientCreds);
      Client.Connect();
    } catch (Exception error) {
      Log.Error(error.Message);
    }
  }

  private void OnClientConnectionError (object? sender, OnConnectionErrorArgs e) {
    Log.Info("Ran into some issues, disconnecting to reup credentials...");
    Client.Disconnect();
  }

  private void OnClientJoined (object? sender, OnJoinedChannelArgs e) {
    Log.Info($"[BOT] JOINED - {e.Channel}");
  }

  private void OnClientCommandReceived (object? sender, OnChatCommandReceivedArgs e) {
    Log.Debug($"[COMMAND] {e.Command.CommandText}");
  }
  #endregion
}

