using NUnit.Framework;
using UnityEngine;

namespace Assets.Tests
{
    [SetUpFixture]
    internal class TestsSetup
    {

        [OneTimeSetUp]
        public void BeforeAny()
        {
            Debug.Log(new string('-', 20));
            Debug.Log("Test Start");
            Debug.Log(new string('-', 20));
        }

        [OneTimeTearDown]
        public void AfterAny()
        {
            Debug.Log(new string('-', 20));
            Debug.Log("Test End");
            Debug.Log(new string('-', 20));
        }
    }
}
