using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Score 
{
    public static event EventHandler OnHighscoreChanged;
    private static int score;

    public static void InitiatizeStatic(){
        score=0;
        OnHighscoreChanged = null;
    }

    public static int GetScore(){
        return score;
    }

    public static void AddScore(LevelGrid.FoodType foodType){
        if (foodType==LevelGrid.FoodType.Food){
            score +=100;
        }else if (foodType==LevelGrid.FoodType.Star){
            score +=500;
        }else if (foodType==LevelGrid.FoodType.Rotten){
            score -=500;
        }
    }

    public static int GetHighscore() {
            return PlayerPrefs.GetInt("highscore",0);
    }

    public static bool TrySetNewHighscore(){
        return TrySetNewHighscore(score);
    }

    public static bool TrySetNewHighscore(int score){
        int highscore=GetHighscore();
        if (score >highscore){
                PlayerPrefs.SetInt("highscore", score);
                PlayerPrefs.Save();
                if (OnHighscoreChanged != null) OnHighscoreChanged(null, EventArgs.Empty);
                return true;
        }else{
            return false;
        }
    }
}
