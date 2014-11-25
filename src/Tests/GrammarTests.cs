﻿using System;
using System.Linq;
using BSParser;
using Nitra;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class GrammarTests
    {
        [TestCase("class Greeter { }")]
        public void ParseCodeSnippet(string sourceCode)
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

            Assert.That(compilationUnit.IsSuccess, Is.True);

            // Get pretty-printed version of parse tree.
            var parseTree = compilationUnit.CreateParseTree();
            var parsedCode = parseTree.ToString();

            Console.WriteLine(parsedCode);
        }
    }
}