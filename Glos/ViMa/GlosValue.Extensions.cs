﻿namespace GeminiLab.Glos.ViMa {
    public static class GlosValueExtensions {
        public static ref GlosValue SetNil(this ref GlosValue v) {
            v.ValueNumber.Integer = 0;
            v.ValueObject = null;
            v.Type = GlosValueType.Nil;

            return ref v;
        }

        public static ref GlosValue SetInteger(this ref GlosValue v, long value) {
            v.ValueNumber.Integer = value;
            v.ValueObject = null;
            v.Type = GlosValueType.Integer;
            
            return ref v;
        }

        public static ref GlosValue SetFloat(this ref GlosValue v, double value) {
            v.ValueNumber.Float = value;
            v.ValueObject = null;
            v.Type = GlosValueType.Float;

            return ref v;
        }

        public static ref GlosValue SetFloatByBinaryPresentation(this ref GlosValue v, ulong representation) {
            v.ValueNumber.Integer = unchecked((long)representation);
            v.ValueObject = null;
            v.Type = GlosValueType.Float;

            return ref v;
        }

        public static ref GlosValue SetBoolean(this ref GlosValue v, bool value) {
            v.ValueNumber.Integer = value ? -1L : 0L;
            v.ValueObject = null;
            v.Type = GlosValueType.Boolean;

            return ref v;
        }
        
        public static ref GlosValue SetTable(this ref GlosValue v, GlosTable value) {
            v.ValueNumber.Integer = 0;
            v.ValueObject = value;
            v.Type = GlosValueType.Table;

            return ref v;
        }

        public static ref GlosValue SetString(this ref GlosValue v, string value) {
            v.ValueNumber.Integer = 0;
            v.ValueObject = value;
            v.Type = GlosValueType.String;

            return ref v;
        }

        public static ref GlosValue SetFunction(this ref GlosValue v, GlosFunction fun) {
            v.ValueNumber.Integer = 0;
            v.ValueObject = fun;
            v.Type = GlosValueType.Function;

            return ref v;
        }

        public static ref GlosValue SetExternalFunction(this ref GlosValue v, GlosExternalFunction value) {
            v.ValueNumber.Integer = 0;
            v.ValueObject = value;
            v.Type = GlosValueType.ExternalFunction;

            return ref v;
        }


        public static void AssertNil(this in GlosValue v) {
            if (v.Type != GlosValueType.Nil) throw new GlosValueTypeAssertionFailedException(v, GlosValueType.Nil);
        }

        public static long AssertInteger(this in GlosValue v) {
            if (v.Type == GlosValueType.Integer) return v.ValueNumber.Integer;
            throw new GlosValueTypeAssertionFailedException(v, GlosValueType.Integer);
        }

        public static double AssertFloat(this in GlosValue v) {
            if (v.Type == GlosValueType.Float) return v.ValueNumber.Float;
            throw new GlosValueTypeAssertionFailedException(v, GlosValueType.Float);
        }

        public static bool AssertBoolean(this in GlosValue v) {
            if (v.Type == GlosValueType.Boolean) return v.ValueNumber.Integer != 0;
            throw new GlosValueTypeAssertionFailedException(v, GlosValueType.Boolean);
        }

        public static GlosTable AssertTable(this in GlosValue v) {
            if (v.Type == GlosValueType.Table && v.ValueObject is GlosTable table) return table;
            throw new GlosValueTypeAssertionFailedException(v, GlosValueType.Table);
        }

        public static string AssertString(this in GlosValue v) {
            if (v.Type == GlosValueType.String && v.ValueObject is string s) return s;
            throw new GlosValueTypeAssertionFailedException(v, GlosValueType.String);
        }

        public static GlosFunction AssertFunction(this in GlosValue v) {
            if (v.Type == GlosValueType.Function && v.ValueObject is GlosFunction fun) return fun;
            throw new GlosValueTypeAssertionFailedException(v, GlosValueType.Function);
        }

        public static GlosExternalFunction AssertExternalFunction(this in GlosValue v) {
            if (v.Type == GlosValueType.ExternalFunction && v.ValueObject is GlosExternalFunction fun) return fun;
            throw new GlosValueTypeAssertionFailedException(v, GlosValueType.ExternalFunction);
        }


        public static long AssumeInteger(this in GlosValue v) => v.ValueNumber.Integer;
        public static double AssumeFloat(this in GlosValue v) => v.ValueNumber.Float;
        public static bool AssumeBoolean(this in GlosValue v) => v.ValueNumber.Integer != 0;
        public static GlosTable AssumeTable(this in GlosValue v) => (GlosTable)v.ValueObject!;
        public static string AssumeString(this in GlosValue v) => (string)v.ValueObject!;
        public static GlosFunction AssumeFunction(this in GlosValue v) => (GlosFunction)v.ValueObject!;
        public static GlosExternalFunction AssumeExternalFunction(this in GlosValue v) => (GlosExternalFunction)v.ValueObject!;


        public static bool Truthy(this in GlosValue v) {
            if (v.Type == GlosValueType.Nil) return false;
            if (v.Type == GlosValueType.Boolean) return v.ValueNumber.Integer != 0;
            return true;
        }

        public static bool Falsey(this in GlosValue v) {
            if (v.Type == GlosValueType.Nil) return true;
            if (v.Type == GlosValueType.Boolean) return v.ValueNumber.Integer == 0;
            return false;
        }

        public static bool IsNil(this in GlosValue v) {
            return v.Type == GlosValueType.Nil;
        }

        public static bool IsNonNil(this in GlosValue v) {
            return v.Type != GlosValueType.Nil;
        }

        public static bool IsInvokable(this in GlosValue v) {
            return v.Type == GlosValueType.Function || v.Type == GlosValueType.ExternalFunction;
        }

        public static void AssertInvokable(this in GlosValue v) {
            if (!v.IsInvokable()) throw new GlosValueNotCallableException(v);
        }

        public static GlosValue[] Invoke(this in GlosValue v, GlosViMa vm, GlosValue[] args) {
            v.AssertInvokable();

            return v.Type == GlosValueType.Function ? vm.ExecuteFunction(v.AssertFunction(), args) : v.AssertExternalFunction().Invoke(args);
        }
    }
}
