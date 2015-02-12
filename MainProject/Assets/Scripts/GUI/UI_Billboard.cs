using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UI_Billboard : MonoBehaviour
{
    //[SerializeField]
    private Transform mainCamera;

    private Transform trans;

    private bool isActive = false;

    void Start()
    {
        if (GameConfig.GetSceneEnum(Application.loadedLevelName) == GameScene.GalaxyMap
            || GameConfig.GetSceneEnum(Application.loadedLevelName) == GameScene.CombatScene)
        {
            isActive = true;
        }
        else
        {
            isActive = false;
            return;
        }
        trans = transform;

        CameraDirector camDirector = Camera.main.GetComponent<CameraDirector>();
        GalaxyCamera galaxyCam = Camera.main.GetComponent<GalaxyCamera>();

        if (galaxyCam)
        {
            mainCamera = GalaxyCamera.Instance.transform;
            GalaxyCamera.Instance.OnCameraMove += OnCameraMove;
        }
        else if (camDirector)
        {
            mainCamera = CameraDirector.Instance.transform;
            CameraDirector.Instance.OnCameraMove += OnCameraMove;
        }

        OnCameraMove();
    }

    void OnCameraMove()
    {
        if (isActive && trans)
        {
            trans.rotation = mainCamera.rotation;
        }
    }

}
