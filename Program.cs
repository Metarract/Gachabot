namespace Gachabot;
internal class Program {
  public static void Main() {
    Console.WriteLine("Initializing");
    while (true) {
      Console.Write("BOT:> ");
      string? inputMessage = Console.ReadLine();
      if (inputMessage == null) continue;
      AdminInputHandler(inputMessage);
    }
  }

  private static void AdminInputHandler(string message) {
    var sanitizedMessage = message.Trim().ToLower();
    if (sanitizedMessage.StartsWith("!")) sanitizedMessage = sanitizedMessage.Remove(0, 1);
    switch (sanitizedMessage) {
      case "exit":
      case "close":
      case "end":
      case "leave":
        Console.WriteLine("Exiting...");
        Environment.Exit(0);
        break;
      default:
        break;
    }
  }
}
