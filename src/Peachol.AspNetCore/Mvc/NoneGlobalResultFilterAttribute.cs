namespace Microsoft.AspNetCore.Mvc;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class NoneGlobalResultFilterAttribute : Attribute;