#region Using

using System.Text;
using UnityEditor;
using UnityEngine;

#endregion

public sealed class EditorWindowBlendShapes : EditorWindow
{
    private Vector2 scroll;
    private SkinnedMeshRenderer skinnedMeshRenderer;

    [MenuItem("Reflecta/Inspect/Blend Shapes")]
    public static void Initialize()
    {
        GetWindow<EditorWindowBlendShapes>("Blend Shapes");
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        skinnedMeshRenderer =
            (SkinnedMeshRenderer) EditorGUILayout.ObjectField(skinnedMeshRenderer, typeof (SkinnedMeshRenderer), true);
        EditorGUI.EndChangeCheck();

        if (skinnedMeshRenderer != null)
        {
            var mesh = skinnedMeshRenderer.sharedMesh;

            var sb = new StringBuilder();
            sb.AppendLine("public enum BlendShapes : int");
            sb.AppendLine("{");
            sb.AppendLine(string.Format("\t{0} = {1},", "Unknown", -1));
            sb.AppendLine();
            for (var i = 0; i < mesh.blendShapeCount; i++)
                sb.AppendLine(string.Format("\t{0} = {1},", mesh.GetBlendShapeName(i).Replace('.', '_'), i));
            sb.AppendLine();
            sb.AppendLine(string.Format("\t{0} = {1}", "LastBlendShape", mesh.blendShapeCount));
            sb.AppendLine("}");

            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.TextArea(sb.ToString());
            EditorGUILayout.EndScrollView();
        }
    }
}