using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public int groundLayer;
    public float movementEpsilon = 0.2f;

    public GameObject projectile;

    Transform _trans;

    public int maxActionPoints = 2;
    int currentAP;

    Color originalColor;
    Material _mat;

    bool active;

    void Awake()
    {
        _trans = transform;
        _mat = renderer.material;
        originalColor = _mat.color;
    }

    public IEnumerator StartTurn()
    {
        Debug.Log("Start Player Turn");
        _mat.color = Color.green;
        active = true;
        currentAP = maxActionPoints;

        while(!Input.GetKeyDown(KeyCode.Space) && currentAP>0)
        {
            if(Input.GetMouseButtonDown(1))
            {
                currentAP--;
                yield return StartCoroutine(Move(Input.mousePosition));
            }
            else if(Input.GetMouseButtonDown(0))
            {
                currentAP--;
                yield return StartCoroutine(Shoot(Input.mousePosition));
            }
            
            yield return null;
        }
        _mat.color = originalColor;
        active = false;
    }

    IEnumerator Move(Vector3 mousePos)
    {
        Vector3 dest = GetWorldCoordsFromMouse(mousePos);
        Vector3 moveDir = dest - _trans.position;
        while(Vector3.SqrMagnitude(moveDir)>movementEpsilon*movementEpsilon)
        {
            _trans.Translate(moveDir * Time.deltaTime);
            moveDir = dest - _trans.position;
            yield return null;
        }
        Debug.Log("End Movement");
    }

    IEnumerator Shoot(Vector3 mousePos)
    {
        Vector3 aimPos = GetWorldCoordsFromMouse(mousePos);
        Vector3 shootDir = aimPos - _trans.position;
        shootDir.Normalize();
        GameObject bullet = Instantiate(projectile, _trans.position + _trans.forward, Quaternion.identity) as GameObject;
        bullet.rigidbody.AddForce(shootDir * 500f);
        yield return new WaitForSeconds(1f);
    }

   

    Vector3 GetWorldCoordsFromMouse(Vector3 mousePos)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, 0f));
        RaycastHit hit;

        Vector3 worldCoords = _trans.position;

        if (Physics.Raycast(ray, out hit, 1000f, 1 << groundLayer))
        {
            worldCoords = hit.point;
            worldCoords.y = _trans.position.y;
        }
        return worldCoords;
    }

    void OnGUI()
    {
        if(active)
        {
            GUI.Label(new Rect(5f, Screen.height - 75f, 500f, 50f), "<size=24> Action Points: " + currentAP + "</size>");
        }
    }
}
