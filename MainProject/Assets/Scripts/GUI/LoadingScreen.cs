using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadingScreen : MonoBehaviour 
{
    [SerializeField]
    private FillBar loadingbar;

    private AsyncOperation async;

    public IEnumerator LoadLevel(string level)
    {
        Debug.LogWarning("Loading sync level " + level);
        loadingbar.SetValue(0.0f);

        async = Application.LoadLevelAsync(level);
        yield return async;

        //while(!async.isDone)
        //{
        //    loadingbar.SetValue(async.progress);
        //    Debug.Log("Loading Progress: " + async.progress);
        //    yield return null;  
        //}
    }
    
    void Update()
    {
        if(async!=null)
        {
            loadingbar.SetValue(async.progress);
        }
    }

}
