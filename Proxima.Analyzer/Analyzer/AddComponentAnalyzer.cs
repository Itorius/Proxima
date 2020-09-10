using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzer
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class AddComponentAnalyzer : DiagnosticAnalyzer
	{
		internal const string Title = "Using incompatiable arguments in variadic method";
		internal const string MessageFormat = "{0}";
		internal const string Description = "Check the API for correct usage.";
		internal const string Category = "Syntax";

		internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor("PX0001", Title, MessageFormat, Category, DiagnosticSeverity.Error, true, Description);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze);
			context.EnableConcurrentExecution();
			context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
		}

		private void AnalyzeNode(SyntaxNodeAnalysisContext context)
		{
			var invocationExpr = (InvocationExpressionSyntax) context.Node;
			if (!IsValidMethod(context, invocationExpr, out var methodSymbol)) return;

			var constructors = ((INamedTypeSymbol) methodSymbol.ReturnType).Constructors;

			if (constructors.All(ctor => invocationExpr.ArgumentList.Arguments.Count != ctor.Parameters.Length))
			{
				var diagnostic = Diagnostic.Create(Rule, invocationExpr.GetLocation(), "Wrong method parameters count");
				context.ReportDiagnostic(diagnostic);
				return;
			}

			var invocationTypes = invocationExpr.ArgumentList.Arguments.Select(argument => context.SemanticModel.GetTypeInfo(argument.Expression).Type).ToList();

			if (!constructors.Any(constructor =>
			{
				var parameters = constructor.Parameters.Select(parameter => parameter.Type).ToList();
				if (parameters.Count != invocationTypes.Count) return false;

				var flag = true;
				for (var i = 0; i < parameters.Count; i++) flag &= context.Compilation.ClassifyConversion(parameters[i], invocationTypes[i]).IsImplicit;
				return flag;
			}))
			{
				var diagnostic = Diagnostic.Create(Rule, invocationExpr.GetLocation(), "Invalid method parameters");
				context.ReportDiagnostic(diagnostic);
			}
		}

		private static bool IsValidMethod(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocationExpr, out IMethodSymbol outMethodSymbol)
		{
			var objectType = context.SemanticModel.Compilation.GetTypeByMetadataName("System.Object");
			var arrayType = context.SemanticModel.Compilation.CreateArrayTypeSymbol(objectType);

			if (
				context.SemanticModel.GetSymbolInfo(invocationExpr).Symbol is IMethodSymbol methodSymbol &&
				methodSymbol.Parameters.Length == 1 &&
				methodSymbol.Parameters[0].IsParams &&
				methodSymbol.TypeArguments.Length == 1 &&
				SymbolEqualityComparer.Default.Equals(methodSymbol.Parameters[0].Type, arrayType))
			{
				outMethodSymbol = methodSymbol;
				return true;
			}

			//if (!(context.SemanticModel.GetSymbolInfo(invocationExpr).Symbol is IMethodSymbol methodSymbol) || methodSymbol.Parameters.Length != 1|| methodSymbol.TypeArguments.Length != 1) return false;

			//if (methodSymbol.Parameters[0].IsParams && SymbolEqualityComparer.Default.Equals(methodSymbol.Parameters[0].Type, arrayType)) return false;
			//outMethodSymbol = methodSymbol;

			outMethodSymbol = null;
			return false;
		}
	}
}