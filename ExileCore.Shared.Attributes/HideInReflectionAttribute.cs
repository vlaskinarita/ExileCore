using System;

namespace ExileCore.Shared.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
public class HideInReflectionAttribute : Attribute
{
}
