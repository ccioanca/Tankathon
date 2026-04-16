using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Tankathon.Tools;

public static class TankCodeValidator
{
    private static readonly string[] ForbiddenNamespaces =
    {
        "System.Reflection",
        "System.Runtime.CompilerServices",
        "System.Runtime.InteropServices",
        "Tankathon.API.Internal",
        "Godot.Engine",
        "Godot.SceneTree",
        "System.IO",
        "System.Net",
        "System.Threading",
        "System.Threading.Tasks"
    };

    private static readonly string[] ForbiddenTypes =
    {
        "BindingFlags",
        "GetField",
        "GetProperty",
        "GetMethod",
        "GetValue",
        "SetValue",
        "FieldInfo",
        "PropertyInfo",
        "MethodInfo",
        "Activator"
    };

    private static readonly string[] ForbiddenKeywords =
    {
        "unsafe",
        "async",
        "await"
    };

    private static readonly string[] ForbiddenCastPatterns =
    {
        "Tankathon.API.Internal",
        "Actions",
        "TheTank",
        "GameManager"
    };

    public static ValidationResult ValidateFile(string filePath)
    {
        var result = new ValidationResult(true);

        if (!File.Exists(filePath))
        {
            result.AddViolation($"File not found: '{filePath}'");
            return result;
        }

        try
        {
            var sourceCode = File.ReadAllText(filePath);
            return ValidateCode(sourceCode);
        }
        catch (Exception ex)
        {
            result.AddViolation($"Failed to read file: {ex.Message}");
            return result;
        }
    }

    private static ValidationResult ValidateCode(string sourceCode)
    {
        var result = new ValidationResult(true);

        if (string.IsNullOrWhiteSpace(sourceCode))
        {
            result.AddViolation("Source code is empty or null");
            return result;
        }

        try
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
            var root = syntaxTree.GetRoot();

            ValidateUsingDirectives(root, result);
            ValidateForbiddenTypes(root, result);
            ValidateForbiddenKeywords(root, result);
            ValidateForbiddenCasts(root, result);
            ValidateTaskUsage(root, result);
            ValidateGodotUsage(root, result);
        }
        catch (Exception ex)
        {
            result.AddViolation($"Failed to parse code: {ex.Message}");
        }

        return result;
    }

    private static void ValidateUsingDirectives(SyntaxNode root, ValidationResult result)
    {
        var usingDirectives = root.DescendantNodes().OfType<UsingDirectiveSyntax>();

        foreach (var usingDirective in usingDirectives)
        {
            var namespaceName = usingDirective.Name?.ToString();
            if (string.IsNullOrEmpty(namespaceName))
                continue;

            // Check for forbidden namespaces
            foreach (var forbidden in ForbiddenNamespaces)
            {
                if (namespaceName.Equals(forbidden, StringComparison.OrdinalIgnoreCase) ||
                    namespaceName.StartsWith(forbidden + ".", StringComparison.OrdinalIgnoreCase))
                {
                    result.AddViolation($"Forbidden namespace: '{namespaceName}' (Line {usingDirective.GetLocation().GetLineSpan().StartLinePosition.Line + 1})");
                }
            }

            // Check for direct Godot namespace (not aliased as GD)
            if (usingDirective.Alias == null && namespaceName.Equals("Godot", StringComparison.OrdinalIgnoreCase))
            {
                result.AddViolation($"Direct 'using Godot;' is forbidden. Use 'using GD = Godot.GD;' instead (Line {usingDirective.GetLocation().GetLineSpan().StartLinePosition.Line + 1})");
            }
        }
    }

    private static void ValidateForbiddenTypes(SyntaxNode root, ValidationResult result)
    {
        var identifiers = root.DescendantNodes().OfType<IdentifierNameSyntax>();

        foreach (var identifier in identifiers)
        {
            var identifierText = identifier.Identifier.Text;

            foreach (var forbiddenType in ForbiddenTypes)
            {
                if (identifierText.Equals(forbiddenType, StringComparison.OrdinalIgnoreCase))
                {
                    var lineNumber = identifier.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    result.AddViolation($"Forbidden type/method: '{identifierText}' (Line {lineNumber})");
                }
            }
        }
    }

    private static void ValidateForbiddenKeywords(SyntaxNode root, ValidationResult result)
    {
        // Check for unsafe keyword
        var unsafeStatements = root.DescendantNodes().OfType<UnsafeStatementSyntax>();
        foreach (var unsafeStmt in unsafeStatements)
        {
            var lineNumber = unsafeStmt.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
            result.AddViolation($"Forbidden keyword: 'unsafe' (Line {lineNumber})");
        }

        // Check for async keyword
        var asyncMethods = root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Where(m => m.Modifiers.Any(SyntaxKind.AsyncKeyword));
        foreach (var method in asyncMethods)
        {
            var lineNumber = method.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
            result.AddViolation($"Forbidden keyword: 'async' in method '{method.Identifier.Text}' (Line {lineNumber})");
        }

        // Check for await keyword
        var awaitExpressions = root.DescendantNodes().OfType<AwaitExpressionSyntax>();
        foreach (var awaitExpr in awaitExpressions)
        {
            var lineNumber = awaitExpr.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
            result.AddViolation($"Forbidden keyword: 'await' (Line {lineNumber})");
        }
    }

    private static void ValidateForbiddenCasts(SyntaxNode root, ValidationResult result)
    {
        // Check for "as" casts
        var binaryExpressions = root.DescendantNodes()
            .OfType<BinaryExpressionSyntax>()
            .Where(b => b.IsKind(SyntaxKind.AsExpression));

        foreach (var cast in binaryExpressions)
        {
            var castType = cast.Right.ToString();

            foreach (var forbiddenPattern in ForbiddenCastPatterns)
            {
                if (castType.Contains(forbiddenPattern, StringComparison.OrdinalIgnoreCase))
                {
                    var lineNumber = cast.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    result.AddViolation($"Forbidden cast: 'as {castType}' (Line {lineNumber})");
                }
            }
        }

        // Check for direct casts (Type)
        var castExpressions = root.DescendantNodes().OfType<CastExpressionSyntax>();
        foreach (var cast in castExpressions)
        {
            var castType = cast.Type.ToString();

            foreach (var forbiddenPattern in ForbiddenCastPatterns)
            {
                if (castType.Contains(forbiddenPattern, StringComparison.OrdinalIgnoreCase))
                {
                    var lineNumber = cast.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    result.AddViolation($"Forbidden cast: '({castType})' (Line {lineNumber})");
                }
            }
        }
    }

    private static void ValidateTaskUsage(SyntaxNode root, ValidationResult result)
    {
        // Check for Task<T> or Task usage
        var genericNames = root.DescendantNodes().OfType<GenericNameSyntax>();
        foreach (var generic in genericNames)
        {
            if (generic.Identifier.Text.Equals("Task", StringComparison.OrdinalIgnoreCase))
            {
                var lineNumber = generic.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                result.AddViolation($"Forbidden type: 'Task<T>' (Line {lineNumber})");
            }
        }

        // Check for Thread usage
        var identifiers = root.DescendantNodes().OfType<IdentifierNameSyntax>();
        foreach (var identifier in identifiers)
        {
            if (identifier.Identifier.Text.Equals("Thread", StringComparison.OrdinalIgnoreCase))
            {
                var lineNumber = identifier.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                result.AddViolation($"Forbidden type: 'Thread' (Line {lineNumber})");
            }
        }
    }

    private static void ValidateGodotUsage(SyntaxNode root, ValidationResult result)
    {
        // Check for Godot API access patterns (Engine, SceneTree, etc.)
        var memberAccess = root.DescendantNodes().OfType<MemberAccessExpressionSyntax>();

        foreach (var access in memberAccess)
        {
            var fullText = access.ToString();

            // Check for Engine.GetMainLoop() or similar Godot API calls
            if (fullText.Contains("Engine.GetMainLoop", StringComparison.OrdinalIgnoreCase) ||
                fullText.Contains("SceneTree.Root", StringComparison.OrdinalIgnoreCase) ||
                fullText.Contains("GD.Load", StringComparison.OrdinalIgnoreCase))
            {
                var lineNumber = access.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                result.AddViolation($"Forbidden Godot API access: '{fullText}' (Line {lineNumber})");
            }
        }
    }


}
