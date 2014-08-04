using UnityEngine;
using System.Collections;

public class AICube : TurnBasedEntity 
{
    public float movementEpsilon = 0.2f;
    
    bool moveLeft = true;

    Transform _trans;
    Color originalColor;
    Material _mat;

    bool activated;

    void Start()
    {
        _trans = transform;
        _mat = renderer.material;
        originalColor = _mat.color;
    }
    public override IEnumerator StartTurn()
    {
        Debug.Log("AI Start Turn");
        _mat.color = Color.green;
        activated = true;

        yield return StartCoroutine(Move());
        _mat.color = originalColor;
        activated = false;
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator Move()
    {
        Vector3 dest;
        if(moveLeft)
        {
            dest = _trans.position+Vector3.left*5f;
        }
        else
        {
            dest = _trans.position+Vector3.right*5f;
        }
        Vector3 dir = dest - _trans.position; 
        while (Vector3.SqrMagnitude(dir) > movementEpsilon * movementEpsilon)
        {
            _trans.Translate(dir * Time.deltaTime);
            dir = dest - _trans.position;
            yield return null;
        }
        moveLeft = !moveLeft;
    }
    void OnGUI()
    {
        if (activated)
        {
            GUI.Label(new Rect(5f, Screen.height - 45f, 500f, 50f), "<size=24> Initiative: " + initiative + "</size>");
        }
    }
}
