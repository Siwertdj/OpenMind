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
        SceneManager.LoadScene("Loading");
        
        yield return new WaitForSeconds(3);
        
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();
        
        var actual = gm.HasQuestionsLeft();

        yield return new WaitForSeconds(1);
        
        Assert.AreEqual(true, actual);
    }
}
