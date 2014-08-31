using UnityEngine;
using System.Collections;

public class ShipAttack : MonoBehaviour 
{

    public Weapon weapon;

    void Start()
    {
        InputManager.Instance.OnMouseClick += OnMouseClick;
    }

    void OnMouseClick(MouseEventArgs args)
    {
        if(args.button==0)
        {
            //weapon.Fire();
        }
    }
   

}
