using System;
using GeminiLab.Glos;
using Xunit;

namespace GeminiLab.XUnitTester.Gliep.Misc {
    public class GlosValueArrayItemChecker {
        public GlosValueArrayChecker Checker { get; }
        public int Position { get; private set; }
        private ref GlosValue Current => ref Checker.Target.Span[Position];

        public GlosValueArrayItemChecker(GlosValueArrayChecker checker, int position) {
            Checker = checker;
            Position = position;
        }

        public GlosValueArrayItemChecker MoveNext() {
            ++Position;
            return this;
        }

        public GlosValueArrayItemChecker AssertPositionInRange() {
            Assert.True(0 <= Position && Position < Checker.Length);
            return this;
        }

        public GlosValueArrayItemChecker AssertEnd() {
            Assert.True(Position == Checker.Length);
            return this;
        }

        public GlosValueArrayItemChecker AssertNil() {
            AssertPositionInRange();
            Current.AssertNil();
            return this;
        }

        public GlosValueArrayItemChecker AssertInteger() {
            AssertPositionInRange();
            Current.AssertInteger();
            return this;
        }

        public GlosValueArrayItemChecker AssertInteger(long value) {
            AssertPositionInRange();
            Assert.Equal(value, Current.AssertInteger());
            return this;
        }

        public GlosValueArrayItemChecker AssertInteger(Action<long> checker) {
            AssertPositionInRange();
            checker(Current.AssertInteger());
            return this;
        }

        public GlosValueArrayItemChecker AssertInteger(Predicate<long> predicate) {
            AssertPositionInRange();
            Assert.True(predicate(Current.AssertInteger()));
            return this;
        }

        public GlosValueArrayItemChecker AssertFloat() {
            AssertPositionInRange();
            Current.AssertFloat();
            return this;
        }

        public GlosValueArrayItemChecker AssertFloat(double value) {
            AssertPositionInRange();
            Assert.Equal(value, Current.AssertFloat());
            return this;
        }

        public GlosValueArrayItemChecker AssertFloatRelativeError(double value, double error) {
            AssertPositionInRange();
            Assert.InRange(Current.AssumeFloat(), value * (1 - error), value * (1 + error));
            return this;
        }

        public GlosValueArrayItemChecker AssertFloatAbsoluteError(double value, double error) {
            AssertPositionInRange();
            Assert.InRange(Current.AssumeFloat(), value - error, value + error);
            return this;
        }

        public GlosValueArrayItemChecker AssertFloat(Action<double> checker) {
            AssertPositionInRange();
            checker(Current.AssertFloat());
            return this;
        }

        public GlosValueArrayItemChecker AssertFloat(Predicate<double> predicate) {
            AssertPositionInRange();
            Assert.True(predicate(Current.AssertFloat()));
            return this;
        }

        public GlosValueArrayItemChecker AssertFloatRaw(ulong binaryRepresentation) {
            AssertPositionInRange();
            Assert.Equal(binaryRepresentation, Current.AssertFloatRaw());
            return this;
        }

        public GlosValueArrayItemChecker AssertFloatRaw(Action<ulong> binaryRepresentationChecker) {
            AssertPositionInRange();
            binaryRepresentationChecker(Current.AssertFloatRaw());
            return this;
        }

        public GlosValueArrayItemChecker AssertFloatRaw(Predicate<ulong> binaryRepresentationPredicate) {
            AssertPositionInRange();
            Assert.True(binaryRepresentationPredicate(Current.AssertFloatRaw()));
            return this;
        }

        public GlosValueArrayItemChecker AssertFloatInRange(float lower, float upper) {
            AssertPositionInRange();
            Assert.InRange(Current.AssertFloat(), lower, upper);
            return this;
        }

        public GlosValueArrayItemChecker AssertBoolean() {
            AssertPositionInRange();
            Current.AssertBoolean();
            return this;
        }

        public GlosValueArrayItemChecker AssertBoolean(bool value) {
            AssertPositionInRange();
            Assert.False(value ^ Current.AssertBoolean());
            return this;
        }

        public GlosValueArrayItemChecker AssertBoolean(Action<bool> checker) {
            AssertPositionInRange();
            checker(Current.AssertBoolean());
            return this;
        }

        public GlosValueArrayItemChecker AssertBoolean(Predicate<bool> predicate) {
            AssertPositionInRange();
            Assert.True(predicate(Current.AssertBoolean()));
            return this;
        }

        public GlosValueArrayItemChecker AssertTrue() {
            AssertPositionInRange();
            Assert.True(Current.AssertBoolean());
            return this;
        }

        public GlosValueArrayItemChecker AssertFalse() {
            AssertPositionInRange();
            Assert.False(Current.AssertBoolean());
            return this;
        }

        public GlosValueArrayItemChecker AssertString() {
            AssertPositionInRange();
            Current.AssertString();
            return this;
        }

        public GlosValueArrayItemChecker AssertString(string value) {
            AssertPositionInRange();
            Assert.Equal(value, Current.AssertString());
            return this;
        }

        public GlosValueArrayItemChecker AssertString(Action<string> checker) {
            AssertPositionInRange();
            checker(Current.AssertString());
            return this;
        }

        public GlosValueArrayItemChecker AssertString(Predicate<string> predicate) {
            AssertPositionInRange();
            Assert.True(predicate(Current.AssertString()));
            return this;
        }

        public GlosValueArrayItemChecker AssertTable() {
            AssertPositionInRange();
            Current.AssertTable();
            return this;
        }

        public GlosValueArrayItemChecker AssertTable(Action<GlosTable> checker) {
            AssertPositionInRange();
            checker(Current.AssertTable());
            return this;
        }

        public GlosValueArrayItemChecker AssertTable(Predicate<GlosTable> predicate) {
            AssertPositionInRange();
            Assert.True(predicate(Current.AssertTable()));
            return this;
        }

        public GlosValueArrayItemChecker AssertVector() {
            AssertPositionInRange();
            Current.AssertVector();
            return this;
        }

        public GlosValueArrayItemChecker AssertVector(Action<GlosVector> checker) {
            AssertPositionInRange();
            checker(Current.AssertVector());
            return this;
        }

        public GlosValueArrayItemChecker AssertVector(Predicate<GlosVector> predicate) {
            AssertPositionInRange();
            Assert.True(predicate(Current.AssertVector()));
            return this;
        }
    }

    public class GlosValueArrayChecker {
        public Memory<GlosValue> Target { get; }
        public int Length => Target.Length;

        public GlosValueArrayChecker(Memory<GlosValue> target) {
            Target = target;
        }

        public static GlosValueArrayChecker Create(Memory<GlosValue> target) => new GlosValueArrayChecker(target);

        public GlosValueArrayItemChecker FirstOne() => new GlosValueArrayItemChecker(this, 0);
    }
}
