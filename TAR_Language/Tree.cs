namespace Tar {
  public enum ReplacementMethod {
    NONE,
    REGEX,
    TEXT
  };

  public abstract class Statement {}

  public class ConditionalStatement : Statement {
    public string CONDITION_MATCH;
    public List<Statement> IF_TRUE;
    public List<Statement> IF_FALSE;

    public ConditionalStatement() {
      this.IF_TRUE = new List<Statement>();
      this.IF_FALSE = new List<Statement>();
    }
  }

  public class ReplaceStatement : Statement {
    public string REPLACE_FROM;
    public string REPLACE_TO;
    public ReplacementMethod REPLACEMENT_METHOD;
  }

  public class PrintStatement : Statement {
    public string PRINT_TOKENS; 
  }

  public class InputStatement : Statement {
    public string INPUT_NAME;
  }

  public static class Tree {
    public static List<Statement> ParseTokenizedInput(Tokenizer? tokens) {

      // This is the core list for the tree (parser)
      List<Statement> treeDeclaration = new List<Statement>();
      
      // Used to follow the nested statements (supports multiple if statements)
      Stack<List<Statement>> treePath = new Stack<List<Statement>>();
      treePath.Push(treeDeclaration);
      
      void AdvanceToken() => tokens = tokens.next;
      while (tokens != null) {
        if (tokens.tokenData.keywordEnum == KeywordEnum.COMMENT) {
          AdvanceToken();
          while (tokens != null && tokens.tokenData.keywordEnum == KeywordEnum.DATA)
            AdvanceToken();
          continue; // Should skip everything, it's not part of the parser
        }

        switch (tokens.tokenData.keywordEnum) {
          // If statement
          case KeywordEnum.IF:
            ConditionalStatement conditionStatement = new ConditionalStatement();
            
            AdvanceToken();
            if (tokens.tokenData.keywordEnum != KeywordEnum.DATA)
              throw new Exception("Tree.cs - IF0: Invalid IF statement, needs a regex after the IF statement.");

            conditionStatement.CONDITION_MATCH = tokens.token;

            treePath.Peek().Add(conditionStatement);
            treePath.Push(conditionStatement.IF_TRUE);

            break;

          // Else statement
          case KeywordEnum.ELSE:
            if (treePath.Count < 1)
              throw new Exception("Tree.cs - ELSE0: There is a missing IF statement");

            treePath.Pop();

            if (treePath.Peek().Last() is not ConditionalStatement)
              throw new Exception("Tree.cs - ELSE1: There is a missing IF statement");

            treePath.Push((treePath.Peek().Last() as ConditionalStatement).IF_FALSE);
            break;

          // End statement
          case KeywordEnum.END:
            if (treePath.Count < 1)
              throw new Exception("Tree.cs - END0: There is a missing IF statement");

            treePath.Pop();

            if (treePath.Peek().Last() is not ConditionalStatement)
              throw new Exception("Tree.cs - END1: There is a missing IF statement");
            break;

          case KeywordEnum.WITH:
            ReplaceStatement replaceStatement = new ReplaceStatement();
            AdvanceToken();
            
            if (!new List<KeywordEnum>{KeywordEnum.REGEX, KeywordEnum.TEXT, KeywordEnum.NONE }.Contains(tokens.tokenData.keywordEnum))
              throw new Exception("Tree.cs - WITH0: 'REGEX', 'TEXT' or 'NONE' after the WITH, nothing else");

            switch (tokens.token) {
              case "REGEX":
                replaceStatement.REPLACEMENT_METHOD = ReplacementMethod.REGEX;
                break;
              case "TEXT":
                replaceStatement.REPLACEMENT_METHOD = ReplacementMethod.TEXT;
                break;
              case "NONE":
                replaceStatement.REPLACEMENT_METHOD = ReplacementMethod.NONE;
                break;
              default:
                throw new Exception("Tree.cs - WITH1: Invalid type of replacement");

            }

            treePath.Peek().Add(replaceStatement);
            break;

          case KeywordEnum.REPLACE:
            if (treePath.Peek().Last() is not ReplaceStatement || (treePath.Peek().Last() as ReplaceStatement).REPLACE_FROM != null)
              throw new Exception("Tree.cs - REPLACE0: There is not any 'WITH' statement or it has been already used");

            if (tokens.next.tokenData.keywordEnum != KeywordEnum.DATA)
              throw new Exception("Tree.cs - REPLACE1: Data is expected after the REPLACE keyword");

            AdvanceToken();
            List<string> data = new List<string>();

            while (tokens != null && tokens.tokenData.keywordEnum == KeywordEnum.DATA) {
              data.Add(tokens.token);
              AdvanceToken();
            }
            (treePath.Peek().Last() as ReplaceStatement).REPLACE_FROM = String.Join(' ', data);
            // Console.WriteLine((treePath.Peek().Last() as ReplaceStatement).REPLACE_FROM);
            continue;

          case KeywordEnum.TO:
            if (treePath.Peek().Last() is not ReplaceStatement || (treePath.Peek().Last() as ReplaceStatement).REPLACE_TO != null)
              throw new Exception("Tree.cs - TO0: There is not any 'WITH' statement or it has been already used");

            if (tokens.next.tokenData.keywordEnum != KeywordEnum.DATA)
              throw new Exception("Tree.cs - TO1: Data is expected after the TO keyword");

            AdvanceToken();
            List<string> data_t = new List<string>();

            while (tokens != null && tokens.tokenData.keywordEnum == KeywordEnum.DATA) {
              data_t.Add(tokens.token);
              AdvanceToken();
            }

            (treePath.Peek().Last() as ReplaceStatement).REPLACE_TO = String.Join(' ', data_t);
            continue;

          case KeywordEnum.PRINT:
            if (tokens.next.tokenData.keywordEnum != KeywordEnum.DATA)
              throw new Exception("Tree.cs - PRINT0: Data is expected after the 'PRINT' keyword");

            AdvanceToken();
            List<string> data_t2 = new List<string>();

            while (tokens != null && tokens.tokenData.keywordEnum == KeywordEnum.DATA) {
              data_t2.Add(tokens.token);
              AdvanceToken();
            }

            treePath.Peek().Add(new PrintStatement {PRINT_TOKENS = String.Join(' ', data_t2)});
            continue;

          case KeywordEnum.INPUT:
            if (tokens.next.tokenData.keywordEnum != KeywordEnum.DATA)
              throw new Exception("Tree.cs - INPUT0: An input name is expected after the 'INPUT' statement");

            AdvanceToken();

            if (tokens.next.tokenData.keywordEnum == KeywordEnum.DATA)
              throw new Exception("Tree.cs - INPUT1: An input name should not be of a token size > 1");

            treePath.Peek().Add(new InputStatement {INPUT_NAME = tokens.token});
            break;


        }

        AdvanceToken();
      }

      // Some last checkups
      if (treePath.Count > 1)
        throw new Exception("Tree.cs - LAST_CHECKUP0: Missing 'END' statement for an 'IF' block");

      return treeDeclaration;
    }
  }
}
