namespace Tar {
  class Tar_Entrypoint {
    public static void Main(string[] args) {
      string code = @"IF test_
WITH TEXT
REPLACE test
TO tester
ELSE
WITH REGEX
REPLACE \w+
TO HELP 
END
PRINT WORKED";

      string test = (Interpreter.InterpreterCode(code, "test testing"));
      Console.WriteLine(test);
    }
  }
}

