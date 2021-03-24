using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ItemLootTable.TableEntry))]
public class ItemDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        //var nameRect = new Rect(position.x, position.y, 30, position.height);
        var freqRect = new Rect(position.x, position.y, 35, position.height);
        var roomDataRect = new Rect(position.x + 40, position.y, position.width - 40, position.height);

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        //EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), GUIContent.none);
        EditorGUI.PropertyField(freqRect, property.FindPropertyRelative("freq"), GUIContent.none);
        EditorGUI.PropertyField(roomDataRect, property.FindPropertyRelative("value"), GUIContent.none);

        // other!
        SerializedProperty prop = property.FindPropertyRelative("value");
        if (prop.objectReferenceValue != null)
        {
            SerializedObject propObj = new SerializedObject(prop.objectReferenceValue);

            // TableEntry.name = TableEntry.value.name;
            property.FindPropertyRelative("name").stringValue = propObj.FindProperty("name").stringValue;
        }


        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
