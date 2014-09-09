using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AIShip : TurnBasedUnit 
{
    [SerializeField] float maxHullHP;
    [SerializeField]
    Slider healthBarSlider;

    float hullHP;

    public override void Awake()
    {
        base.Awake();
        hullHP = maxHullHP;
    }
    public override IEnumerator ExecuteTurn()
    {
        Debug.Log(unitName + " (AI Ship) starts turn");
        projector.enabled = true;

        yield return base.ExecuteTurn();

        yield return new  WaitForSeconds(1.0f);

        projector.enabled = false;
        Debug.Log(unitName + " (AI Ship) Ends turn");
    }

    public void TakeDamage(float damage)
    {
        hullHP -= damage;
        Debug.Log("Hull HP: " + hullHP);
        healthBarSlider.value -= damage/maxHullHP;
    }

}
