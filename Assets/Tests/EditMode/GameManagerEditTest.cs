using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class GameManagerEditTest
{
    /// <summary>
    /// Checks if the correct prompt texts gets retrieved, based on the question type.
    /// </summary>
    [TestCase(Question.Name, "What is your name?")]
    [TestCase(Question.Age, "How old are you?")]
    [TestCase(Question.Wellbeing, "How are you doing?")]
    [TestCase(Question.Political, "What are your political thoughts?")]
    [TestCase(Question.Personality, "Can you describe what your personality is like?")]
    [TestCase(Question.Hobby, "What are some of your hobbies?")]
    [TestCase(Question.CulturalBackground, "What is your cultural background?")]
    [TestCase(Question.Education, "What is your education level?")]
    [TestCase(Question.CoreValues, "What core values are the most important to you?")]
    [TestCase(Question.ImportantPeople, "Who are the most important people in your life?")]
    [TestCase(Question.PositiveTrait, "What do you think is your best trait?")]
    [TestCase(Question.NegativeTrait, "What is a bad trait you may have?")]
    [TestCase(Question.OddTrait, "Do you have any odd traits?")]
    public void GetPromptTextTest(Question questionType, string answer)
    {
        // Get GameManager object
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();
        
        // Set up expected
        string expected = gm.GetPromptText(questionType);
        
        // Check if the output is correct
        Assert.AreEqual(expected, answer);
    }
}