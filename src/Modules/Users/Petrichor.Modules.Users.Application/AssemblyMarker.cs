using System.Reflection;

namespace Petrichor.Modules.Users.Application;

public static class AssemblyMarker
{
    public static readonly Assembly Assembly = typeof(AssemblyMarker).Assembly;
}
