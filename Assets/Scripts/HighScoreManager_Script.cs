using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreManager_Script : MonoBehaviour
{
    public enum RuleSet
    {
        Custom,
        Classic,
        Casual,
        Wild
    }

    private GameObject highScoreScreen;
    private Text customHighScoreText;
    private Text classicHighScoreText;
    private Text casualHighScoreText;
    private Text wildHighScoreText;

    // Dictionary to store the high scores for each rule set
    private Dictionary<RuleSet, Dictionary<string, int>> highScores;

    void Start()
    {
        highScoreScreen = GameObject.Find("HighScoreScreen");
        customHighScoreText = GameObject.Find("CustomText").GetComponent<Text>();
        classicHighScoreText = GameObject.Find("ClassicText").GetComponent<Text>();
        casualHighScoreText = GameObject.Find("CasualText").GetComponent<Text>();
        wildHighScoreText = GameObject.Find("WildText").GetComponent<Text>();

        highScores = new Dictionary<RuleSet, Dictionary<string, int>>();
        InitializeHighScores();
        LoadHighScores();
        UpdateHighScoreText();

        //HideScreen();
    }

    // Initialize the high scores dictionary with an empty dictionary for each rule set
    private void InitializeHighScores()
    {
        foreach (RuleSet ruleSet in System.Enum.GetValues(typeof(RuleSet)))
        {
            highScores.Add(ruleSet, new Dictionary<string, int>());
        }
    }

    // Load the high scores for each rule set from PlayerPrefs and add them to the dictionary
    private void LoadHighScores()
    {
        foreach (RuleSet ruleSet in System.Enum.GetValues(typeof(RuleSet)))
        {
            Dictionary<string, int> scoresForRuleSet = highScores[ruleSet];
            for (int i = 1; i <= 10; i++)
            {
                string key = ruleSet.ToString() + "HighScore" + i.ToString();
                int score = PlayerPrefs.GetInt(key, 0);
                scoresForRuleSet.Add(key, score);
            }
        }
    }

    // Update the high score text for each rule set in the UI
    private void UpdateHighScoreText()
    {
        foreach (RuleSet ruleSet in System.Enum.GetValues(typeof(RuleSet)))
        {
            Dictionary<string, int> scoresForRuleSet = highScores[ruleSet];
            string text = "";
            for (int i = 1; i <= 10; i++)
            {
                string key = ruleSet.ToString() + "HighScore" + i.ToString();
                int score = scoresForRuleSet[key];
                // Get the player name from the high score key
                string playerName = key.Substring(key.IndexOf("HighScore") + 9);
                text += playerName + ": " + score.ToString() + "\n";
            }
            switch (ruleSet)
            {
                case RuleSet.Custom:
                    customHighScoreText.text = text;
                    break;
                case RuleSet.Classic:
                    classicHighScoreText.text = text;
                    break;
                case RuleSet.Casual:
                    casualHighScoreText.text = text;
                    break;
                case RuleSet.Wild:
                    wildHighScoreText.text = text;
                    break;
            }
        }
    }


    // Save a new high score for the given name, score, and rule set
    public void SaveHighScore(string name, int score, string ruleSetName)
    {
        RuleSet ruleSet = (RuleSet)System.Enum.Parse( typeof(RuleSet), ruleSetName );
        Dictionary<string, int> scoresForRuleSet = highScores[ruleSet];
        string key = ruleSet.ToString() + "HighScore" + name;
        if (scoresForRuleSet.ContainsKey(key))
        {
            if (score > scoresForRuleSet[key])
            {
                scoresForRuleSet[key] = score;
                PlayerPrefs.SetInt(key, score);
                PlayerPrefs.Save();
                UpdateHighScoreText();
            }
        }
        else
        {
            PlayerPrefs.SetInt(key,score);
            Debug.Log("new key created: " + key);
        }
        UpdateHighScoreText();

    }

    public void ShowScreen(){
        highScoreScreen.SetActive(true);
    }

    public void HideScreen(){
        highScoreScreen.SetActive(false);
    }
}
