using NUnit.Framework;
using System;

namespace SocialMediaServices.Tests.Integration
{
    internal static class TestParamUtility
    {
        public static string GetParamOrDefault(string parameterKey)
        {
            return TestContext.Parameters.Exists(parameterKey) ?
                TestContext.Parameters[parameterKey] :
                Environment.GetEnvironmentVariable(parameterKey);
        }
    }
}