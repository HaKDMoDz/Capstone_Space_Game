using UnityEngine;
using UnityEngine.UI;
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

    public List<Transform> planets = new List<Transform>();

    [SerializeField]
    private GameObject systemRingGUI;
    public GameObject SystemRingGUI
    {
        get { return systemRingGUI; }
        set { systemRingGUI = value; }
    }
    //Internal
    private string systemName;
    private Text systemLabel;
    private SystemObject[] systemObjects;
    private Dictionary<SystemObject, SystemObjectInfo> systemObject_info_table = new Dictionary<SystemObject, SystemObjectInfo>();
    
    public bool OnScreen = false;

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
        
        systemRingGUI.SetActive(false);
        systemLabel.gameObject.SetActive(true);
        systemLabel.text = systemName;

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

        systemLabel.gameObject.SetActive(false);
        systemRingGUI.SetActive(true);

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

    void OnBecameInvisible()
    {
        OnScreen = false;
    }

    void OnBecameVisible()
    {
        OnScreen = true;
    }

    private void Awake()
    {
        if (transform.FindChild("SystemRingGUI").FindChild("Image").renderer.isVisible)
        {
            OnScreen = true;
        }
        systemObjects = GetComponentsInChildren<SystemObject>();
        #if FULL_DEBUG || LOW_DEBUG
        if (systemObjects == null || systemObjects.Length == 0)
        {
            Debug.LogError("No system objects found");
        }
        #endif
        systemLabel = GameObject.Find("SystemLabel").GetComponent<Text>();
        #if FULL_DEBUG || LOW_DEBUG
        if(systemLabel == null)
        {
            Debug.LogError("No system label found");
        }
        #endif
        //systemName = systemRingGUI.GetComponentInChildren<Text>().text;
        systemName = gameObject.name;
    }
    private void Start()
    {
        systemLabel.gameObject.SetActive(false);
        
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
            //sysObjTrans.gameObject.SetActive(false);    
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == TagsAndLayers.MotherShipTag)
        {
            #if FULL_DEBUG
            //Debug.Log("Mothership entered system");
	        #endif
            StartCoroutine("OpenSystem");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        
        if (other.tag == TagsAndLayers.MotherShipTag)
        {
            #if FULL_DEBUG
            //Debug.Log("Mothership left system");
            #endif
            StartCoroutine("CloseSystem");
        }
    }
    #endregion UnityCallbacks
    #endregion Methods
}
