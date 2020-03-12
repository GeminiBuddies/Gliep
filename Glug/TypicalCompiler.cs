using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using GeminiLab.Glos.ViMa;
using GeminiLab.Glug.AST;
using GeminiLab.Glug.Parser;
using GeminiLab.Glug.Tokenizer;

namespace GeminiLab.Glug {
    [ExcludeFromCodeCoverage]
    public static class TypicalCompiler {
        public static IGlugTokenStream Tokenize(string value) => new GlugTokenizer(new StringReader(value));

        public static Expr Parse(IGlugTokenStream stream) => GlugParser.Parse(stream);
        
        public static GlosUnit PostProcessAndCodeGen(Expr root) {
            root = new Function("<root>", false, new List<string>(), root);

            var it = new NodeInformation();

            new WhileBreakPairingVisitor(it).Visit(root, null);
            new IsOnStackListVisitor(it).Visit(root);
            new IsAssignableVisitor(it).Visit(root);
            
            var vdv = new VarDefVisitor(it);
            vdv.Visit(root, vdv.RootTable);

            var vcv = new VarRefVisitor(vdv.RootTable, it);
            vcv.Visit(root);

            vdv.DetermineVariablePlace();

            var gen = new CodeGenVisitor(it);
            gen.Visit(root, new CodeGenContext(null!, false));

            return gen.Builder.GetResult();
        }

        public static GlosUnit Compile(string code) {
            using var tok = Tokenize(code);
            var ast = Parse(tok);
            return PostProcessAndCodeGen(ast);
        }
    }
}
