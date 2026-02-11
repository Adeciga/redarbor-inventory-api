using NetArchTest.Rules;
using Xunit;

namespace Inventory.Architecture.Tests;

public class ArchitectureTests
{
    private const string DomainNamespace = "Inventory.Domain";
    private const string ApplicationNamespace = "Inventory.Application";
    private const string InfrastructureNamespace = "Inventory.Infrastructure";
    private const string ApiNamespace = "Inventory.Api";
    private const string IdentityNamespace = "Inventory.Identity";

    [Fact]
    public void Domain_should_not_depend_on_other_projects()
    {
        var result = Types.InAssembly(typeof(Inventory.Domain.AssemblyReference).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(ApplicationNamespace, InfrastructureNamespace, ApiNamespace, IdentityNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, BuildFailureMessage(result));
    }

    [Fact]
    public void Application_should_not_depend_on_Infrastructure_or_Api_or_Identity()
    {
        var result = Types.InAssembly(typeof(Inventory.Application.AssemblyReference).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(InfrastructureNamespace, ApiNamespace, IdentityNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, BuildFailureMessage(result));
    }

    [Fact]
    public void Infrastructure_should_not_depend_on_Api()
    {
        var result = Types.InAssembly(typeof(Inventory.Infrastructure.AssemblyReference).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(ApiNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, BuildFailureMessage(result));
    }

    private static string BuildFailureMessage(TestResult result)
    {
        if (result is null)
            return "TestResult was null.";

        // NetArchTest can sometimes return null here; guard it.
        var failing = result.FailingTypeNames ?? System.Array.Empty<string>();

        if (failing.Count() == 0)
            return "Architecture test failed but NetArchTest did not report failing types.";

        return string.Join("\n", failing);
    }
}
