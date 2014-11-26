using System;
using System.Linq;
using BSParser;
using Nitra;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class GrammarTests
    {
        private const string GreekSemicolonQuestionThing = ";";

        private string ParseCodeSnippet(string sourceCode)
        {
            var sourceSnapshot = new SourceSnapshot(sourceCode);
            var parserHost = new ParserHost();
            var compilationUnit = BSGrammar.CompilationUnit(sourceSnapshot, parserHost);

            // Check for parse errors.
            if (!compilationUnit.IsSuccess)
            {
                throw new Exception(string.Join(Environment.NewLine,
                    compilationUnit.GetErrors().Select(x => string.Format("Line {0}, Col {1}: {2}{3}{4}",
                        x.Location.StartLineColumn.Line, x.Location.StartLineColumn.Column, x.Message,
                        Environment.NewLine, x.Location.Source.GetSourceLine(x.Location.StartPos).GetText()))));
            }

            Assert.IsTrue(compilationUnit.IsSuccess);

            // Get pretty-printed version of parse tree.
            var parseTree = compilationUnit.CreateParseTree();
            var parsedCode = parseTree.ToString();

            return parsedCode;
        }

        [TestCase("class Greeter:")]
        [TestCase(
@"class Greeter:
    public function foo()
      ;")]
        [TestCase(
@"class Greeter:
    public function foo(€name)
      ;")]
        [TestCase(
@"class Greeter:
    public function __construct(€name)
      ;")]
        [TestCase(
@"class Greeter:
    public function __construct(€name)
        HALT_AND_CATCH_FIRE
      (unless true)")]
        [TestCase(
@"class Greeter:
    public function __construct(€name)
        HALT_AND_CATCH_FIRE
      (unless €name != null);")]
        [TestCase(
@"class Greeter:
    public function __construct(€name)
      €this->name = €name;")]
        [TestCase(
@"class Greeter:
    public function __construct(€name)
      echo ""hello"", 'hello', ' ',  €this->name;")]
        public void Parse(string code)
        {
            string result = ParseCodeSnippet(code);

            Console.WriteLine(result);
        }
    }
}
