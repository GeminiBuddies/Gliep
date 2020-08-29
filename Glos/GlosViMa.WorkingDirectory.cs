namespace GeminiLab.Glos {
    public partial class GlosViMa {
        public string WorkingDirectory { get; set; }

        public GlosUnit? CurrentExecutingUnit => _callStack.Count > 0 ? _callStack[^1].Function.Prototype.Unit : null;
    }
}