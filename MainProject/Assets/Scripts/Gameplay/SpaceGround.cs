using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class SpaceGround : Singleton<SpaceGround>, IPointerClickHandler
{
    public delegate void GroundRightClick(Vector3 worldPosition);
    public event GroundRightClick OnGroundRightClick = new GroundRightClick((Vector3) => { });

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button==PointerEventData.InputButton.Left)
        {
            //Debug.Log("Click on ground at position: " + eventData.worldPosition);
            OnGroundRightClick(eventData.worldPosition);
        }
    }
}
