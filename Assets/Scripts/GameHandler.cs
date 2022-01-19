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
    
        //int number=0;
        //FunctionPeriodic.Create(() => {
        //    CMDebug.TextPopupMouse("Ding! " + number);
        //    number ++;
        //}, .3f);
    
        //GameObject snakeHeadGameObject = new GameObject();
        //SpriteRenderer snakeSpriteRenderer = snakeHeadGameObject.AddComponent<SpriteRenderer>();
        //snakeSpriteRenderer.sprite = GameAssets.i.snakeHeadSprite;
        
        levelGrid = new LevelGrid(20,20);
        
        snake.Setup(levelGrid);
        levelGrid.Setup(snake);

        /*CMDebug.ButtonUI(Vector2.zero, "Reload Scene", () => {
            Loader.Load(Loader.Scene.GameScene);
        } );*/
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

}
