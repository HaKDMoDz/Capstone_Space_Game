using UnityEngine;
using UnityEditor;
using System.Collections;

public class AddEmptyAsChild : MonoBehaviour 
{
    [MenuItem("GameObject/Create Empty Child #&c")]

    static void CreateEmpty()
    {
        GameObject empty = new GameObject("GameObject");
        Transform emptyTrans = empty.transform;
        Transform selectTrans = Selection.activeTransform;
        if(selectTrans)
        {
            emptyTrans.parent = selectTrans;
            emptyTrans.Translate(selectTrans.position);

        }
    }

}
