using NLog;

namespace Gachabot;
public static class Log {
  private static readonly Logger _Logger = LogManager.GetCurrentClassLogger();

  public static void Trace (string message) => _Logger.Trace(message);

  public static void Info (string message) => _Logger.Info(message);
  public static void Info (string message, params object[] args) => _Logger.Info(message, args);

  public static void Debug (string message) => _Logger.Debug(message);
  public static void Warn (string message) => _Logger.Warn(message);
  public static void Error (string message) => _Logger.Error(message);
  public static void Fatal (string message) => _Logger.Fatal(message);
}
