using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class DialogueManagerPlayTest
{
    static Tuple<Question, string> [] questionsAnswers = new Tuple<Question, string>[]
    {
        new Tuple<Question, string>(Question.Name, "What is your name?"),
        new Tuple<Question, string>(Question.Age, "How old are you?"),
        new Tuple<Question, string>(Question.Wellbeing, "How are you doing?"),
        new Tuple<Question, string>(Question.Political, "What are your political thoughts?"),
        new Tuple<Question, string>(Question.Personality, "Can you describe what your personality is like?"),
        new Tuple<Question, string>(Question.Hobby, "What are some of your hobbies?"),
        new Tuple<Question, string>(Question.CulturalBackground, "What is your cultural background?"),
        new Tuple<Question, string>(Question.Education, "What is your education level?"),
        new Tuple<Question, string>(Question.CoreValues, "What core values are the most important to you?"),
        new Tuple<Question, string>(Question.ImportantPeople, "Who are the most important people in your life?"),
        new Tuple<Question, string>(Question.PositiveTrait, "What do you think is your best trait?"),
        new Tuple<Question, string>(Question.NegativeTrait, "What is a bad trait you may have?"),
        new Tuple<Question, string>(Question.OddTrait, "Do you have any odd traits?")
    };

    /// <summary>
    /// Checks if the correct prompt texts gets retrieved, based on the question type.
    /// </summary>
    [UnityTest]
    public IEnumerator GetPromptTextTest([ValueSource(nameof(questionsAnswers))] Tuple<Question, string> qATuple)
    {
        // Load scene
        SceneManager.LoadScene("DialogueScene");
        yield return new WaitForSeconds(3); // Wait for it to load
        
        // Get GameManager object
        var g = GameObject.Find("DialogueManager");
        var dm = g.GetComponent<DialogueManager>();

        Question questionType = qATuple.Item1;
        string answer = qATuple.Item2;
        
        // Set up expected
        string expected = dm.GetPromptText(questionType);
    
        // Check if the output is correct
        Assert.AreEqual(expected, answer);

        yield return null;
    }
}