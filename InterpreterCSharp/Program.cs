namespace InterpreterCSharp
{
    internal static class Program
    {
        private static void Main()
        {
            MyRepl? repl = new MyRepl();
            repl.Run();
        }
    }
}



