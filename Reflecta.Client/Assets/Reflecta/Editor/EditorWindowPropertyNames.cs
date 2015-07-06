#region Using

using UnityEditor;
using UnityEngine;

#endregion

public sealed class EditorWindowPropertyNames : EditorWindow
{
    private Object obj;
    private Vector2 scroll;
    private SerializedObject serializedObj;

    [MenuItem("Reflecta/Inspect/Property Names")]
    public static void Initialize()
    {
        GetWindow<EditorWindowPropertyNames>("Property Names");
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();

        obj = EditorGUILayout.ObjectField(obj, typeof (Object), true);

        if (EditorGUI.EndChangeCheck())
            if (obj)
                serializedObj = new SerializedObject(obj);
            else
                serializedObj = null;

        if (serializedObj != null)
        {
            var property = serializedObj.GetIterator();
            scroll = EditorGUILayout.BeginScrollView(scroll);
            while (property.Next(true))
                EditorGUILayout.SelectableLabel(property.propertyPath);
            EditorGUILayout.EndScrollView();
        }
    }
}