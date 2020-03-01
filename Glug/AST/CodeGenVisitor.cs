﻿using System;
using System.Linq;

using GeminiLab.Glos.CodeGenerator;
using GeminiLab.Glos.ViMa;

namespace GeminiLab.Glug.AST {
    public class CodeGenVisitor : RecursiveVisitor {
        public GlosUnitBuilder Builder { get; } = new GlosUnitBuilder();

        public GlosFunctionBuilder? CurrentFunction { get; private set; }

        public override void VisitFunction(Function val) {
            var fun = Builder.AddFunction();
            var oldFun = CurrentFunction;
            CurrentFunction = fun;

            if (oldFun == null) fun.SetEntry();

            var variables = val.VariableTable.Variables.Values.ToArray();
            fun.VariableInContext = variables.Where(x => x.Place == VariablePlace.Context).Select(x => x.Name).ToArray();

            foreach (var variable in variables.Where(x => x.Place == VariablePlace.LocalVariable)) {
                variable.LocalVariable = fun.AllocateLocalVariable();
            }

            foreach (var variable in variables.Where(x => x.IsArgument && x.Place != VariablePlace.Argument)) {
                fun.AppendLdArg(variable.ArgumentId);
                if (variable.Place == VariablePlace.LocalVariable) {
                    fun.AppendStLoc(variable.LocalVariable!);
                } else if (variable.Place == VariablePlace.Context) {
                    fun.AppendLdStr(variable.Name);
                    fun.AppendUvc();
                }
            }

            base.VisitFunction(val);

            CurrentFunction.AppendRetIfNone();

            CurrentFunction = oldFun;
            if (CurrentFunction != null) {
                CurrentFunction.AppendLdFun(fun);
                CurrentFunction.AppendBind();

                if (val.Self != null) {
                    CurrentFunction.AppendDup();
                    val.Self.CreateStoreInstr(CurrentFunction);
                }
            }
        }

        public override void VisitIf(If val) {
            int brc = val.Branches.Count;
            Label? nextLabel = null;
            Label endLabel = CurrentFunction!.AllocateLabel();

            for (int i = 0; i < brc; ++i) {
                if (nextLabel != null) CurrentFunction!.InsertLabel(nextLabel);
                nextLabel = CurrentFunction!.AllocateLabel();

                var (cond, expr) = val.Branches[i];
                visitAndConvertResultToValue(cond);
                CurrentFunction!.AppendBf(nextLabel);
                
                if (val.IsOnStackList) visitAndConvertResultToOsl(expr);
                else visitAndConvertResultToValue(expr);

                CurrentFunction!.AppendB(endLabel);
            }

            CurrentFunction!.InsertLabel(nextLabel!); // brc must be at least 1 when this ast is well-formed
            if (val.ElseBranch != null) {
                if (val.IsOnStackList) visitAndConvertResultToOsl(val.ElseBranch);
                else visitAndConvertResultToValue(val.ElseBranch);
            } else {
                if (val.IsOnStackList) CurrentFunction!.AppendLdDel();
                else CurrentFunction!.AppendLdNil();
            }

            CurrentFunction.InsertLabel(endLabel);
        }

        public override void VisitWhile(While val) {
            base.VisitWhile(val);
        }

        public override void VisitReturn(Return val) {
            visitAndConvertResultToOsl(val.Expr);

            CurrentFunction!.AppendRet();
        }

        public override void VisitOnStackList(OnStackList val) {
            CurrentFunction!.AppendLdDel();

            foreach (var item in val.List) {
                visitAndConvertResultToValue(item);
            }
        }

        public override void VisitBlock(Block val) {
            var count = val.List.Count;

            if (count == 0) {
                CurrentFunction!.AppendLdNil();
            } else {
                for (int i = 0; i < count; ++i) {
                    Visit(val.List[i]);

                    if (i != count - 1) {
                        if (val.List[i].IsOnStackList) {
                            CurrentFunction!.AppendShpRv(0);
                        } else {
                            CurrentFunction!.AppendPop();
                        }
                    }
                }
            }
        }

        private void visitAndConvertResultToValue(Expr expr) {
            Visit(expr);

            if (expr.IsOnStackList) CurrentFunction!.AppendShpRv(1);
        }

        private void visitAndConvertResultToOsl(Expr expr) {
            if (!expr.IsOnStackList) CurrentFunction!.AppendLdDel();

            Visit(expr);
        }

        public override void VisitUnOp(UnOp val) {
            visitAndConvertResultToValue(val.Expr);

            switch (val.Op) {
            case GlugUnOpType.Neg:
                CurrentFunction!.AppendNeg();
                break;
            case GlugUnOpType.Not:
                CurrentFunction!.AppendNot();
                break;
            }
        }

        public override void VisitBiOp(BiOp val) {
            if (val.Op == GlugBiOpType.Call) {
                visitAndConvertResultToOsl(val.ExprR);
                visitAndConvertResultToValue(val.ExprL);
                CurrentFunction!.AppendCall();
            } else if (val.Op == GlugBiOpType.Assign) {
                if (val.ExprL.IsOnStackList) {
                    visitAndConvertResultToOsl(val.ExprR);

                    var list = ((OnStackList)(val.ExprL)).List;
                    var count = list.Count;

                    CurrentFunction!.AppendShpRv(count);
                    for (int i = count - 1; i >= 0; --i) ((VarRef)(list[i])).Variable.CreateStoreInstr(CurrentFunction!);

                    CurrentFunction.AppendLdNil();
                } else if (val.ExprL is VarRef vr) {
                    visitAndConvertResultToValue(val.ExprR);
                    CurrentFunction!.AppendDup();
                    vr.Variable.CreateStoreInstr(CurrentFunction!);
                } else if (val.ExprL is BiOp { Op: GlugBiOpType.Index } ind) {
                    visitAndConvertResultToValue(val.ExprR);
                    CurrentFunction!.AppendDup();
                    visitAndConvertResultToValue(ind.ExprL);
                    visitAndConvertResultToValue(ind.ExprR);
                    CurrentFunction!.AppendUen();
                } else if (val.ExprL is Metatable mt) {
                    visitAndConvertResultToValue(val.ExprR);
                    CurrentFunction!.AppendDup();
                    visitAndConvertResultToValue(mt.Table);
                    CurrentFunction!.AppendSmt();
                } else {
                    throw new ArgumentOutOfRangeException();
                }
            } else {
                visitAndConvertResultToValue(val.ExprL);
                visitAndConvertResultToValue(val.ExprR);

                CurrentFunction!.AppendInstruction(val.Op switch {
                    GlugBiOpType.Add => GlosOp.Add,
                    GlugBiOpType.Sub => GlosOp.Sub,
                    GlugBiOpType.Mul => GlosOp.Mul,
                    GlugBiOpType.Div => GlosOp.Div,
                    GlugBiOpType.Mod => GlosOp.Mod,
                    GlugBiOpType.Lsh => GlosOp.Lsh,
                    GlugBiOpType.Rsh => GlosOp.Rsh,
                    GlugBiOpType.And => GlosOp.And,
                    GlugBiOpType.Orr => GlosOp.Orr,
                    GlugBiOpType.Xor => GlosOp.Xor,
                    GlugBiOpType.Gtr => GlosOp.Gtr,
                    GlugBiOpType.Lss => GlosOp.Lss,
                    GlugBiOpType.Geq => GlosOp.Geq,
                    GlugBiOpType.Leq => GlosOp.Leq,
                    GlugBiOpType.Equ => GlosOp.Equ,
                    GlugBiOpType.Neq => GlosOp.Neq,
                    GlugBiOpType.Index => GlosOp.Ren,
                    _ => GlosOp.Nop, // Add a exception here
                });
            }
        }

        public override void VisitLiteralInteger(LiteralInteger val) {
            CurrentFunction!.AppendLd(val.Value);
        }

        public override void VisitLiteralBool(LiteralBool val) {
            if (val.Value) CurrentFunction!.AppendLdTrue();
            else CurrentFunction!.AppendLdFalse();
        }

        public override void VisitLiteralString(LiteralString val) {
            CurrentFunction!.AppendLdStr(val.Value);
        }

        public override void VisitLiteralNil(LiteralNil val) {
            CurrentFunction!.AppendLdNil();
        }

        public override void VisitVarRef(VarRef val) {
            val.Variable.CreateLoadInstr(CurrentFunction!);
        }

        public override void VisitTableDef(TableDef val) {
            CurrentFunction!.AppendLdNTbl();

            foreach (var (key, value) in val.Pairs) {
                visitAndConvertResultToValue(key);
                visitAndConvertResultToValue(value);
                CurrentFunction.AppendIen();
            }
        }

        public override void VisitMetatable(Metatable val) {
            visitAndConvertResultToValue(val.Table);
            CurrentFunction!.AppendGmt();
        }
    }
}
