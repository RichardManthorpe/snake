using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class LevelGrid {

    private Vector2Int foodGridPosition;
    private GameObject foodGameObject;
    private Vector2Int wallGridPosition;
    private Vector2Int lastWallGridPosition;
    private GameObject wallGameObject;
    private List<WallPosition> wallPositionList;
    private Vector2Int gridPosition;
    private Vector2Int newWallGridPosition;
    private Vector2Int baseBrick;

    private int foodSelection;
    private int width;
    private int height;
    private Snake snake;
    private int moveCountSinceFood;
    private int moveCountSinceWall;
    private int foodCount=0;
    private int maxMovesForFood =40;
    private int movesForWall =40;
    private int brickCount;
    private int pickABrick;
    private int pickDirection;

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
        Timeout,
        HitWall,
        WallTimeout
    }

    private FoodType foodType;
    private TryResult tryResult;

    public LevelGrid(int width, int height){
            this.width=width;
            this.height=height;
    }

    public void Setup(Snake snake){
        gridPosition=new Vector2Int(Random.Range(0,this.width),Random.Range(0,this.height));
        wallGridPosition=gridPosition;
        this.snake = snake;
        SpawnFood();
        wallPositionList = new List<WallPosition>();
    }

    public void SpawnFood(){
        do{ 
            foodGridPosition = new Vector2Int(Random.Range(0,width), Random.Range(0, height));
        } while ((snake.getFullSnakeGridPositionList().IndexOf(foodGridPosition) != -1) && (GetFullWallGridPositionList().IndexOf(foodGridPosition) != -1));
        
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
        }else if (foodSelection<=3){
            foodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snailSprite;
            SetFoodType(FoodType.Snail);
        }else if (foodSelection<=4){
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

    public void SpawnWall(){
        
        Debug.Log("width: "+width);
        Debug.Log("height: "+height);
        do { 
            wallGridPosition = GetNextWallGridPosition();
        } while (GetFullWallGridPositionList().IndexOf(wallGridPosition) !=-1);
        
        wallGameObject = new GameObject("Wall", typeof(SpriteRenderer));
        wallGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.wallSprite;
        wallGameObject.transform.position = new Vector3(wallGridPosition.x, wallGridPosition.y);
        WallPosition wallPosition = new WallPosition(wallGridPosition);
        wallPositionList.Insert(0, wallPosition);
        
        return;
    }

    public bool TrySnakeHitWall(Vector2Int snakeGridPosition){
        foreach(WallPosition wallPosition in wallPositionList) {
            Vector2Int wallGridPosition = wallPosition.GetGridPosition();
            if (snakeGridPosition == wallGridPosition){
                SetTryResult(TryResult.HitWall);
                return true;
            }
        }

        if (moveCountSinceWall>movesForWall){
            //time for next wall object
            SetTryResult(TryResult.WallTimeout);
            moveCountSinceWall=0;
            return false;
         }else {
            moveCountSinceWall++;
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

    private class WallPosition {
        
        private Vector2Int gridPosition;

        public WallPosition(Vector2Int gridPosition) {
            this.gridPosition = gridPosition;
        }

        public Vector2Int GetGridPosition(){
            return gridPosition;
        }
    }

    public List<Vector2Int> GetFullWallGridPositionList() {
        List<Vector2Int> gridPositionList = new List<Vector2Int>() { gridPosition };
        foreach (WallPosition wallPosition in wallPositionList) {
            gridPositionList.Add(wallPosition.GetGridPosition());
        }
        return gridPositionList;
    }

    private Vector2Int GetNextWallGridPosition() {
        
//        newWallGridPosition=new Vector2Int(0,0);
        List<Vector2Int> gridPositionList=GetFullWallGridPositionList();
        brickCount=gridPositionList.Count;
        pickABrick = Random.Range(0,brickCount);
        pickDirection = Random.Range(0,3);
        baseBrick=gridPositionList[pickABrick];
        newWallGridPosition=baseBrick;
        Debug.Log("Base Brick: ("+baseBrick.x+","+baseBrick.y+")");
        if (pickDirection ==0){
            if (newWallGridPosition.x <width){
                newWallGridPosition.x = baseBrick.x+1;
            }else {
                newWallGridPosition.x = baseBrick.x-1;
            }
        } else if (pickDirection ==1){
            if (newWallGridPosition.x >0){
                newWallGridPosition.x = baseBrick.x-1;
            }else{
                newWallGridPosition.x = baseBrick.x+1;
            }
        } else if (pickDirection ==2){
            if (newWallGridPosition.y <height){
                newWallGridPosition.y = baseBrick.y+1;
            }else{
                newWallGridPosition.y = baseBrick.y-1;
            }
        } else if (pickDirection ==3){
            if (newWallGridPosition.y >0){
                newWallGridPosition.y = baseBrick.y-1;
            }else{
                newWallGridPosition.y = baseBrick.y+1;
            }
        } 
        Debug.Log("New Brick: ("+newWallGridPosition.x+","+newWallGridPosition.y+")");
        return newWallGridPosition;      
    }

}
