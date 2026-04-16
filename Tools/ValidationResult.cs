using System.Collections.Generic;

namespace Tankathon.Tools;

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Violations { get; set; } = new();
    public List<string> Warnings { get; set; } = new();

    public ValidationResult(bool valid)
    {
        IsValid = valid;
    }

    public void AddViolation(string violation)
    {
        Violations.Add(violation);
        IsValid = false;
    }

    public void AddWarning(string warning)
    {
        Warnings.Add(warning);
    }

    public override string ToString()
    {
        var result = IsValid ? "VALID" : "INVALID";
        if (Violations.Count > 0)
        {
            result += $"\n\nViolations ({Violations.Count}):";
            foreach (var violation in Violations)
            {
                result += $"\n - {violation}";
            }
        }
        if (Warnings.Count > 0)
        {
            result += $"\n\nWarnings ({Warnings.Count}):";
            foreach (var warning in Warnings)
            {
                result += $"\n - {warning}";
            }
        }
        return result;
    }
}
