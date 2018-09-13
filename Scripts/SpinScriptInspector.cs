using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpinScript))]
public class SpinScriptInspector : Editor
{
    private string[] choiceLabels = new string[] { "x-axis", "y-axis", "z-axis" };

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var spinner = target as SpinScript;
        // Choose an option from the list
        spinner.ChoiceIndex = EditorGUILayout.Popup(spinner.ChoiceIndex, choiceLabels);

        EditorUtility.SetDirty(spinner);
    }
}