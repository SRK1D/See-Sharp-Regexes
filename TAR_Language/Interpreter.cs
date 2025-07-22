using System.Text.RegularExpressions;

namespace Tar {
  public class Interpreter {
    private string pInput;
    private VM pVM;

    public string output => pInput;


    Interpreter(string input) {
      this.pVM = new VM();
      this.pInput = input;
    }

    private void InterpretStatement(Statement stat) {
      if (stat is ConditionalStatement) {
        ConditionalStatement conditionStatement = stat as ConditionalStatement;

        Regex conditionalRegex = new Regex(conditionStatement.CONDITION_MATCH);
        
        if (conditionalRegex.Matches(this.pInput).Count() > 0 && conditionStatement.IF_TRUE != null)
          foreach (Statement innerIFStatement in conditionStatement.IF_TRUE)
            this.InterpretStatement(innerIFStatement);
        else if (conditionStatement.IF_FALSE != null)
          foreach (Statement innerIFStatement in conditionStatement.IF_FALSE)
            this.InterpretStatement(innerIFStatement);

        return;
      }

      if (stat is ReplaceStatement) {
        ReplaceStatement replaceStatement = stat as ReplaceStatement;
        switch (replaceStatement.REPLACEMENT_METHOD) {
          case ReplacementMethod.TEXT:
            this.pInput = this.pInput.Replace(replaceStatement.REPLACE_FROM, replaceStatement.REPLACE_TO);
            break;
          case ReplacementMethod.REGEX:
            Regex replaceRegex = new Regex(replaceStatement.REPLACE_FROM);
            this.pInput = replaceRegex.Replace(this.pInput, replaceStatement.REPLACE_TO);
            break;
        }

        return;
      }

      if (stat is InputStatement) {
        // TODO
        return;
      }

      if (stat is PrintStatement) {
        PrintStatement printStatement = stat as PrintStatement;
        Console.WriteLine(printStatement.PRINT_TOKENS);
        return;
      }
    }


    public static string InterpreterCode(string code, string input) {
      Tokenizer? tokens = Tokenizer.InputTokenization(code);

      if (tokens == null)
        throw new Exception("Interpreter.cs: Tokenized input is NULL");

      List<Statement> statementBlocks = Tree.ParseTokenizedInput(tokens);
      Interpreter interpreterInst = new Interpreter(input);

      foreach (Statement statement in statementBlocks)
        interpreterInst.InterpretStatement(statement);

      return interpreterInst.output;
    }
  }
}
