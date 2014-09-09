using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LookAtCamera : MonoBehaviour 
{
    [SerializeField]
    Transform mainCamera;

    Transform trans;

    void Start()
    {
        trans = transform;
        CameraDirector.Instance.OnCamerMove += OnCameraMoved;
    }

    void OnCameraMoved()
    {
        trans.LookAt(mainCamera);
    }
	
}
