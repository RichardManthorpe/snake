using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class GameHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("GameHandler Start");
    
        //int number=0;
        //FunctionPeriodic.Create(() => {
        //    CMDebug.TextPopupMouse("Ding! " + number);
        //    number ++;
        //}, .3f);
    
        GameObject snakeHeadGameObject = new GameObject();
        SpriteRenderer snakeSpriteRenderer = snakeHeadGameObject.AddComponent<SpriteRenderer>();
        snakeSpriteRenderer.sprite = GameAssets.i.snakeHeadSprite;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
