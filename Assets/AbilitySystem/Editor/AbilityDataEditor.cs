using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(AbilityData))]
public class AbilityDataEditor : Editor
{
    private SerializedProperty _iconProp;
    private SerializedProperty _labelProp;
    private SerializedProperty _descriptionProp;
    private SerializedProperty _vfxApply;
    private SerializedProperty _castTimeProp;
    private SerializedProperty _cooldownTimeProp;
    private SerializedProperty _castAnimProp;
    private SerializedProperty _effectsProp;
    private SerializedProperty _targetingProp;

    private VisualElement _root;
    private VisualElement _effectsListContainer;
    private VisualElement _targetingContainer;
    private Image _iconPreview;

    public override VisualElement CreateInspectorGUI()
    {
        serializedObject.Update();

        _iconProp = serializedObject.FindProperty("Icon");
        _labelProp = serializedObject.FindProperty("Label");
        _descriptionProp = serializedObject.FindProperty("Description");
        _vfxApply = serializedObject.FindProperty("VFXApply");
        _castTimeProp = serializedObject.FindProperty("CastTime");
        _cooldownTimeProp = serializedObject.FindProperty("CooldownTime");
        _castAnimProp = serializedObject.FindProperty("CastAnimation");
        _effectsProp = serializedObject.FindProperty("Effects");
        _targetingProp = serializedObject.FindProperty("TargetingStrategy");

        _root = new VisualElement();

        // Icon field + preview row
        var iconRow = new VisualElement
        {
            style =
            {
                flexDirection = FlexDirection.Row,
                alignItems = Align.Center,
                marginBottom = 4
            }
        };

        var iconField = new ObjectField("Icon") { objectType = typeof(Sprite) };
        iconField.BindProperty(_iconProp);
        iconField.style.flexGrow = 1f;
        iconField.RegisterValueChangedCallback(_ => RefreshIconPreview());
        iconRow.Add(iconField);

        _iconPreview = new Image
        {
            scaleMode = ScaleMode.ScaleToFit,
            image = null,
            style =
            {
                width = 64,
                height = 64,
                marginLeft = 8,
                marginRight = 2,
                borderTopColor = new Color(0, 0, 0, 0.25f),
                borderBottomColor = new Color(0, 0, 0, 0.25f),
                borderLeftColor = new Color(0, 0, 0, 0.25f),
                borderRightColor = new Color(0, 0, 0, 0.25f),
                borderTopWidth = 1,
                borderBottomWidth = 1,
                borderLeftWidth = 1,
                borderRightWidth = 1
            }
        };
        iconRow.Add(_iconPreview);
        _root.Add(iconRow);

        // Core fields
        _root.Add(new PropertyField(_labelProp, "Label") { tooltip = "Friendly name shown in tools" });
        _root.Add(new PropertyField(_descriptionProp, "Description"));
        _root.Add(new PropertyField(_castTimeProp, "Cast Time"));
        _root.Add(new PropertyField(_cooldownTimeProp, "Cooldown Time"));
        _root.Add(new PropertyField(_vfxApply, "Apply VFX"));
        _root.Add(new PropertyField(_castAnimProp, "Cast Animation"));

        // Effects header with Add button
        var headerRow = new VisualElement
        {
            style =
            {
                flexDirection = FlexDirection.Row,
                justifyContent = Justify.SpaceBetween,
                alignItems = Align.Center,
                marginTop = 6,
                marginBottom = 4
            }
        };

        var effectsLabel = new Label("Effects") { style = { unityFontStyleAndWeight = FontStyle.Bold } };
        headerRow.Add(effectsLabel);

        var addButton = new Button { text = "Add" };
        addButton.clicked += () => ShowAddMenu(addButton.worldBound);
        headerRow.Add(addButton);

        _root.Add(headerRow);

        // Effects list container
        _effectsListContainer = new VisualElement { style = { flexDirection = FlexDirection.Column } };
        _root.Add(_effectsListContainer);

        // Targeting Strategy header with Add/Change/Clear button
        var targetingHeader = new VisualElement
        {
            style =
            {
                flexDirection = FlexDirection.Row,
                justifyContent = Justify.SpaceBetween,
                alignItems = Align.Center,
                marginTop = 8,
                marginBottom = 4
            }
        };
        var targetingLabel = new Label("Targeting Strategy") { style = { unityFontStyleAndWeight = FontStyle.Bold } };
        targetingHeader.Add(targetingLabel);

        var targetingButtons = new VisualElement { style = { flexDirection = FlexDirection.Row } };
        var targetingAddOrChangeBtn = new Button { text = "Set" };
        targetingAddOrChangeBtn.clicked += () =>
        {
            var rect = targetingAddOrChangeBtn.worldBound;
            if (string.IsNullOrEmpty(_targetingProp.managedReferenceFullTypename))
                ShowTargetingAddMenu(rect);
            else
                ShowTargetingChangeMenu(rect);
        };
        targetingButtons.Add(targetingAddOrChangeBtn);

        var targetingClearBtn = new Button { text = "Clear", tooltip = "Remove targeting strategy" };
        targetingClearBtn.clicked += ClearTargeting;
        targetingButtons.Add(targetingClearBtn);

        targetingHeader.Add(targetingButtons);
        _root.Add(targetingHeader);

        // Targeting body
        _targetingContainer = new VisualElement { style = { flexDirection = FlexDirection.Column } };
        _root.Add(_targetingContainer);

        RebuildTargetingSection();
        RebuildEffectsList();

        _root.Bind(serializedObject);
        RefreshIconPreview();
        return _root;
    }

    private void RefreshIconPreview()
    {
        // Try to reflect current sprite into the preview image
        _iconProp ??= serializedObject.FindProperty("Icon");
        var sprite = _iconProp?.objectReferenceValue as Sprite;
        _iconPreview.image = sprite ? sprite.texture : null;
    }

    private void RebuildTargetingSection()
    {
        _targetingContainer.Clear();
        serializedObject.Update();
        _targetingProp ??= serializedObject.FindProperty("TargetingStrategy");
        if (_targetingProp == null) return;

        if (string.IsNullOrEmpty(_targetingProp.managedReferenceFullTypename))
        {
            var empty = new Label("None")
            {
                style =
                {
                    color = new Color(0.6f, 0.6f, 0.6f),
                    marginLeft = 4
                }
            };
            _targetingContainer.Add(empty);
            return;
        }

        // Draw a bordered box similar to effects items
        var box = new VisualElement
        {
            style =
            {
                borderTopColor = new Color(0, 0, 0, 0.25f),
                borderBottomColor = new Color(0, 0, 0, 0.25f),
                borderLeftColor = new Color(0, 0, 0, 0.25f),
                borderRightColor = new Color(0, 0, 0, 0.25f),
                borderTopWidth = 1,
                borderBottomWidth = 1,
                borderLeftWidth = 1,
                borderRightWidth = 1,
                paddingLeft = 6,
                paddingRight = 6,
                paddingTop = 4,
                paddingBottom = 6,
                marginBottom = 2
            }
        };

        var header = new VisualElement
        {
            style =
            {
                flexDirection = FlexDirection.Row,
                justifyContent = Justify.SpaceBetween,
                alignItems = Align.Center
            }
        };

        var title = new Label(GetTypeDisplayName(_targetingProp)) { style = { unityFontStyleAndWeight = FontStyle.Bold } };
        header.Add(title);

        var headerButtons = new VisualElement { style = { flexDirection = FlexDirection.Row } };
        var changeBtn = new Button(() => ShowTargetingChangeMenu(header.worldBound)) { text = "Type", tooltip = "Change targeting type" };
        var clearBtn = new Button(ClearTargeting) { text = "✕", tooltip = "Remove" };
        headerButtons.Add(changeBtn);
        headerButtons.Add(clearBtn);
        header.Add(headerButtons);
        box.Add(header);

        var body = new VisualElement { style = { marginTop = 4 } };
        var field = new PropertyField(_targetingProp) { name = "targeting_fields" };
        field.Bind(serializedObject);
        body.Add(field);
        box.Add(body);

        _targetingContainer.Add(box);
    }

    private void ShowTargetingAddMenu(Rect buttonRect)
    {
        var types = GetTargetingTypes();
        var menu = new GenericMenu();

        if (types.Count == 0)
        {
            menu.AddDisabledItem(new GUIContent("No TargetingStrategy types found"));
        }
        else
        {
            foreach (var t in types)
            {
                var nice = t.Name;
                menu.AddItem(new GUIContent(nice), false, () => SetTargeting(t));
            }
        }

        menu.DropDown(buttonRect);
    }

    private void ShowTargetingChangeMenu(Rect buttonRect)
    {
        var types = GetTargetingTypes();
        var menu = new GenericMenu();
        foreach (var t in types)
        {
            var nice = t.Name;
            menu.AddItem(new GUIContent(nice), false, () => SetTargeting(t));
        }
        menu.DropDown(buttonRect);
    }

    private void SetTargeting(Type type)
    {
        serializedObject.Update();
        Undo.RecordObject(target, "Set Targeting Strategy");
        _targetingProp ??= serializedObject.FindProperty("TargetingStrategy");
        if (_targetingProp == null) return;

        _targetingProp.managedReferenceValue = Activator.CreateInstance(type);
        _targetingProp.isExpanded = true;
        serializedObject.ApplyModifiedProperties();
        RebuildTargetingSection();
    }

    private void ClearTargeting()
    {
        serializedObject.Update();
        Undo.RecordObject(target, "Clear Targeting Strategy");
        _targetingProp ??= serializedObject.FindProperty("TargetingStrategy");
        if (_targetingProp == null) return;
        _targetingProp.managedReferenceValue = null;
        serializedObject.ApplyModifiedProperties();
        RebuildTargetingSection();
    }

    private void RebuildEffectsList()
    {
        _effectsListContainer.Clear();
        serializedObject.Update();

        _effectsProp ??= serializedObject.FindProperty("Effects");
        if (_effectsProp == null)
            return;

        if (_effectsProp.arraySize == 0)
        {
            var empty = new Label("Empty")
            {
                style =
                {
                    color = new Color(0.6f, 0.6f, 0.6f),
                    marginLeft = 4
                }
            };
            _effectsListContainer.Add(empty);
            return;
        }

        for (int i = 0; i < _effectsProp.arraySize; i++)
        {
            var index = i;
            var elementProp = _effectsProp.GetArrayElementAtIndex(index);

            var item = new VisualElement
            {
                style =
                {
                    borderTopColor = new Color(0, 0, 0, 0.25f),
                    borderBottomColor = new Color(0, 0, 0, 0.25f),
                    borderLeftColor = new Color(0, 0, 0, 0.25f),
                    borderRightColor = new Color(0, 0, 0, 0.25f),
                    borderTopWidth = 1,
                    borderBottomWidth = 1,
                    borderLeftWidth = 1,
                    borderRightWidth = 1,
                    paddingLeft = 6,
                    paddingRight = 6,
                    paddingTop = 4,
                    paddingBottom = 6,
                    marginBottom = 2
                }
            };

            // Header row
            var header = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    justifyContent = Justify.SpaceBetween,
                    alignItems = Align.Center
                }
            };

            var title = new Label(GetTypeDisplayName(elementProp)) { style = { unityFontStyleAndWeight = FontStyle.Bold } };
            header.Add(title);

            var headerButtons = new VisualElement { style = { flexDirection = FlexDirection.Row } };

            var upBtn = new Button(() => MoveEffect(index, -1)) { text = "▲", tooltip = "Move up" };
            var downBtn = new Button(() => MoveEffect(index, +1)) { text = "▼", tooltip = "Move down" };
            var changeBtn = new Button(() => ShowChangeTypeMenu(index, header.worldBound)) { text = "Type", tooltip = "Change effect type" };
            var duplicateBtn = new Button(() => DuplicateEffect(index)) { text = "◳", tooltip = "Duplicate" };
            var removeBtn = new Button(() => RemoveEffect(index)) { text = "✕", tooltip = "Remove" };

            headerButtons.Add(upBtn);
            headerButtons.Add(downBtn);
            headerButtons.Add(changeBtn);
            headerButtons.Add(duplicateBtn);
            headerButtons.Add(removeBtn);
            header.Add(headerButtons);

            item.Add(header);

            // Content (the effect fields) via property drawer
            var body = new VisualElement { style = { marginTop = 4 } };
            var field = new PropertyField(elementProp) { name = $"effect_{index}_fields" };
            field.Bind(serializedObject);
            body.Add(field);
            item.Add(body);

            _effectsListContainer.Add(item);
        }
    }

    private void ShowAddMenu(Rect buttonRect)
    {
        var types = GetEffectTypes();
        var menu = new GenericMenu();

        if (types.Count == 0)
        {
            menu.AddDisabledItem(new GUIContent("No IEffectFactory<IDamageable> types found"));
        }
        else
        {
            foreach (var t in types)
            {
                var nice = t.Name;
                menu.AddItem(new GUIContent(nice), false, () => AddEffect(t));
            }
        }

        menu.DropDown(buttonRect);
    }

    private void ShowChangeTypeMenu(int index, Rect buttonRect)
    {
        var types = GetEffectTypes();
        var menu = new GenericMenu();

        foreach (var t in types)
        {
            var nice = t.Name;
            menu.AddItem(new GUIContent(nice), false, () => ReplaceEffectAt(index, t));
        }

        menu.DropDown(buttonRect);
    }

    private void AddEffect(Type type)
    {
        serializedObject.Update();
        Undo.RecordObject(target, "Add Effect");

        if (_effectsProp == null || !_effectsProp.isArray)
        {
            _effectsProp = serializedObject.FindProperty("Effects");
        }

        _effectsProp.arraySize++;
        var newIndex = _effectsProp.arraySize - 1;
        var elementProp = _effectsProp.GetArrayElementAtIndex(newIndex);
        elementProp.managedReferenceValue = Activator.CreateInstance(type);
        elementProp.isExpanded = true;

        serializedObject.ApplyModifiedProperties();
        RebuildEffectsList();
    }

    private void ReplaceEffectAt(int index, Type newType)
    {
        serializedObject.Update();
        Undo.RecordObject(target, "Change Effect Type");

        var elementProp = _effectsProp.GetArrayElementAtIndex(index);
        elementProp.managedReferenceValue = Activator.CreateInstance(newType);
        elementProp.isExpanded = true;

        serializedObject.ApplyModifiedProperties();
        RebuildEffectsList();
    }

    private void RemoveEffect(int index)
    {
        serializedObject.Update();
        Undo.RecordObject(target, "Remove Effect");

        _effectsProp.DeleteArrayElementAtIndex(index);
        if (index < _effectsProp.arraySize)
        {
            var p = _effectsProp.GetArrayElementAtIndex(index);
            if (p is { managedReferenceValue: null })
                _effectsProp.DeleteArrayElementAtIndex(index);
        }

        serializedObject.ApplyModifiedProperties();
        RebuildEffectsList();
    }

    private void MoveEffect(int index, int direction)
    {
        int newIndex = Mathf.Clamp(index + direction, 0, _effectsProp.arraySize - 1);
        if (newIndex == index) return;

        serializedObject.Update();
        Undo.RecordObject(target, "Reorder Effects");
        _effectsProp.MoveArrayElement(index, newIndex);
        serializedObject.ApplyModifiedProperties();
        RebuildEffectsList();
    }

    private void DuplicateEffect(int index)
    {
        serializedObject.Update();
        Undo.RecordObject(target, "Duplicate Effect");

        var src = _effectsProp.GetArrayElementAtIndex(index);
        var json = EditorJsonUtility.ToJson(src.managedReferenceValue, true);
        var type = src.managedReferenceValue?.GetType();
        if (type == null)
        {
            serializedObject.ApplyModifiedProperties();
            return;
        }

        _effectsProp.arraySize++;
        var newIndex = _effectsProp.arraySize - 1;
        var dst = _effectsProp.GetArrayElementAtIndex(newIndex);
        var newObj = Activator.CreateInstance(type);
        EditorJsonUtility.FromJsonOverwrite(json, newObj);
        dst.managedReferenceValue = newObj;
        dst.isExpanded = true;

        serializedObject.ApplyModifiedProperties();
        RebuildEffectsList();
    }

    private static List<Type> GetEffectTypes()
    {
        // Grab types that implement the closed generic interface <IEffectFactory<IDamageable>>
        var derived = TypeCache.GetTypesDerivedFrom<IEffectFactory<IDamageable>>();
        return derived.Where(t => !t.IsAbstract && t.IsClass)
            .OrderBy(t => t.Name)
            .ToList();
    }

    private static List<Type> GetTargetingTypes()
    {
        var derived = TypeCache.GetTypesDerivedFrom<TargetingStrategy>();
        return derived.Where(t => !t.IsAbstract && t.IsClass)
            .OrderBy(t => t.Name)
            .ToList();
    }

    private static string GetTypeDisplayName(SerializedProperty managedRefProp)
    {
        var full = managedRefProp.managedReferenceFullTypename;
        if (string.IsNullOrEmpty(full)) return "None";
        int space = full.IndexOf(' ');
        var typeName = space >= 0 ? full.Substring(space + 1) : full;
        int lastDot = typeName.LastIndexOf('.');
        if (lastDot >= 0) typeName = typeName[(lastDot + 1)..];
        return typeName;
    }
}
