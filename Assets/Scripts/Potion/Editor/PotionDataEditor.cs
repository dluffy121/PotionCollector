using UnityEngine;
using UnityEditor;
using System.Linq;

namespace PC.Editor
{
    [CustomEditor(typeof(PotionData))]
    public class PotionDataEditor : UnityEditor.Editor
    {
        private bool showBasicInfo = true;
        private bool showVisuals = true;
        private bool showAddressables = true;
        private bool showEffects = true;
        private bool showBehavior = true;

        public override void OnInspectorGUI()
        {
            PotionData potionData = (PotionData)target;

            EditorGUI.BeginChangeCheck();

            // Basic Info
            showBasicInfo = EditorGUILayout.BeginFoldoutHeaderGroup(showBasicInfo, "Basic Information");
            if (showBasicInfo)
            {
                EditorGUI.indentLevel++;
                potionData.potionName = EditorGUILayout.TextField("Name", potionData.potionName);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Description");
                potionData.description = EditorGUILayout.TextArea(potionData.description, GUILayout.Height(60));
                EditorGUILayout.EndHorizontal();

                potionData.potency = EditorGUILayout.IntSlider("Potency", potionData.potency, 1, 100);
                potionData.type = (PotionType)EditorGUILayout.EnumPopup("Type", potionData.type);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            // Visual Elements
            showVisuals = EditorGUILayout.BeginFoldoutHeaderGroup(showVisuals, "Visual Elements");
            if (showVisuals)
            {
                EditorGUI.indentLevel++;
                potionData.potionColor = EditorGUILayout.ColorField("Color", potionData.potionColor);
                potionData.icon = (Sprite)EditorGUILayout.ObjectField("Icon", potionData.icon, typeof(Sprite), false);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            // Addressables
            showAddressables = EditorGUILayout.BeginFoldoutHeaderGroup(showAddressables, "Addressables Settings");
            if (showAddressables)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("prefabReference"));

                SerializedProperty labelsProperty = serializedObject.FindProperty("labels");
                EditorGUILayout.LabelField("Labels");
                EditorGUI.indentLevel++;
                for (int i = 0; i < labelsProperty.arraySize; i++)
                {
                    SerializedProperty labelProp = labelsProperty.GetArrayElementAtIndex(i);
                    EditorGUILayout.BeginHorizontal();
                    labelProp.stringValue = EditorGUILayout.TextField($"Label {i + 1}", labelProp.stringValue);
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        labelsProperty.DeleteArrayElementAtIndex(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button("Add Label"))
                {
                    labelsProperty.InsertArrayElementAtIndex(labelsProperty.arraySize);
                    labelsProperty.GetArrayElementAtIndex(labelsProperty.arraySize - 1).stringValue = "";
                }
                EditorGUI.indentLevel--;

                if (GUILayout.Button("Update Labels"))
                {
                    UpdateAddressableLabels(potionData);
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            // Effects
            showEffects = EditorGUILayout.BeginFoldoutHeaderGroup(showEffects, "Effects");
            if (showEffects)
            {
                EditorGUI.indentLevel++;
                potionData.duration = EditorGUILayout.FloatField("Effect Duration", potionData.duration);
                potionData.collectEffect = (ParticleSystem)EditorGUILayout.ObjectField("Collect Effect", potionData.collectEffect, typeof(ParticleSystem), false);
                potionData.collectSound = (AudioClip)EditorGUILayout.ObjectField("Collect Sound", potionData.collectSound, typeof(AudioClip), false);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            // Behavior
            showBehavior = EditorGUILayout.BeginFoldoutHeaderGroup(showBehavior, "Behavior Settings");
            if (showBehavior)
            {
                EditorGUI.indentLevel++;
                potionData.rotationSpeed = EditorGUILayout.Slider("Rotation Speed", potionData.rotationSpeed, 0f, 360f);
                potionData.bobAmplitude = EditorGUILayout.FloatField("Bob Amplitude", potionData.bobAmplitude);
                potionData.bobFrequency = EditorGUILayout.FloatField("Bob Frequency", potionData.bobFrequency);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(potionData);
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void UpdateAddressableLabels(PotionData potion)
        {
            string[] defaultLabels = new[] { "Potion", potion.type.ToString(), $"Potency_{potion.potency}" };
            potion.labels = defaultLabels;
            EditorUtility.SetDirty(potion);
        }
    }
}