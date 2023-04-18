//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.11.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from E:/Compi/Compilador/Compilador/Components\AlphaParser.g4 by ANTLR 4.11.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace generated {
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="AlphaParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.11.1")]
[System.CLSCompliant(false)]
public interface IAlphaParserVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by the <c>ProgramClassAST</c>
	/// labeled alternative in <see cref="AlphaParser.program"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitProgramClassAST([NotNull] AlphaParser.ProgramClassASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>UsingClassAST</c>
	/// labeled alternative in <see cref="AlphaParser.using"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitUsingClassAST([NotNull] AlphaParser.UsingClassASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>VarDeclAST</c>
	/// labeled alternative in <see cref="AlphaParser.varDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVarDeclAST([NotNull] AlphaParser.VarDeclASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>ClassDeclAST</c>
	/// labeled alternative in <see cref="AlphaParser.classDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitClassDeclAST([NotNull] AlphaParser.ClassDeclASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>MethodDeclAST</c>
	/// labeled alternative in <see cref="AlphaParser.methodDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMethodDeclAST([NotNull] AlphaParser.MethodDeclASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>FormParsAST</c>
	/// labeled alternative in <see cref="AlphaParser.formPars"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFormParsAST([NotNull] AlphaParser.FormParsASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>TypeAST</c>
	/// labeled alternative in <see cref="AlphaParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTypeAST([NotNull] AlphaParser.TypeASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>AssignStatementAST</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAssignStatementAST([NotNull] AlphaParser.AssignStatementASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>IfStatementAST</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIfStatementAST([NotNull] AlphaParser.IfStatementASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>ForStatementAST</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitForStatementAST([NotNull] AlphaParser.ForStatementASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>WhileStatementAST</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitWhileStatementAST([NotNull] AlphaParser.WhileStatementASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>BreakStatementAST</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBreakStatementAST([NotNull] AlphaParser.BreakStatementASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>ReturnStatementAST</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitReturnStatementAST([NotNull] AlphaParser.ReturnStatementASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>ReadStatementAST</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitReadStatementAST([NotNull] AlphaParser.ReadStatementASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>WriteStatementAST</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitWriteStatementAST([NotNull] AlphaParser.WriteStatementASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>BlockStatementAST</c>
	/// labeled alternative in <see cref="AlphaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBlockStatementAST([NotNull] AlphaParser.BlockStatementASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>BlockAST</c>
	/// labeled alternative in <see cref="AlphaParser.block"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBlockAST([NotNull] AlphaParser.BlockASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>ActParsAST</c>
	/// labeled alternative in <see cref="AlphaParser.actPars"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitActParsAST([NotNull] AlphaParser.ActParsASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>ConditionAST</c>
	/// labeled alternative in <see cref="AlphaParser.condition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitConditionAST([NotNull] AlphaParser.ConditionASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>CondTermAST</c>
	/// labeled alternative in <see cref="AlphaParser.condTerm"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCondTermAST([NotNull] AlphaParser.CondTermASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>CondFactAST</c>
	/// labeled alternative in <see cref="AlphaParser.condFact"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCondFactAST([NotNull] AlphaParser.CondFactASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>CastAST</c>
	/// labeled alternative in <see cref="AlphaParser.cast"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCastAST([NotNull] AlphaParser.CastASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>ExprAST</c>
	/// labeled alternative in <see cref="AlphaParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExprAST([NotNull] AlphaParser.ExprASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>TermAST</c>
	/// labeled alternative in <see cref="AlphaParser.term"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTermAST([NotNull] AlphaParser.TermASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>DesignatorFactorAST</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDesignatorFactorAST([NotNull] AlphaParser.DesignatorFactorASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>NumberFactorAST</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNumberFactorAST([NotNull] AlphaParser.NumberFactorASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>CharFactorAST</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCharFactorAST([NotNull] AlphaParser.CharFactorASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>StringFactorAST</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStringFactorAST([NotNull] AlphaParser.StringFactorASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>IntFactorAST</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIntFactorAST([NotNull] AlphaParser.IntFactorASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>DoubleFactorAST</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDoubleFactorAST([NotNull] AlphaParser.DoubleFactorASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>BoolFactorAST</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBoolFactorAST([NotNull] AlphaParser.BoolFactorASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>NewFactorAST</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNewFactorAST([NotNull] AlphaParser.NewFactorASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>ParenFactorAST</c>
	/// labeled alternative in <see cref="AlphaParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitParenFactorAST([NotNull] AlphaParser.ParenFactorASTContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>DesignatorAST</c>
	/// labeled alternative in <see cref="AlphaParser.designator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDesignatorAST([NotNull] AlphaParser.DesignatorASTContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AlphaParser.relop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRelop([NotNull] AlphaParser.RelopContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AlphaParser.addop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAddop([NotNull] AlphaParser.AddopContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AlphaParser.mulop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMulop([NotNull] AlphaParser.MulopContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AlphaParser.ident"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIdent([NotNull] AlphaParser.IdentContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AlphaParser.array"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitArray([NotNull] AlphaParser.ArrayContext context);
}
} // namespace generated
