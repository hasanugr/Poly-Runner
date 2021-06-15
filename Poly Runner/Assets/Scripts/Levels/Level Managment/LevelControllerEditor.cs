using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LevelController))]
public class LevelControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelController myScript = (LevelController)target;
        if (GUILayout.Button("Create Level"))
        {
            myScript.TestCreateLoadedLevel();
        }

    }
}
