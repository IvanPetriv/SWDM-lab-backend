namespace Api.Utils;

public static class EnvVarUtils {
    /// <summary>
    /// Retrieves the value of the specified environment variable.
    /// </summary>
    /// <param name="key">The name of the environment variable to retrieve. This value is case-sensitive on some operating systems.</param>
    /// <returns>The value of the specified environment variable.</returns>
    /// <exception cref="NullReferenceException">Thrown if the specified environment variable does not exist or its value is <see langword="null"/>.</exception>
    public static string TryGetEnvVar(string key) {
        string? variable = Environment.GetEnvironmentVariable(key);

        if (variable is null) {
            throw new NullReferenceException($"Environment does not have '{key}' variable");
        }

        return variable;
    }
}
