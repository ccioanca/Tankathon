using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Tankathon.Tools;

public static class TankCodeValidator
{
    #region Blacklist Definitions

    private static readonly string[] BlacklistNamespaces =
    {
        "System.Reflection",
        "System.Runtime.CompilerServices",
        "System.Runtime.InteropServices",
        "Tankathon.API.Internal",
        "System.IO",
        "System.Net",
        "System.Threading",
        "System.Threading.Tasks"
    };

    // Godot API calls that can manipulate engine/scene state
    private static readonly string[] BlacklistGodotApis =
    {
        // Engine Access
        "Engine.GetMainLoop",
        "Engine.GetSingleton",
        "Engine.TimeScale",

        // Scene Tree Access
        "SceneTree.Root",
        "SceneTree.CurrentScene",
        "SceneTree.Quit",
        "SceneTree.Paused",

        // Node Manipulation
        "GetNode",
        "GetNodeOrNull",
        "GetTree",
        "AddChild",
        "RemoveChild",
        "QueueFree",
        "GetParent",
        "GetChild",
        "FindChild",

        // Resource Loading
        "GD.Load",
        "ResourceLoader.Load",
        "ResourceLoader.Exists",

        // System Access
        "OS.Execute",
        "OS.Kill",
        "OS.ShellOpen",
        "ProjectSettings.Get",
        "ProjectSettings.Set",

        // Input Access (bypasses API)
        "Input.IsActionPressed",
        "Input.IsKeyPressed",
        "Input.GetMousePosition",
        "Input.GetVector",

        // File System
        "FileAccess.Open",
        "DirAccess.Open"
    };

    private static readonly string[] BlacklistTypes =
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

    private static readonly string[] BlacklistCastPatterns =
    {
        "Tankathon.API.Internal",
        "Actions",
        "TheTank",
        "GameManager"
    };
    #endregion



    /// <summary>
    /// Validates all C# files in a directory recursively.
    /// This is the primary entry point for validating tank submissions.
    /// </summary>
    /// <param name="directoryPath">Path to the directory containing tank code</param>
    /// <returns>ValidationResult with aggregated violations from all files</returns>
    public static ValidationResult ValidateDirectory(string directoryPath)
    {
        var result = new ValidationResult(true);

        if (!Directory.Exists(directoryPath))
        {
            result.AddViolation($"Directory not found: '{directoryPath}'");
            return result;
        }

        // Get all .cs files - recursively + subdirectories
        var csFiles = Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories);

        if (csFiles.Length == 0)
        {
            result.AddWarning($"No C# files found in directory: '{directoryPath}'");
            return result;
        }

        foreach (var file in csFiles)
        {
            var fileName = Path.GetFileName(file);
            var fileResult = ValidateFile(file);

            // Aggregate violations with file context
            foreach (var violation in fileResult.Violations)
            {
                result.AddViolation($"[{fileName}] {violation}");
            }

            // Aggregate warnings 
            foreach (var warning in fileResult.Warnings)
            {
                result.AddWarning($"[{fileName}] {warning}");
            }
        }

        return result;
    }

    //privated this method cause I don't think it'll be used by any competitors.
    private static ValidationResult ValidateFile(string filePath)
    {
        var result = new ValidationResult(true);

        if (!File.Exists(filePath))
        {
            result.AddWarning($"File not found: '{filePath}' (Check to ensure you're providing a real path)");
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
            result.AddWarning("Source code is empty or null");
            return result;
        }

        try
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
            var root = syntaxTree.GetRoot();

            ValidateUsingDirectives(root, result);
            ValidateBlacklistTypes(root, result);
            ValidateBlacklistKeywords(root, result);
            ValidateBlacklistCasts(root, result);
            ValidateTaskUsage(root, result);
            ValidateGodotApiCalls(root, result);
        }
        catch (Exception ex)
        {
            result.AddViolation($"Failed to parse code: {ex.Message}");
        }

        return result;
    }
    


    #region Blacklist Item Validation
    
    private static void ValidateUsingDirectives(SyntaxNode root, ValidationResult result)
    {
        var usingDirectives = root.DescendantNodes().OfType<UsingDirectiveSyntax>();

        foreach (var usingDirective in usingDirectives)
        {
            var namespaceName = usingDirective.Name?.ToString();
            if (string.IsNullOrEmpty(namespaceName))
                continue;

            // Check for blacklisted namespaces
            foreach (var blacklisted in BlacklistNamespaces)
            {
                if (namespaceName.Equals(blacklisted, StringComparison.OrdinalIgnoreCase) ||
                    namespaceName.StartsWith(blacklisted + ".", StringComparison.OrdinalIgnoreCase))
                {
                    result.AddViolation($"Forbidden namespace: '{namespaceName}' (Line {usingDirective.GetLocation().GetLineSpan().StartLinePosition.Line + 1})");
                }
            }
        }
    }

    private static void ValidateBlacklistTypes(SyntaxNode root, ValidationResult result)
    {
        var identifiers = root.DescendantNodes().OfType<IdentifierNameSyntax>();

        foreach (var identifier in identifiers)
        {
            var identifierText = identifier.Identifier.Text;

            foreach (var BlacklistType in BlacklistTypes)
            {
                if (identifierText.Equals(BlacklistType, StringComparison.OrdinalIgnoreCase))
                {
                    var lineNumber = identifier.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    result.AddViolation($"Blacklist type/method: '{identifierText}' (Line {lineNumber})");
                }
            }
        }
    }

    private static void ValidateBlacklistKeywords(SyntaxNode root, ValidationResult result)
    {
        // Check for unsafe keyword
        var unsafeStatements = root.DescendantNodes().OfType<UnsafeStatementSyntax>();
        foreach (var unsafeStmt in unsafeStatements)
        {
            var lineNumber = unsafeStmt.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
            result.AddViolation($"Blacklist keyword: 'unsafe' (Line {lineNumber})");
        }

        // Check for async keyword
        var asyncMethods = root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Where(m => m.Modifiers.Any(SyntaxKind.AsyncKeyword));
        foreach (var method in asyncMethods)
        {
            var lineNumber = method.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
            result.AddViolation($"Blacklist keyword: 'async' in method '{method.Identifier.Text}' (Line {lineNumber})");
        }

        // Check for await keyword
        var awaitExpressions = root.DescendantNodes().OfType<AwaitExpressionSyntax>();
        foreach (var awaitExpr in awaitExpressions)
        {
            var lineNumber = awaitExpr.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
            result.AddViolation($"Blacklist keyword: 'await' (Line {lineNumber})");
        }
    }

    private static void ValidateBlacklistCasts(SyntaxNode root, ValidationResult result)
    {
        // Check for "as" casts
        var binaryExpressions = root.DescendantNodes()
            .OfType<BinaryExpressionSyntax>()
            .Where(b => b.IsKind(SyntaxKind.AsExpression));

        foreach (var cast in binaryExpressions)
        {
            var castType = cast.Right.ToString();

            foreach (var BlacklistPattern in BlacklistCastPatterns)
            {
                if (castType.Contains(BlacklistPattern, StringComparison.OrdinalIgnoreCase))
                {
                    var lineNumber = cast.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    result.AddViolation($"Blacklist cast: 'as {castType}' (Line {lineNumber})");
                }
            }
        }

        // Check for direct casts (Type)
        var castExpressions = root.DescendantNodes().OfType<CastExpressionSyntax>();
        foreach (var cast in castExpressions)
        {
            var castType = cast.Type.ToString();

            foreach (var BlacklistPattern in BlacklistCastPatterns)
            {
                if (castType.Contains(BlacklistPattern, StringComparison.OrdinalIgnoreCase))
                {
                    var lineNumber = cast.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    result.AddViolation($"Blacklist cast: '({castType})' (Line {lineNumber})");
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
                result.AddViolation($"Blacklist type: 'Task<T>' (Line {lineNumber})");
            }
        }

        // Check for Thread usage
        var identifiers = root.DescendantNodes().OfType<IdentifierNameSyntax>();
        foreach (var identifier in identifiers)
        {
            if (identifier.Identifier.Text.Equals("Thread", StringComparison.OrdinalIgnoreCase))
            {
                var lineNumber = identifier.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                result.AddViolation($"Blacklist type: 'Thread' (Line {lineNumber})");
            }
        }
    }

    //The Godot namespace is a bit of a special case since there's things in there that are fine to use - like vectors and mathf - but others that can manipulate the scene or engine states. This is validated here.
    private static void ValidateGodotApiCalls(SyntaxNode root, ValidationResult result)
    {
        var memberAccess = root.DescendantNodes().OfType<MemberAccessExpressionSyntax>();
        var invocations = root.DescendantNodes().OfType<InvocationExpressionSyntax>();

        // Check member access expressions
        foreach (var access in memberAccess)
        {
            var fullText = access.ToString();

            foreach (var dangerousApi in BlacklistGodotApis)
            {
                if (fullText.Contains(dangerousApi, StringComparison.OrdinalIgnoreCase))
                {
                    var lineNumber = access.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    result.AddViolation($"Forbidden Godot API call: '{dangerousApi}' detected in '{fullText}' (Line {lineNumber})");
                }
            }
        }

        // Also check method invocations to catch calls like GetNode("path")
        foreach (var invocation in invocations)
        {
            var invocationText = invocation.ToString();

            foreach (var dangerousApi in BlacklistGodotApis)
            {
                // Check if the dangerous API is part of the invocation
                if (invocationText.Contains(dangerousApi, StringComparison.OrdinalIgnoreCase))
                {
                    var lineNumber = invocation.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    result.AddViolation($"Forbidden Godot API call: '{dangerousApi}' (Line {lineNumber})");
                }
            }
        }
    }

    #endregion

}
