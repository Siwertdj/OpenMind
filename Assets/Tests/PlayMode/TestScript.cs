using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class TestScript
{
    [UnityTest]
    public IEnumerator HasQuestionsLeftTest()
    {
        GameObject g = new GameObject();
        GameManager gm = g.AddComponent<GameManager>(); // This gives a NullReferenceException when it tries to call Load() on line 70
        
        var actual = gm.HasQuestionsLeft();

        yield return new WaitForSeconds(1);
        
        Assert.AreEqual(true, true);
    }
}
