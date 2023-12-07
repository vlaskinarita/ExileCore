using System;
using System.Diagnostics.CodeAnalysis;

namespace ExileCore.Shared.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class ConditionalDisplayAttribute : Attribute
{
	public string ConditionMethodName { get; }

	public bool ComparisonValue { get; }

	public ConditionalDisplayAttribute([NotNull] string conditionMethodName, bool comparisonValue = true)
	{
		ConditionMethodName = conditionMethodName;
		ComparisonValue = comparisonValue;
	}
}
