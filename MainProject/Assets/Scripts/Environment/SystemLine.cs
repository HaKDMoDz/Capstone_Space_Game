using UnityEngine;
using System.Collections;

public class SystemLine : MonoBehaviour 
{
    public Transform FirstSystem;
    public Transform SecondSystem;
    public LineRenderer LineRenderer;
    public float Radius;

	void Awake () 
    {
        Vector3 _firstSysPosition = FirstSystem.position;
        Vector3 _secondSysPosition = SecondSystem.position;

        Vector3 _displacement = (_secondSysPosition - _firstSysPosition).normalized * Radius;

        LineRenderer.SetPosition(0, _firstSysPosition + _displacement);
        LineRenderer.SetPosition(1, _secondSysPosition - _displacement);
	}
}
