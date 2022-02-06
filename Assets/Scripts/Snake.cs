using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class Snake : MonoBehaviour
{
    private enum Direction {
        Left,
        Right,
        Up,
        Down
    }

    private enum State {
        Alive,
        Dead
    }

    private enum SpeedAction {
        Min,
        Max,
        Increase,
        Decrease,
        ResetToPrevious
    }

    private State state;
    private Direction gridMoveDirection;
    private Vector2Int gridPosition;
    private float gridMoveTimer;
    private float gridMoveTimerMax;
    private float gridMoveTimerNew;
    private float maxSpeed;
    private float currentSpeed;
    private int speedIncreasePercent;
    private int speedDecreasePercent;
    private LevelGrid levelGrid;
    private int snakeBodySize;
    private List<SnakeMovePosition> snakeMovePositionList;
    private List<SnakeBodyPart> snakeBodyPartList;
    private LevelGrid.FoodType foodType, oldFoodType;
    
    public void Setup(LevelGrid levelGrid) {
        this.levelGrid = levelGrid;

    }

    private void Awake(){
        gridPosition=new Vector2Int(10,10);
        
        gridMoveTimerMax = 0.3f;
        maxSpeed = 0.1f;
        speedIncreasePercent = 5;
        speedDecreasePercent = 50;
        gridMoveTimer = gridMoveTimerMax;
        gridMoveTimerNew = gridMoveTimerMax;
        currentSpeed = gridMoveTimerMax;
        gridMoveDirection = Direction.Right;
        snakeMovePositionList = new List<SnakeMovePosition>();
        snakeBodySize = 0;

        snakeBodyPartList = new List<SnakeBodyPart>();

        state = State.Alive;
    }
    

    // Update is called once per frame
    private void Update()
    {
        switch (state){
            default:
            case State.Alive:
                HandleInput();
                HandleGridMovement();
                break;
            case State.Dead:
                break;
        }
    }

    private void HandleInput (){
        if (Input.GetKeyDown(KeyCode.UpArrow)){
                if (gridMoveDirection!=Direction.Down){
                    gridMoveDirection = Direction.Up;
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)){
                if (gridMoveDirection!=Direction.Up){
                    gridMoveDirection =Direction.Down;
                    
            }

        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)){
                if (gridMoveDirection!=Direction.Right){
                    gridMoveDirection =Direction.Left;
                    
            }

        }
        if (Input.GetKeyDown(KeyCode.RightArrow)){
                if (gridMoveDirection!=Direction.Left){
                    gridMoveDirection = Direction.Right;
            }
        }
    }

    private void HandleGridMovement(){
        gridMoveTimer += Time.deltaTime;
        if (gridMoveTimer >= gridMoveTimerNew){
            gridMoveTimer -= gridMoveTimerNew;
            
            SoundManager.PlaySound(SoundManager.Sound.SnakeMove);


            SnakeMovePosition previousSnakeMovePosition = null;
            if (snakeMovePositionList.Count>0){
                previousSnakeMovePosition=snakeMovePositionList[0];
            }
            SnakeMovePosition snakeMovePosition = new SnakeMovePosition(previousSnakeMovePosition, gridPosition, gridMoveDirection);
            snakeMovePositionList.Insert(0, snakeMovePosition);
            
            Vector2Int gridMoveDirectionVector;
            switch (gridMoveDirection){
                default:
                case Direction.Right: gridMoveDirectionVector = new Vector2Int(1,0); break;
                case Direction.Left: gridMoveDirectionVector = new Vector2Int(-1,0); break;
                case Direction.Up: gridMoveDirectionVector = new Vector2Int(0,1); break;
                case Direction.Down: gridMoveDirectionVector = new Vector2Int(0,-1); break;
            } 

            gridPosition += gridMoveDirectionVector;

            gridPosition = levelGrid.ValidateGridPosition(gridPosition);
            foodType=levelGrid.GetFoodType();
            bool eatFood = levelGrid.TrySnakeEatFood(gridPosition);

            if(levelGrid.GetTryResult()==LevelGrid.TryResult.Timeout){
                levelGrid.SpawnFood();
            }else if(eatFood){
                switch (foodType){
                    default:
                    case LevelGrid.FoodType.Food:
                        //snake ate food grow body
                        SoundManager.PlaySound(SoundManager.Sound.SnakeEatFood);
                        snakeBodySize++;
                        CreateSnakeBodyPart();
                        if (oldFoodType==LevelGrid.FoodType.Lightning){
                            ChangeSnakeSpeed(SpeedAction.ResetToPrevious);
                        }
                        ChangeSnakeSpeed(SpeedAction.Increase);
                        Score.AddScore(foodType);
                        levelGrid.SpawnFood();

                    break;    
                    case LevelGrid.FoodType.Snail:
                    //snake ate snail slow down until next food
                        SoundManager.PlaySound(SoundManager.Sound.SnakeEatSnail);
                        ChangeSnakeSpeed(SpeedAction.Min);
                        levelGrid.SpawnFood();

                    break;
                    case LevelGrid.FoodType.Lightning:
                        //snake ate lightning, max speed until next food
                        SoundManager.PlaySound(SoundManager.Sound.SnakeEatLightning);
                        ChangeSnakeSpeed(SpeedAction.Max);
                        levelGrid.SpawnFood();

                    break;
                    case LevelGrid.FoodType.Star:
                        SoundManager.PlaySound(SoundManager.Sound.SnakeEatStar);
                        if (oldFoodType==LevelGrid.FoodType.Lightning){
                            ChangeSnakeSpeed(SpeedAction.ResetToPrevious);
                        }
                        Score.AddScore(foodType);
                        levelGrid.SpawnFood();

                    break;
                    case LevelGrid.FoodType.Rotten:
                        //snakeBodySize--;
                        SoundManager.PlaySound(SoundManager.Sound.SnakeEatRotten);
                        Score.AddScore(foodType);
                        if (oldFoodType==LevelGrid.FoodType.Lightning){
                            ChangeSnakeSpeed(SpeedAction.ResetToPrevious);
                        }
                        ChangeSnakeSpeed(SpeedAction.Decrease);
                        levelGrid.SpawnFood();
                    break;
                }
                oldFoodType=foodType;
            }
           
            bool hitWall = levelGrid.TrySnakeHitWall(gridPosition);
            if (levelGrid.GetTryResult()==LevelGrid.TryResult.WallTimeout){
                levelGrid.SpawnWall();
            } else if (levelGrid.GetTryResult()==LevelGrid.TryResult.HitWall){
                //DEAD
                state=State.Dead;
                GameHandler.SnakeDied();
                SoundManager.PlaySound(SoundManager.Sound.Crash);
            }
            
            if (snakeMovePositionList.Count >= snakeBodySize + 1) {
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
            }

            UpdateSnakeBodyParts();

            foreach(SnakeBodyPart snakeBodyPart in snakeBodyPartList) {
                Vector2Int snakeBodyPartGridPosition = snakeBodyPart.GetGridPosition();
                if (gridPosition == snakeBodyPartGridPosition){
                    //GameOver
                    //CMDebug.TextPopup("DEAD!", transform.position);
                    state=State.Dead;
                    GameHandler.SnakeDied();
                    SoundManager.PlaySound(SoundManager.Sound.SnakeDie);
                }
            }


            transform.position=new Vector3(gridPosition.x, gridPosition.y);
            transform.eulerAngles = new Vector3(0,0,GetAngleFromVector(gridMoveDirectionVector)-90);

        }       
    }

    private void CreateSnakeBodyPart() {
        snakeBodyPartList.Add(new SnakeBodyPart(snakeBodyPartList.Count));
    }

    private void UpdateSnakeBodyParts(){
        for (int i=0; i<snakeBodyPartList.Count; i++){
                snakeBodyPartList[i].SetSnakeMovePosition(snakeMovePositionList[i]);
            }
    }

    private void ChangeSnakeSpeed(SpeedAction action) {
        // don't go any faster than max speed
        switch (action) {
            default:
            case SpeedAction.Max:
                currentSpeed=gridMoveTimerNew;
                gridMoveTimerNew=maxSpeed;
                break;
            case SpeedAction.Min:
                currentSpeed=gridMoveTimerNew;
                gridMoveTimerNew=gridMoveTimerMax;
                break;
            case SpeedAction.Increase:
                if (gridMoveTimerNew>maxSpeed){
                    currentSpeed=gridMoveTimerNew;
                    gridMoveTimerNew -= (speedIncreasePercent/100f)*gridMoveTimerNew;
                }    
                break;
            case SpeedAction.Decrease:
                if (gridMoveTimerNew<maxSpeed){
                    currentSpeed=gridMoveTimerNew;
                    gridMoveTimerNew += (speedDecreasePercent/100f)*gridMoveTimerNew;
                }    
                break;
            case SpeedAction.ResetToPrevious:
                gridMoveTimerNew=currentSpeed;
                break;
        }
        //currentSpeed=gridMoveTimer;
        //Debug.Log("gridMoveTimerNew: "+gridMoveTimerNew);
    }

    private float GetAngleFromVector(Vector2Int dir){
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n<0) n += 360;
        return n;
    }

    public Vector2Int GetGridPosition(){
        return gridPosition;
    }

    //Returns the full list of snake positions Head + Body
    public List<Vector2Int> getFullSnakeGridPositionList() {
            List<Vector2Int> gridPositionList = new List<Vector2Int>() { gridPosition };
            foreach (SnakeMovePosition snakeMovePosition in snakeMovePositionList) {
                gridPositionList.Add(snakeMovePosition.GetGridPosition());
            }
            return gridPositionList;
    }

    private class SnakeBodyPart {
        
        private SnakeMovePosition snakeMovePosition;
        private Transform transform;
        
        public SnakeBodyPart(int bodyIndex) {
            GameObject snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snakeBodySprite;
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -bodyIndex;
            transform = snakeBodyGameObject.transform;
        }

        public void SetSnakeMovePosition(SnakeMovePosition snakeMovePosition) {
            this.snakeMovePosition = snakeMovePosition;
            transform.position = new Vector3(snakeMovePosition.GetGridPosition().x, snakeMovePosition.GetGridPosition().y);
            
            float angle;
            switch (snakeMovePosition.GetDirection()){
                default:
                case Direction.Up:
                    switch (snakeMovePosition.GetPreviousDirection()) {
                        default: 
                            angle = 0; break;
                        case Direction.Left:
                            angle = 45; break;
                        case Direction.Right:
                            angle = -45; break;
                    }
                    break;
                case Direction.Down:
                    switch (snakeMovePosition.GetPreviousDirection()) {
                        default: 
                            angle = 180; break;
                        case Direction.Left:
                            angle = 180-45; break;
                        case Direction.Right:
                            angle = 180+45; break;
                    }
                    break;
                case Direction.Left:
                    switch (snakeMovePosition.GetPreviousDirection()) {
                        default: 
                            angle = -90; break;
                        case Direction.Down:
                            angle = -45; break;
                        case Direction.Up:
                            angle = 45; break;
                    }
                    break;
                case Direction.Right:
                    switch (snakeMovePosition.GetPreviousDirection()) {
                        default: 
                            angle = 90; break;
                        case Direction.Down:
                            angle = 45; break;
                        case Direction.Up:
                            angle = -45; break;                            
                    }
                    break;
            }
            transform.eulerAngles = new Vector3(0,0, angle);
        }

        public Vector2Int GetGridPosition() {
            return snakeMovePosition.GetGridPosition();
        }

    }


    // Handles one move position from the snake
    private class SnakeMovePosition {
        
        private SnakeMovePosition previousSnakeMovePosition;
        private Vector2Int gridPosition;
        private Direction direction;

        public SnakeMovePosition(SnakeMovePosition previousSnakeMovePosition, Vector2Int gridPosition, Direction direction) {
            this.previousSnakeMovePosition = previousSnakeMovePosition;
            this.gridPosition = gridPosition;
            this.direction = direction;
        }

        public Vector2Int GetGridPosition(){
            return gridPosition;
        }

        public Direction GetDirection(){
            return direction;
        }

        public Direction GetPreviousDirection(){
            if (previousSnakeMovePosition ==null) {
                return Direction.Right;
            }else{
            return previousSnakeMovePosition.direction;
            }
        }
    }
}
