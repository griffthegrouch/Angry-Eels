using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class HighScoreManager_Script : MonoBehaviour
{
    public enum RuleSet
    {
        Custom,
        Classic,
        Casual,
        Wild
    }

    private GameHandler_Script gameHandlerScript;

    private GameObject highScoreScreen;
    private Text customHighScoreText;
    private Text classicHighScoreText;
    private Text casualHighScoreText;
    private Text wildHighScoreText;

    // Dictionary to store the high scores for each rule set
    private Dictionary<RuleSet, List<KeyValuePair<string, int>>> highScores;

    void Start()
    {
        gameHandlerScript = GameObject.Find("GameHandler").GetComponent<GameHandler_Script>();
        transform.localPosition = Vector2.zero;

        highScoreScreen = GameObject.Find("HighScoreScreen");
        customHighScoreText = GameObject.Find("CustomText").GetComponent<Text>();
        classicHighScoreText = GameObject.Find("ClassicText").GetComponent<Text>();
        casualHighScoreText = GameObject.Find("CasualText").GetComponent<Text>();
        wildHighScoreText = GameObject.Find("WildText").GetComponent<Text>();

        highScores = new Dictionary<RuleSet, List<KeyValuePair<string, int>>>();
        InitializeHighScores();
        LoadHighScores();
        UpdateHighScoreText();

        CloseMenu();
    }

    public void OpenMenu()
    {
        highScoreScreen.SetActive(true);
    }

    public void CloseMenu()
    {
        highScoreScreen.SetActive(false);
    }

    private void InitializeHighScores()
    {
        foreach (RuleSet ruleSet in System.Enum.GetValues(typeof(RuleSet)))
        {
            highScores.Add(ruleSet, new List<KeyValuePair<string, int>>());
        }
    }

    private void LoadHighScores()
    {
        foreach (RuleSet ruleSet in System.Enum.GetValues(typeof(RuleSet)))
        {
            for (int i = 0; i < 10; i++)
            {
                string key = ruleSet.ToString() + "HighScore" + i.ToString();
                string playerName = PlayerPrefs.GetString(key + "Name", "Player " + (i + 1));
                int score = PlayerPrefs.GetInt(key + "Score", 0);
                highScores[ruleSet].Add(new KeyValuePair<string, int>(playerName, score));
            }
        }
    }

    private void UpdateHighScoreText()
    {
        foreach (RuleSet ruleSet in System.Enum.GetValues(typeof(RuleSet)))
        {
            string text = "";
            List<KeyValuePair<string, int>> scoresForRuleSet = highScores[ruleSet];
            for (int i = 0; i < scoresForRuleSet.Count; i++)
            {
                string playerName = scoresForRuleSet[i].Key;
                int score = scoresForRuleSet[i].Value;
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

    public void SaveHighScore(string name, int score, RuleSet ruleSet)
    {
        List<KeyValuePair<string, int>> scoresForRuleSet = highScores[ruleSet];
        scoresForRuleSet.Add(new KeyValuePair<string, int>(name, score));
        scoresForRuleSet = scoresForRuleSet.OrderByDescending(entry => entry.Value).Take(10).ToList();
        highScores[ruleSet] = scoresForRuleSet;
        for (int i = 0; i < scoresForRuleSet.Count; i++)
        {
            string key = ruleSet.ToString() + "HighScore" + i.ToString();
            PlayerPrefs.SetString(key + "Name", scoresForRuleSet[i].Key);
            PlayerPrefs.SetInt(key + "Score", scoresForRuleSet[i].Value);
        }
        PlayerPrefs.Save();
        UpdateHighScoreText();
    }

}
