using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(IEffect<>), true)]
public class AbilityEffectDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var root = new VisualElement();

        // Draw immediate children of the managed reference, preserving nested fields via PropertyField
        var iterator = property.Copy();
        var end = iterator.GetEndProperty();
        bool enterChildren = true;
        while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, end))
        {
            // Only draw direct children (one level deeper than the parent)
            if (iterator.depth == property.depth + 1)
            {
                var childCopy = iterator.Copy();
                var field = new PropertyField(childCopy);
                field.Bind(property.serializedObject);
                root.Add(field);
                enterChildren = false; // don't go deeper automatically
                continue;
            }

            // If we bubbled back up to the same or higher level, stop
            if (iterator.depth <= property.depth)
                break;

            // Otherwise, keep to siblings only
            enterChildren = false;
        }

        return root;
    }
}
