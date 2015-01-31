using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SolarSystem : MonoBehaviour
{
    #region Fields

    private SystemObject[] systemObjects;
    public SystemObject[] SystemObjects
    {
        get { return systemObjects; }
    }




    #endregion Fields
    #region Methods

    #region PrivateMethods

    private IEnumerator OpenSystem()
    {
        foreach (GameObject obj in systemObjects.Select(s => s.gameObject))
        {
            obj.SetActive(true);
        }
        yield return null;
    }

    private IEnumerator CloseSystem()
    {
        foreach (GameObject obj in systemObjects.Select(s => s.gameObject))
        {
            obj.SetActive(false);
        }
        yield return null;
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
        foreach (GameObject obj in systemObjects.Select(s => s.gameObject))
        {
            obj.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == TagsAndLayers.MotherShipTag)
        {
            #if FULL_DEBUG
            Debug.Log("Mothership entered system");
	        #endif

            StartCoroutine(OpenSystem());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        
        if (other.tag == TagsAndLayers.MotherShipTag)
        {
            #if FULL_DEBUG
            Debug.Log("Mothership left system");
            #endif
            StartCoroutine(CloseSystem());
        }
    }
    #endregion UnityCallbacks
    #endregion Methods
}
