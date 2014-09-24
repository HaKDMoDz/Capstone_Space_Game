using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(TestData))]
public class TestDataEditor : Editor 
{
    int num;
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
        TestData testData = target as TestData;
        num = EditorGUILayout.IntField("num", num);
        if(GUILayout.Button("Add Entry"))
        {
            testData.AddEntry(num);
            EditorUtility.SetDirty(testData);

        }

    }
}
