namespace GeminiLab.Glos {
    public partial struct GlosValue {
        private static long Combine(int hi, int lo) {
            unchecked {
                return (long) (((ulong) (uint) hi) << 32 | (uint) lo);
            }
        }

        private static long StringHash(string v) {
            int mid = v.Length / 2;

            // only this ugly and slow implementation
            // until string.GetHashCode(ReadOnlySpan<char>) becomes available in .net standard
            return Combine(v.Substring(0, mid).GetHashCode(), v.Substring(mid).GetHashCode());
        }

        private static long FunctionHash(GlosFunction fun) {
            return Combine(fun.Prototype.GetHashCode(), fun.ParentContext?.GetHashCode() ?? 0);
        }
    }
}
