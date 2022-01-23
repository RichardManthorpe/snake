using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class LevelGrid {

    private Vector2Int foodGridPosition;
    private GameObject foodGameObject;
    private int foodSelection;
    private int width;
    private int height;
    private Snake snake;
    private int moveCountSinceFood;
    private int foodCount=0;
    private int maxMovesForFood =40;

    public enum FoodType {
        None,
        Food,
        Rotten,
        Star,
        Snail,
        Lightning
    }

    public enum TryResult {
        Hit,
        Miss,
        Timeout
    }

    private FoodType foodType;
    private TryResult tryResult;

    public LevelGrid(int width, int height){
            this.width=width;
            this.height=height;
    }

    public void Setup(Snake snake){
        this.snake = snake;
        SpawnFood();
    }

    public void SpawnFood(){
        do{ 
            foodGridPosition = new Vector2Int(Random.Range(0,width), Random.Range(0, height));
        } while (snake.getFullSnakeGridPositionList().IndexOf(foodGridPosition) != -1);
        
        //only allow special foods after first few food drops
        if (foodCount>3){
            foodSelection=Random.Range(0, 30);
        }else{
            foodSelection=30;
        }
        //Debug.Log("FoodSelection: "+foodSelection);
        
        foodGameObject = new GameObject("Food", typeof(SpriteRenderer));

        if (foodSelection<=0){
            foodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.starSprite;
            SetFoodType(FoodType.Star);   
        }else if (foodSelection<=2){
            foodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.rottenSprite;
            SetFoodType(FoodType.Rotten);
        }else if (foodSelection<=4){
            foodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snailSprite;
            SetFoodType(FoodType.Snail);
        }else if (foodSelection<=12){
            foodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.lightningSprite;
            SetFoodType(FoodType.Lightning);
        }else{
            foodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.foodSprite;
            SetFoodType(FoodType.Food);
        }
        foodGameObject.transform.position = new Vector3(foodGridPosition.x, foodGridPosition.y);
        
        return;
    }

    private void SetFoodType (FoodType food){
        foodType=food;
        return;
    }

    public FoodType GetFoodType (){
        return foodType;
    }

    private void SetTryResult (TryResult result){
        tryResult=result;
        return;
    }

    public TryResult GetTryResult (){
        return tryResult;
    }

    public bool TrySnakeEatFood(Vector2Int snakeGridPosition){
        if (snakeGridPosition == foodGridPosition) {
            //food eaten
            Object.Destroy(foodGameObject);
            moveCountSinceFood=0;
            foodCount++;
            SetTryResult(TryResult.Hit);
            return true;
        }else if (moveCountSinceFood>maxMovesForFood){
            //food timed out
            Object.Destroy(foodGameObject);
            SetTryResult(TryResult.Timeout);
            moveCountSinceFood=0;
            return false;
        }else{
            moveCountSinceFood++;
            SetTryResult(TryResult.Miss);
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
