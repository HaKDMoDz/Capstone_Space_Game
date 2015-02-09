using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UI_Billboard : MonoBehaviour 
{
    //[SerializeField]
    private Transform mainCamera;

    private Transform trans;

    void Start()
    {
        trans = transform;
        var mainCamScript = Camera.main.GetComponent<GalaxyCamera>();
        if(mainCamScript)
        {
            mainCamera = GalaxyCamera.Instance.transform;
            GalaxyCamera.Instance.OnCameraMove += OnCameraMove;
        }
        else
        {
            var camScript = Camera.main.GetComponent<CameraDirector>();
            mainCamera = CameraDirector.Instance.transform;
            CameraDirector.Instance.OnCameraMove += OnCameraMove;
        }
        
        OnCameraMove();
    }

    void OnCameraMove()
    {
        if (trans)
        {
            trans.rotation = mainCamera.rotation;
        }
    }
	
}
