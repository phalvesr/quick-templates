namespace MyTemplate.CrossCutting.Helpers;

public static class EnvironmentHelpers
{
    public static bool IsLocalEnvironment => 
        string.Equals(
            Environment.GetEnvironmentVariable("EXECUTING_ENVIRONMENT"), 
            "local", 
            StringComparison.InvariantCultureIgnoreCase
        );
    
    public static bool IsTestingEnvironment => 
        string.Equals(
            Environment.GetEnvironmentVariable("EXECUTING_ENVIRONMENT"), 
            "test", 
            StringComparison.InvariantCultureIgnoreCase
        );

    public static bool IsObservableEnvironment => (
        !IsLocalEnvironment && !IsTestingEnvironment
    );

    public static bool IsRealEnvironment => (
        !IsLocalEnvironment && !IsTestingEnvironment
    );
}
