using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MyTemplate.Entrypoint")]
[assembly:InternalsVisibleTo("MyTemplate.UnitTests")]
// Make internals visible to mock libraries like Moq, NSubstitute, etc.
[assembly:InternalsVisibleTo("DynamicProxyGenAssembly2")]