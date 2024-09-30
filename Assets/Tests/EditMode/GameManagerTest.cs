using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GameManagerTest
{
    [TestCase(1)]
    public void Test(int x)
    {
        GameObject g = new GameObject("GameManager");
        GameManager gm = g.AddComponent<GameManager>();
        int actual = gm.AddOne(x);
        int expected = x + 1;
        Assert.AreEqual(expected, actual);
    }
}
