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
    private int moveCountSinceFood;
    private int maxMovesForApple =30;

    public LevelGrid(int width, int height){
            this.width=width;
            this.height=height;
    }

    public void Setup(Snake snake){
        this.snake = snake;

        SpawnFood();
    }

    private void SpawnFood(){
        do{ 
            foodGridPosition = new Vector2Int(Random.Range(0,width), Random.Range(0, height));
        } while (snake.getFullSnakeGridPositionList().IndexOf(foodGridPosition) != -1);
        
        foodGameObject = new GameObject("Food", typeof(SpriteRenderer));
        foodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.foodSprite;
        foodGameObject.transform.position = new Vector3(foodGridPosition.x, foodGridPosition.y);
    }

    public bool TrySnakeEatFood(Vector2Int snakeGridPosition){
        if (snakeGridPosition == foodGridPosition) {
            Object.Destroy(foodGameObject);
            SpawnFood();
            Score.AddScore();
            moveCountSinceFood=0;
            return true;
        }else if (moveCountSinceFood>maxMovesForApple){
            Object.Destroy(foodGameObject);
            SpawnFood();
            moveCountSinceFood=0;
            return false;
        }else {
            moveCountSinceFood++;
            //Debug.Log("Move Count: " + moveCountSinceFood);
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
