using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class GameHandler : MonoBehaviour
{ 
    private static GameHandler instance;

    private static int score;

    [SerializeField] private Snake snake;
    
    private LevelGrid levelGrid;
   
   
    private void Awake() {
        instance = this;
        InitiatizeStatic();
    }

    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log("GameHandler Start");
        Time.timeScale = 1f;
        levelGrid = new LevelGrid(20,20);
        
        snake.Setup(levelGrid);
        levelGrid.Setup(snake);

        /*CMDebug.ButtonUI(Vector2.zero, "Reload Scene", () => {
            Loader.Load(Loader.Scene.GameScene);
        } );*/
    }

    private void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)){
            if (IsGamePaused()){
                GameHandler.ResumeGame();
            }else{
                GameHandler.PauseGame();
            }
        }
    }

    public static void InitiatizeStatic(){
        score=0;
    }

    public static int GetScore(){
        return score;
    }

    public static void AddScore(){
        score +=100;
    }

    public static void SnakeDied(){
        GameOverWindow.ShowStatic();
    }

    public static void ResumeGame(){
        PauseWindow.HideStatic();
        Time.timeScale = 1f;
    }

    public static void PauseGame(){
        PauseWindow.ShowStatic();
        Time.timeScale = 0f;
    }

     public static bool IsGamePaused(){
        return Time.timeScale == 0f;
    }


}
