using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(TestData))]
public class TestDataEditor : Editor 
{
    int num;

    //ReorderableList list;
    //SerializedProperty intList;
    //private static readonly GUIContent INT_LIST_HEADER = new GUIContent("Int List", "List of ints");

    //void OnEnable()
    //{
    //    intList = serializedObject.FindProperty("intList");
    //    Debug.Log(intList);
    //    list = new ReorderableList(serializedObject, intList, true, true, true, true);
    //    list.drawHeaderCallback += rect => GUI.Label(rect, INT_LIST_HEADER);
    //    list.drawElementCallback += (rect, index, active, focused) =>
    //        {
    //            rect.height = 16;
    //            rect.y += 2;
    //            if (index >= intList.arraySize) return;
    //            var num = (intList.GetArrayElementAtIndex(index).objectReferenceValue);
    //            if (num == null)
    //            {
    //                EditorGUI.LabelField(rect, "null");
    //                return;
    //            }
    //            EditorGUI.LabelField(rect, num+"");
    //        };
    //}

    [MenuItem("Data/Create Test Data")]
    static void CreateTestData()
    {
        string path = EditorUtility.SaveFilePanel("Create Test Data", "Assets/", "TestData.asset", "asset");
        if(path=="")
        {
            return;
        }
        path = FileUtil.GetProjectRelativePath(path);
        TestData testData = CreateInstance<TestData>();
        testData.intList = new List<int>();
        
        AssetDatabase.CreateAsset(testData, path);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = testData;

    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //list.DoLayoutList();

        TestData testData = target as TestData;
        num = EditorGUILayout.IntField("num", num);
        if(GUILayout.Button("Add Entry"))
        {
            testData.AddEntry(num);
            EditorUtility.SetDirty(testData);

        }

    }
}
