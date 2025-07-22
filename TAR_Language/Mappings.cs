namespace Tar {
  public enum KeywordEnum {
    IF,
    WITH,
    TEXT,
    REGEX,
    NONE,
    REPLACE,
    TO,
    ELSE,
    END,
    INPUT,
    PRINT,

    // Not normal ones
    DATA,
    COMMENT
  };

  static class Mappings {
    public static Dictionary<string, KeywordEnum> keywordMappings = new Dictionary<string, KeywordEnum> {
      ["IF"] = KeywordEnum.IF,
      ["WITH"] = KeywordEnum.WITH,
      ["TEXT"] = KeywordEnum.TEXT,
      ["REGEX"] = KeywordEnum.REGEX,
      ["NONE"] = KeywordEnum.NONE,
      ["REPLACE"] = KeywordEnum.REPLACE,
      ["TO"] = KeywordEnum.TO,
      ["ELSE"] = KeywordEnum.ELSE,
      ["END"] = KeywordEnum.END,
      ["INPUT"] = KeywordEnum.INPUT,
      ["PRINT"] = KeywordEnum.PRINT
    };

    public static KeywordEnum GiveKeyMapping(string token) {
      if (token[0] == '#')
        return KeywordEnum.COMMENT;

      if (keywordMappings.ContainsKey(token))
        return keywordMappings[token];

      return KeywordEnum.DATA;
    }
  }
}
