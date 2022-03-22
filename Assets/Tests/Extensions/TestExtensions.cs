using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Assets.Tests.Extensions
{
    internal class TestExtensions
    {
        public static TestCaseAttribute GetCurrentTestCase()
        {
            var test = TestContext.CurrentContext.Test;

            var className = test.FullName.Remove(test.FullName.LastIndexOf("."));

            var classType = AppDomain.CurrentDomain.GetAssemblies()
                .Select(x => x.GetType(className))
                .First(x => x != null);

            var method = classType.GetMethod(test.MethodName);

            var testCase = method
                .GetCustomAttributes<TestCaseAttribute>()
                .First(x => x.TestName == test.Name);

            return testCase;
        }
    }
}
