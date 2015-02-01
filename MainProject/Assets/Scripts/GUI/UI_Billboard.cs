using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UI_Billboard : MonoBehaviour 
{
    [SerializeField]
    private Transform mainCamera;

    private Transform trans;

    void Start()
    {
        trans = transform;
        mainCamera = GalaxyCamera.Instance.transform;
        GalaxyCamera.Instance.OnCameraMove += OnCameraMove;
        OnCameraMove();
    }

    void OnCameraMove()
    {
        trans.rotation = mainCamera.rotation;
    }
	
}
