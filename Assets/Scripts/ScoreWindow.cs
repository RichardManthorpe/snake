using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreWindow : MonoBehaviour
{
    private Text scoreText;
    private static ScoreWindow instance;

    private void Awake() {
        instance= this;
        scoreText = transform.Find("scoreText").GetComponent<Text>();

        Score.OnHighscoreChanged += Score_OnHighscoreChanged;
        UpdateHighScore();
    }
    
    private void Score_OnHighscoreChanged(object sender, System.EventArgs e){
        UpdateHighScore();
    }

    private void Update() {
        scoreText.text = Score.GetScore().ToString();
    }

    private void UpdateHighScore(){
        int highscore = Score.GetHighscore();
        transform.Find("highScoreText").GetComponent<Text>().text = "HIGHSCORE\n" + highscore.ToString();
    }

    public static void HideStatic() {
        instance.gameObject.SetActive(false);
    }

}
