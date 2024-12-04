using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using TMPro;

public class NPCSelectPlayTest
{
    private GameManager gm;
    private SelectionManager sm;
    private NPCSelectScroller scroller;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Load StartScreenScene in order to put the SettingsManager into DDOL
        SceneManager.LoadScene("StartScreenScene");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("StartScreenScene").isLoaded);

        // Move debugmanager and copyright back to startscreenscene so that 
        SceneManager.MoveGameObjectToScene(GameObject.Find("DebugManager"), SceneManager.GetSceneByName("StartScreenScene"));
        SceneManager.MoveGameObjectToScene(GameObject.Find("Copyright"), SceneManager.GetSceneByName("StartScreenScene"));

        // Unload the StartScreenScene
        SceneManager.UnloadSceneAsync("StartScreenScene");

        // Load the "Loading" scene in order to get access to the toolbox in DDOL
        SceneManager.LoadScene("Loading");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("Loading").isLoaded);

        // Put toolbox as parent of SettingsManager
        GameObject.Find("SettingsManager").transform.SetParent(GameObject.Find("Toolbox").transform);

        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        gm.StartGame(null, Resources.LoadAll<StoryObject>("Stories")[0]);

        SceneManager.LoadScene("NPCSelectScene");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded);

        sm = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();
        scroller = GameObject.Find("Scroller").GetComponent<NPCSelectScroller>();
    }

    [TearDown]
    public void TearDown()
    {
        SceneManager.MoveGameObjectToScene(GameObject.Find("Toolbox"), SceneManager.GetSceneByName("NPCSelectScene"));
        SceneController.sc.UnloadAdditiveScenes();
    }

    [UnityTest]
    public IEnumerator StartNPCSelectTest()
    {
        // Check if the scroller children are what they should be
        Assert.AreEqual("Scrollable", scroller.transform.GetChild(0).gameObject.name);
        Assert.AreEqual("NavLeft", scroller.transform.GetChild(1).gameObject.name);
        Assert.AreEqual("NavRight", scroller.transform.GetChild(2).gameObject.name);

        yield return null;
    }

    [UnityTest]
    public IEnumerator FadeInTest()
    {
        var testObject = new GameObject();
        var cg = testObject.AddComponent<CanvasGroup>();
        cg.alpha = 0;
        float duration = 0.2f;

        // Make sure the starting values are correct
        Assert.AreEqual(0, cg.alpha);

        // Start the fade
        sm.FadeIn_Test(cg, duration);

        // Make sure something is happening
        yield return new WaitForSeconds(duration / 2);
        Assert.Greater(cg.alpha, 0);
        Assert.Less(cg.alpha, 1);

        // Check if the value is correct at the end
        yield return new WaitForSeconds(duration / 2);
        Assert.AreEqual(1, cg.alpha);
    }

    [UnityTest] 
    public IEnumerator FadeOutTest()
    {
        var testObject = new GameObject();
        var cg = testObject.AddComponent<CanvasGroup>();
        cg.alpha = 1;
        float duration = 0.2f;

        // Make sure the starting values are correct
        Assert.AreEqual(1, cg.alpha);

        // Start the fade
        sm.FadeOut_Test(cg, duration);

        // Make sure something is happening
        yield return new WaitForSeconds(duration / 2);
        Assert.Greater(cg.alpha, 0);
        Assert.Less(cg.alpha, 1);

        // Check if the value is correct at the end
        yield return new WaitForSeconds(duration / 2);
        Assert.AreEqual(0, cg.alpha);
    }

    [UnityTest]
    public IEnumerator SelectedCharacterNameTest()
    {
        for (int i = 0; i < scroller.Test_Children.Length; i++)
        {
            scroller.Test_InstantNavigate(i);
            scroller.Test_SelectedChild = i;

            yield return null;

            var character = scroller.Test_Children[i].GetComponentInChildren<SelectOption>().character;
            TMP_Text text = sm.Test_GetSelectionButtonRef().GetComponentInChildren<TMP_Text>();
            Assert.IsTrue(text.text.Contains(character.characterName),
                $"Expected the string to contain {character.characterName}, but the string was {text.text}");
        }
    }
}
