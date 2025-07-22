namespace Tar {
  
  public class TokenData {
    // Private token data
    private KeywordEnum pKeywordEnum;

    // Public token properties
    public KeywordEnum keywordEnum => pKeywordEnum;

    public TokenData(string token) {
      this.pKeywordEnum = Mappings.GiveKeyMapping(token); 
    }
  }

  public class Tokenizer {
    // Private token properties
    private Tokenizer? pNext;
    private Tokenizer? pPrev;
    private string pToken;
    private TokenData pTokenData;

    // Public properties
    public Tokenizer? next => pNext;
    public Tokenizer? prev => pPrev;
    public string token => pToken;
    public TokenData tokenData => pTokenData;

    public Tokenizer(string token, Tokenizer? previous = null, Tokenizer? next = null) {
      this.pToken = token;
      this.pTokenData = new TokenData(token);
      this.pNext = next;
      this.pPrev = previous;
    }

    public static Tokenizer? InputTokenization(string input) {
      // Convers the input into multiple tokens
      List<string> tokens = input
        .Replace("\r", null)
        .Split('\n', StringSplitOptions.RemoveEmptyEntries)
        .Select<string, string[]>(lineToken => lineToken.Split(' '))
        .SelectMany(ln => ln)
        .Select(tokens => tokens.Trim())
        .Reverse()
        .ToList();

      if (!tokens.Any())
        return null;

      Tokenizer? tracker = null;

      foreach (string token in tokens) {
        // Console.WriteLine(token);
        if (tracker == null)
          tracker = new Tokenizer(token);
        else
          tracker = tracker.pPrev = new Tokenizer(token, null, tracker);
      }

      return tracker;
    }
  }
}
