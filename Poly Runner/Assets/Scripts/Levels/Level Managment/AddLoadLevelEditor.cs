using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(AddLevelDatas))]
public class AddLoadLevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AddLevelDatas myScript = (AddLevelDatas)target;
        if (GUILayout.Button("Load From Scene"))
        {
            myScript.LoadFromScene();
        }

        if (GUILayout.Button("Load From Object"))
        {
            myScript.LoadFromObject();
        }

        if (GUILayout.Button("Save Level"))
        {
            myScript.SaveLevel();
            EditorUtility.SetDirty(myScript.levelDesignScriptableObject);
        }
        
        if (GUILayout.Button("Clear"))
        {
            myScript.Clear(true);
        }
    }
}
