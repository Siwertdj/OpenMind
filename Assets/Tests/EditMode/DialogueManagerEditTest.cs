using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class DialogueManagerEditTest
{
    [Test]
    [TestCase(Question.Name, "What's your name?")]
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
    public void GetPromptTextTest(Question question, string expected)
    {
        DialogueManager dm = new DialogueManager();

        string actual = dm.GetPromptText(question);
        
        Assert.AreEqual(expected, actual);
    }
}