using System.Reflection;
using NUnit.Framework;

[SetUpFixture]
public class TestSetupPlayMode
{
    [OneTimeSetUp]
    public void DisablePopups()
    {
        FieldInfo fieldInfo = typeof(DebugManager).GetField("FullyDisablePopups",
            BindingFlags.NonPublic | BindingFlags.Static);
        fieldInfo.SetValue(null, true);
    }
}
