using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class LevelGrid {

    private Vector2Int foodGridPosition;
    private GameObject foodGameObject;
    private int width;
    private int height;
    private Snake snake;

    public LevelGrid(int width, int height){
            this.width=width;
            this.height=height;

            //FunctionPeriodic.Create(SpawnFood, 1f);
            //SpawnFood();
    }

    public void Setup(Snake snake){
        this.snake = snake;
        SpawnFood();
    }

    private void SpawnFood(){
        do{ 
            foodGridPosition = new Vector2Int(Random.Range(0,width), Random.Range(0, height));
        //} while (snake.GetGridPosition()==foodGridPosition);
        } while (snake.getFullSnakeGridPositionList().IndexOf(foodGridPosition) != -1);
        
        foodGameObject = new GameObject("Food", typeof(SpriteRenderer));
        foodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.foodSprite;
        foodGameObject.transform.position = new Vector3(foodGridPosition.x, foodGridPosition.y);
    }

    public bool TrySnakeEatFood(Vector2Int snakeGridPosition){
        if (snakeGridPosition == foodGridPosition) {
            Object.Destroy(foodGameObject);
            SpawnFood();
            GameHandler.AddScore();
            //CMDebug.TextPopupMouse("Snake Ate Food");
            return true;
        }
        else {
            return false;
        }
    }

    public Vector2Int ValidateGridPosition(Vector2Int gridPosition){
            if (gridPosition.x<0){
                gridPosition.x=width-1;
            }
            if (gridPosition.y<0){
                gridPosition.y=height-1;
            }
            if (gridPosition.x>width-1){
                gridPosition.x=0;
            }
            if (gridPosition.y>height-1){
                gridPosition.y=0;
            }   
            return gridPosition;         
    }


}
