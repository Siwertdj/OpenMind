using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class NPCSelectPlayTest
{
    private GameManager gm;
    private SelectionManager sm;
    private NPCSelectScroller scroller;

    private GameObject[] scrollChildren;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        SceneManager.LoadScene("Loading");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("Loading").isLoaded);

        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        gm.StartGame(null, Resources.LoadAll<StoryObject>("Stories")[0]);

        SceneManager.LoadScene("NPCSelectScene");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded);

        sm = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();
        scroller = GameObject.Find("Scroller").GetComponent<NPCSelectScroller>();

        List<GameObject> children = new();
        foreach (Transform child in scroller.transform.GetChild(0))
            children.Add(child.gameObject);

        scrollChildren = children.ToArray();

    }

    [UnityTearDown]
    public void TearDown()
    {

    }

    [UnityTest]
    public IEnumerator StartNPCSelectTest()
    {
        Assert.AreEqual("Scrollable", scroller.transform.GetChild(0).gameObject.name);
        Assert.AreEqual("NavLeft", scroller.transform.GetChild(1).gameObject.name);
        Assert.AreEqual("NavRight", scroller.transform.GetChild(2).gameObject.name);

        yield return null;
    }

    [UnityTest]
    public IEnumerator GetTargetPosTest()
    {
        yield return null;
    }
}
