using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GameManagerTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void Test_Test()
    {
        GameManager gm = new GameManager();
        int x = 1;
        int res = gm.TestFunction(1);
        Assert.AreEqual(x + 1, res);
    }
}
