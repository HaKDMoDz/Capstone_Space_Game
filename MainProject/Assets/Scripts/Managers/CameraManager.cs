using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

#region AdditionalData
[Serializable]
public class HullCamInfoEntry
{
    public Hull hull;
    public Vector3 camPos;
    public float orthoSize;
}
#endregion AdditionalData

public class CameraManager : Singleton<CameraManager>
{
    #region Fields
    #region EditorExposed
    [SerializeField]
    private Transform background;
    [SerializeField]
    private List<HullCamInfoEntry> hull_camInfoTable;
    #endregion //EditorExposed

    #region InternalFields
    private Camera cam;
    #endregion //InternalFields
    #endregion //Fields

    #region Methods

    #region Public

    public void HullDisplayed(Hull hull)
    {
        //Debug.Log(hull);
        HullCamInfoEntry camInfo = hull_camInfoTable.Find(item => item.hull == hull);
        float ratio = camInfo.orthoSize / cam.orthographicSize;
        background.localScale *= ratio;
        cam.transform.position = camInfo.camPos;
        cam.orthographicSize = camInfo.orthoSize;
    }
#if UNITY_EDITOR
    [ContextMenu("ResetToDetault")]
    public void ResetToDefault()
    {
        camera.orthographicSize = 13.0f;
        camera.transform.position = new Vector3(0.0f, 35.0f, 0.0f);
    }
#endif 
    #endregion //Public

    #region Private
    private void Awake()
    {
        cam = camera;
#if !RELEASE
        foreach (HullCamInfoEntry info in hull_camInfoTable)
        {

            if (hull_camInfoTable
                .FindAll(item => item.hull == info.hull)
                .Count > 1)
            {
                Debug.LogError("More than 1 occurance of Hull " + info.hull + "in Hull_CamInfo Table");
            }
        }
#endif

    }
    #endregion //Private

    #endregion //Methods


    
}
