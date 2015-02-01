﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[Serializable]
public struct SystemObjectInfo
{
    public Vector3 scenePosition;
    public Vector3 sceneScale;

}
public class SolarSystem : MonoBehaviour
{
    #region Fields


    //Internal
    private SystemObject[] systemObjects;
    private Dictionary<SystemObject, SystemObjectInfo> systemObject_info_table = new Dictionary<SystemObject, SystemObjectInfo>();

    #endregion Fields
    #region Methods

    #region PrivateMethods

    private IEnumerator OpenSystem()
    {
        StopCoroutine("CloseSystem");
        foreach (GameObject obj in systemObjects.Select(s => s.gameObject))
        {
            obj.SetActive(true);
        }

        float time = 0.0f;
        while(time<1.0f)
        {

            foreach (var obj_info in systemObject_info_table)
            {
                Transform trans_SysObj = obj_info.Key.transform;
                trans_SysObj.localPosition = Vector3.Lerp(trans_SysObj.localPosition, obj_info.Value.scenePosition, time);
                trans_SysObj.localScale = Vector3.Lerp(trans_SysObj.localScale, obj_info.Value.sceneScale, time);
            }

            time += Time.deltaTime / GalaxyConfig.SystemAnimationPeriod;
            yield return null;
        }
    }

    private IEnumerator CloseSystem()
    {
        StopCoroutine("OpenSystem");
        float time = 0.0f;
        while (time < 1.0f)
        {

            foreach (var obj_info in systemObject_info_table)
            {
                Transform trans_SysObj = obj_info.Key.transform;
                trans_SysObj.localPosition = Vector3.Lerp(trans_SysObj.localPosition, Vector3.zero, time);
                trans_SysObj.localScale = Vector3.Lerp(trans_SysObj.localScale, Vector3.zero, time);
            }

            time += Time.deltaTime / GalaxyConfig.SystemAnimationPeriod;
            yield return null;
        }
        foreach (GameObject obj in systemObjects.Select(s => s.gameObject))
        {
            obj.SetActive(false);
        }
    }

    #endregion PrivateMethods

    #region UnityCallbacks
    private void Awake()
    {
        systemObjects = GetComponentsInChildren<SystemObject>();
        #if FULL_DEBUG || LOW_DEBUG
        if (systemObjects == null || systemObjects.Length == 0)
        {
            Debug.LogError("No system objects found");
        }
        #endif
    }
    private void Start()
    {
        foreach (SystemObject sysObj in systemObjects)
        {
            #if FULL_DEBUG
            if(systemObject_info_table.ContainsKey(sysObj))
            {
                Debug.LogError("Duplicate System Object found");
            }
            #endif
            Transform sysObjTrans = sysObj.transform;
            systemObject_info_table.Add(sysObj, new SystemObjectInfo { scenePosition = sysObjTrans.localPosition, sceneScale = sysObjTrans.localScale });
            sysObjTrans.localPosition = Vector3.zero;
            sysObjTrans.localScale = Vector3.zero;
            sysObjTrans.gameObject.SetActive(false);    
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == TagsAndLayers.MotherShipTag)
        {
            #if FULL_DEBUG
            Debug.Log("Mothership entered system");
	        #endif
            StartCoroutine("OpenSystem");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        
        if (other.tag == TagsAndLayers.MotherShipTag)
        {
            #if FULL_DEBUG
            Debug.Log("Mothership left system");
            #endif
            StartCoroutine("CloseSystem");
        }
    }
    #endregion UnityCallbacks
    #endregion Methods
}
