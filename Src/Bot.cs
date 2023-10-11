using Gachabot.Models;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Gachabot {
  public sealed class Bot {
    private readonly TwitchClient Client;
    private readonly Config ClientConfig;

    public Bot(Config botConfig) {
      ClientConfig = botConfig;
      Client = GetClient();
      InitClient();
    }

    #region bot setup
    private TwitchClient GetClient() {
      ClientOptions clientOptions = new() {
        MessagesAllowedInPeriod = 750,
        ThrottlingPeriod = TimeSpan.FromSeconds(30)
      };

      WebSocketClient customClient = new(clientOptions);
      TwitchClient newClient = new(customClient);

      return newClient;
    }

    private void InitClient() {
      Client.OnLog += OnClientLog;
      Client.OnConnected += OnClientConnected;
      Client.OnJoinedChannel += OnClientJoined;
      Client.OnMessageReceived += OnClientMessageReceived;

      ConnectionCredentials credentials = new(ClientConfig.clientSettings.botUsername, ClientConfig.clientSettings.botToken);
      Client.Initialize(credentials);
      Client.Connect();
    }

    private void JoinChannels() => ClientConfig.clientSettings.twitchChannels.ForEach(channel => Client.JoinChannel(channel));

    #endregion

    #region client events
    private void OnClientLog(object? sender, OnLogArgs e) {
      Log.Info($"{e.DateTime}: {e.BotUsername} - {e.Data}");
    }

    private void OnClientConnected(object? sender, OnConnectedArgs e) {
      Log.Info("CLIENT CONNECTED");
      JoinChannels();
    }

    private void OnClientJoined(object? sender, OnJoinedChannelArgs e) {
      Log.Info($"JOINED - {e.Channel}");
    }

    private void OnClientMessageReceived(object? sender, OnMessageReceivedArgs e) {
      return;
    }
    #endregion
  }
}
