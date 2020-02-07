using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(Reward))]
[CanEditMultipleObjects]
public class RewardEditor : Editor
{
    FieldInfo[] fields;

    string[] rewardProperties;

    int selectedIndex;

    private void OnEnable()
    {
        fields = typeof(ShipStats).GetFields(BindingFlags.Public | BindingFlags.Instance);

        rewardProperties = new string[fields.Length];

        for (int i = 0; i < rewardProperties.Length; i++)
        {
            rewardProperties[i] = fields[i].Name;
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        selectedIndex = EditorGUILayout.Popup("Type", selectedIndex, rewardProperties);

        ((Reward)target).statField = fields[selectedIndex];
    }
}
