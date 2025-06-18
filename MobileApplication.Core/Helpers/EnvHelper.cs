using System.Reflection;
using DotNetEnv;

namespace MobileApplication.Core.Helpers
{
    public sealed class EnvHelper
    {
        private static readonly Lazy<EnvHelper> _instance = new(() => new EnvHelper());
        public static EnvHelper Instance => _instance.Value;

        private EnvHelper()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var resourceNames = assembly.GetManifestResourceNames();
            foreach (var name in resourceNames)
                Console.WriteLine($"Resource found: {name}");

            string resourceName = "MobileApplication.Core..env";

            using Stream? stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                Console.WriteLine($"Embedded resource not found: {resourceName}");
                return;
            }

            Env.Load(stream);

            Console.WriteLine(".env loaded from embedded resource.");
        }

        public string GetEnvironmentVariable(string key, string? defaultValue = null)
        {
            string? value = Environment.GetEnvironmentVariable(key);
            if (string.IsNullOrWhiteSpace(value))
            {
                Console.WriteLine($"Key not found: {key}");
                return defaultValue!;
            }
            return value;
        }
    }
}