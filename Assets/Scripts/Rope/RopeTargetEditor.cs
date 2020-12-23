using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RopeTarget))]
[CanEditMultipleObjects]
public class RopeTargetEditor : Editor
{
    SerializedProperty targetGameObject;
    SerializedProperty type;
    bool showConnectionSettings = false;
    SerializedProperty autoConfigureMaxLength;
    SerializedProperty maxLength;
    SerializedProperty addDistanceConstraint;
    SerializedProperty linkScale;
    SerializedProperty linkMass;
    SerializedProperty collisionMode;
    SerializedProperty enablePrePro;
    SerializedProperty enableProjection;

    void OnEnable()
    {
        targetGameObject = serializedObject.FindProperty("targetGameObject");
        type = serializedObject.FindProperty("type");
        autoConfigureMaxLength = serializedObject.FindProperty("autoConfigureMaxLength");
        maxLength = serializedObject.FindProperty("maxLength");
        addDistanceConstraint = serializedObject.FindProperty("addDistanceConstraint");
        linkScale = serializedObject.FindProperty("linkScale");
        linkMass = serializedObject.FindProperty("linkMass");
        collisionMode = serializedObject.FindProperty("collisionMode");
        enablePrePro = serializedObject.FindProperty("enablePrePro");
        enableProjection = serializedObject.FindProperty("enableProjection");
}

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.LabelField("How to use / recommended setup:",EditorStyles.boldLabel);
        EditorGUILayout.LabelField("The topmost GameObject in the hierarchy should contain this script (and" +
            " potentially a Rigidbody). You can add more children that contain meshes and colliders" +
            " (with their tags and layers)",EditorStyles.wordWrappedLabel);
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("GameObject containing the collider used as a target for the RopeTool", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("(Tag and Layer will be overridden!)");
        EditorGUILayout.PropertyField(targetGameObject);
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Type of the connection", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(type);
        EditorGUILayout.Separator();
        showConnectionSettings = EditorGUILayout.Foldout(showConnectionSettings,"Connection Settings");
        if (showConnectionSettings)
        {
            switch ((target as RopeTarget).type)
            {
                case ROPE_TYPE.DYNAMIC_DISTANCE:
                    EditorGUILayout.PropertyField(autoConfigureMaxLength);
                    EditorGUILayout.PropertyField(maxLength);
                    break;
                case ROPE_TYPE.DYNAMIC_LINK:
                    EditorGUILayout.PropertyField(linkScale);
                    EditorGUILayout.PropertyField(linkMass);
                    EditorGUILayout.PropertyField(addDistanceConstraint, new GUIContent("Constrain max distance", "Add a distance connection together with links" +
                        " increases simulation stability but decreases 'link-ness'"));
                    EditorGUILayout.PropertyField(collisionMode);
                    EditorGUILayout.PropertyField(enablePrePro, new GUIContent("Enable preprocessing on links"));
                    EditorGUILayout.PropertyField(enableProjection);
                    break;
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}