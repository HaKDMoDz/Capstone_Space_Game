using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class ShipDesignCamera : Singleton<ShipDesignCamera> 
{
    [SerializeField]
    private float defaultOrthoSize = 13.0f;
    [SerializeField]
    private Vector3 defaultPos;
    [SerializeField]
    private Transform background;
    [SerializeField]
    private List<HullCamInfo> hullCamInfoList;


    private Camera cam;
    private Transform camTrans;

    public void SetCameraForHull(Hull hull)
    {
        HullCamInfo camInfo = hullCamInfoList.Find(info => info.hull == hull);
        float ratio = camInfo.orthoSize / cam.orthographicSize;
        background.localScale *= ratio;
        camTrans.position = camInfo.camPos;
        cam.orthographicSize = camInfo.orthoSize;
    }

    private void ResetToDefault()
    {
        cam.orthographicSize = defaultOrthoSize;
        camTrans.position = defaultPos;
    }
    

    private void Awake()
    {
        cam = camera;
        camTrans = transform;
        #if FULL_DEBUG
        hullCamInfoList.ForEach((hc) =>
            {
                if(hullCamInfoList.Count((h) => h.hull == hc.hull)
                    >1)
                {
                    Debug.LogError("More than 1 occurance of Hull: "+ hc.hull.hullName);
                }
            });
        #endif
        ResetToDefault();
    }
}

[Serializable]
public struct HullCamInfo
{
    public Hull hull;
    public Vector3 camPos;
    public float orthoSize;
}