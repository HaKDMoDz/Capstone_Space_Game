using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mothership : MonoBehaviour
{
    #region Fields

    //EditorExposed
    [SerializeField]
    private SpaceGround spaceGround;
    [SerializeField]
    private ShipMove shipMove;

    #endregion Fields

    #region Methods

    #region UnityCallbacks
    private void Start()
    {
        shipMove.Init();
        spaceGround.OnGroundRightClick += OnGroundClick;
    }

    #endregion UnityCallbacks

    #region InternalCallbacks
    void OnGroundClick(Vector3 worldPosition)
    {
        shipMove.destination = worldPosition;
        StartCoroutine(shipMove.Move());
    }
    #endregion InternalCallbacks
    #endregion Methods
}
