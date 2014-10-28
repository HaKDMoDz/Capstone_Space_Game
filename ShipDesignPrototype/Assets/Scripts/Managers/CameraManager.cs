using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField]
    Transform background;
    [SerializeField]
    List<HullCamInfoEntry> hull_camInfoTable;

    Camera cam;

    void Awake()
    {
        cam = camera;
        foreach (HullCamInfoEntry info in hull_camInfoTable)
        {
            if(hull_camInfoTable
                .FindAll(item => item.hull == info.hull)
                .Count > 1)
            {
                Debug.LogError("More than 1 occurance of Hull " + info.hull + "in Hull_CamInfo Table");
            }
        }
    }

    public void HullDisplayed(Hull hull)
    {
        //Debug.Log(hull);
        HullCamInfoEntry camInfo = hull_camInfoTable.Find(item => item.hull == hull);
        float ratio = camInfo.orthoSize / cam.orthographicSize;
        background.localScale *= ratio;
        cam.transform.position = camInfo.camPos;
        cam.orthographicSize = camInfo.orthoSize;
    }
}
[Serializable]
public class HullCamInfoEntry
{
    public Hull hull;
    public Vector3 camPos;
    public float orthoSize;
}