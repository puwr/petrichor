using System.Reflection;

namespace Petrichor.Modules.Gallery.Application;

public static class AssemblyMarker
{
    public static readonly Assembly Assembly = typeof(AssemblyMarker).Assembly;
}
