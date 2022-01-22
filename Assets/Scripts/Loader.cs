using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    
    public enum Scene {
        GameScene,
        Loading,
        MainMenu
    }
    
    public static Action loaderCallbackAction;

    public static void Load(Scene scene){
        
        // setup callback action that will be triggered after the loading scene is loaded
        loaderCallbackAction = () =>{
            // Load targetscene after loading scene completed
            SceneManager.LoadScene(scene.ToString());
        };

        //Load loading scene
        SceneManager.LoadScene(Scene.Loading.ToString());
    }

    public static void LoaderCallback(){
        if (loaderCallbackAction != null){
            loaderCallbackAction();
            loaderCallbackAction = null;
        }
    }
  
}
