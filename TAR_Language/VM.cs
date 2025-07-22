namespace Tar {
  public class VM {
    private Dictionary<string, string> pVariables;

    public Dictionary<string, string> variables => pVariables;

    public void AddVariable(string key, string input) {

    }

    public VM() {
      this.pVariables = new Dictionary<string, string>();
    }
  }
}
