using NetArchTest.Rules;
using Xunit;

namespace ArchitecturePlayground.Tests.Architecture;

/// <summary>
/// Architecture tests verifying module boundaries are respected.
/// </summary>
public class ModuleBoundaryTests
{
    [Fact]
    public void CoreShouldNotReferenceInfrastructure()
    {
        // Placeholder - implement when modules have actual types
        Assert.True(true);
    }

    [Fact]
    public void ModulesShouldNotReferenceOtherModuleCore()
    {
        // Placeholder - implement when modules have actual types
        Assert.True(true);
    }
}
