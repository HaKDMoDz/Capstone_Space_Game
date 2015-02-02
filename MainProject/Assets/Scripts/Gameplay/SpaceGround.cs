using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class SpaceGround : Singleton<SpaceGround>, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler
{
    public delegate void GroundClick(Vector3 worldPosition);
    public event GroundClick OnGroundClick = new GroundClick((Vector3) => { });

    public delegate void GroundHold(Vector3 worldPosition);
    public event GroundHold OnGroundHold = new GroundHold((Vector3) => { });

    private bool holding = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        //if (eventData.button == PointerEventData.InputButton.Left)
        //{
        //    holding = true;
        //    OnGroundClick(eventData.worldPosition);
        //}
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        holding = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            holding = true;
            OnGroundClick(eventData.worldPosition);
        }
    }

    //private void Start()
    //{
    //    //InputManager.Instance.RegisterMouseButtonsHold(MouseDown, MouseButton.Left);
    //}

    //void MouseDown(MouseButton btn)
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    RaycastHit hit;
    //    if (Physics.Raycast(ray, out hit, 1000.0f, 1 << TagsAndLayers.SpaceGroundLayer))
    //    {
    //        holding = true;
    //        OnGroundClick(hit.point);
    //    }
    //}
}
