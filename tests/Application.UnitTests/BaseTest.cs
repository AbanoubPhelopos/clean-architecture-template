using System.Reflection;
using Application;
using Domain;
using SharedKernel;

namespace Application.UnitTests;

public abstract class BaseTest
{
    protected static Assembly ApplicationAssembly => typeof(Application.DependencyInjection).Assembly;
    protected static Assembly DomainAssembly => typeof(Domain.Users.User).Assembly;
    protected static Assembly SharedKernelAssembly => typeof(Result).Assembly;
}