namespace Gachabot;
internal class Program {
  public static void Main () {
    try {
      App app = new();
      app.Run();
    } catch (Exception ex) {
      Log.Fatal(ex.Message);
    }
  }
}
