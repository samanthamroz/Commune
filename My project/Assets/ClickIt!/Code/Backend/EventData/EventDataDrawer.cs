using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
namespace ClickIt.Editor {
    internal abstract class EventDataDrawer : PropertyDrawer {
        private const float Spacing = 2f;
        private const float Padding = 4f;
        private const float FieldWidth = 70f;

        protected void DrawGUI(Rect position, SerializedProperty property, GUIContent label, string interactionType) {
            EditorGUI.BeginProperty(position, label, property);

            float line = EditorGUIUtility.singleLineHeight;
            float y = position.y + Padding;

            // Background box
            Rect box = new Rect(position.x, position.y, position.width, GetPropertyHeight(property, label));
            GUI.Box(box, GUIContent.none, EditorStyles.helpBox);

            Rect content = new Rect(position.x + Padding, position.y + Padding, position.width - Padding * 2, position.height);

            // Optional label override
            SerializedProperty optionalLabel = property.FindPropertyRelative("optionalLabel");
            if (!string.IsNullOrEmpty(optionalLabel.stringValue)) {
                label.text = optionalLabel.stringValue;
            }
            else {
                label.text = $"{interactionType} Event";
            }

            // ───── Foldout Header inside the box ─────
            float arrowOffset = 12f; // adjust for arrow space
            string foldoutKey = GetFoldoutKey(property);
            bool expanded = SessionState.GetBool(foldoutKey, false);

            expanded = EditorGUI.Foldout(new Rect(content.x + arrowOffset, y, content.width - arrowOffset, line), expanded, label, true);

            SessionState.SetBool(foldoutKey, expanded);
            y += line + Spacing;

            if (expanded) {
                EditorGUI.indentLevel++;

                DrawEnabledBox(ref y, content, property);

                DrawOptionaLabelField(ref y, content, optionalLabel);

                DrawMouseButtonBoxes(ref y, content, property);

                DrawEvents(ref y, content, property, interactionType);

                DrawTimingFields(ref y, content, property);

                EditorGUI.indentLevel -= 2;
            }

            EditorGUI.EndProperty();
        }

        private void DrawOptionaLabelField(ref float y, Rect rect, SerializedProperty label) {
            float line = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(
                new Rect(rect.x, y, rect.width, line),
                label,
                new GUIContent("Label (Optional)")
            );
            y += line + Spacing;
        }
        private void DrawMouseButtonBoxes(ref float y, Rect rect, SerializedProperty prop) {
            float line = EditorGUIUtility.singleLineHeight;
            SerializedProperty buttons = prop.FindPropertyRelative("buttons");
            EditorGUI.LabelField(new Rect(rect.x, y, rect.width, line), "Mouse Buttons");
            y += line;
            EditorGUI.indentLevel++;
            int value = buttons.intValue;
            DrawMouseCheckbox(ref y, rect, "Left", MouseButton.left, ref value);
            DrawMouseCheckbox(ref y, rect, "Middle", MouseButton.middle, ref value);
            DrawMouseCheckbox(ref y, rect, "Right", MouseButton.right, ref value);
            buttons.intValue = value;
            EditorGUI.indentLevel--;
            y += Spacing;
        }
        private void DrawEnabledBox(ref float y, Rect rect, SerializedProperty prop) {
            SerializedProperty enabled = prop.FindPropertyRelative("enabled");
            bool value = enabled.boolValue;
            DrawCheckbox(ref y, rect, "Enabled", enabled, ref value);
            enabled.boolValue = value;
            y += Spacing;
        }
        private void DrawEvents(ref float y, Rect rect, SerializedProperty prop, string interactionType) {
            SerializedProperty evt = prop.FindPropertyRelative("evt");
            float evtHeight = EditorGUI.GetPropertyHeight(evt);
            EditorGUI.PropertyField(
                new Rect(rect.x, y, rect.width, evtHeight),
                evt,
                new GUIContent($"On {interactionType} Event")
            );
            y += evtHeight + Spacing;
        }
        private void DrawTimingFields(ref float y, Rect rect, SerializedProperty prop) {
            float line = EditorGUIUtility.singleLineHeight;

            EditorGUI.LabelField(new Rect(rect.x, y, rect.width, line), "Timing Parameters");
            y += line + Spacing;
            EditorGUI.indentLevel++;

            SerializedProperty delay = prop.FindPropertyRelative("delay");
            SerializedProperty cooldown = prop.FindPropertyRelative("cooldown");
            SerializedProperty buffer = prop.FindPropertyRelative("bufferTime");
            SerializedProperty timeout = prop.FindPropertyRelative("timeout");

            DrawRightAlignedPositiveFloat(ref y, rect, delay, "Delay (seconds)");
            DrawRightAlignedPositiveFloat(ref y, rect, timeout, "Timeout (seconds)");
            DrawRightAlignedPositiveFloat(ref y, rect, cooldown, "Cooldown (seconds)");
            if (cooldown.floatValue > 0f) {
                DrawRightAlignedPositiveFloat(ref y, rect, buffer, "Buffer (seconds)");
            }
        }

        private void DrawMouseCheckbox(ref float y, Rect rect, string label, MouseButton button, ref int value) {
            bool has = (value & (int)button) != 0;
            bool next = EditorGUI.ToggleLeft(
                new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                label,
                has
            );

            if (next != has) {
                if (next) value |= (int)button;
                else value &= ~(int)button;
            }

            y += EditorGUIUtility.singleLineHeight;
        }

        private void DrawCheckbox(ref float y, Rect rect, string label, SerializedProperty prop, ref bool value) {
            bool has = prop.boolValue;
            bool next = EditorGUI.ToggleLeft(
                new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                label,
                has
            );

            if (next != has) {
                value = next;
            }

            y += EditorGUIUtility.singleLineHeight;
        }

        private void DrawRightAlignedPositiveFloat(ref float y, Rect rect, SerializedProperty prop, string label) {
            float line = EditorGUIUtility.singleLineHeight;

            Rect fieldRect = new Rect(rect.xMax - FieldWidth, y, FieldWidth, line);

            // Label sized to text, but clamped so it never overlaps the field
            Vector2 labelSize = GUI.skin.label.CalcSize(new GUIContent(label));
            float maxLabelWidth = fieldRect.xMin - rect.x - Spacing;
            float labelWidth = Mathf.Min(labelSize.x, maxLabelWidth);

            Rect labelRect = new Rect(rect.x, y, labelWidth + ((FieldWidth / 2f) - Spacing), line);

            // Draw label (passive)
            GUI.Label(labelRect, label);

            EditorGUI.BeginChangeCheck();
            float value = EditorGUI.FloatField(fieldRect, prop.floatValue);
            if (EditorGUI.EndChangeCheck()) {
                prop.floatValue = Mathf.Max(0f, value);
            }

            y += line + Spacing;
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            float line = EditorGUIUtility.singleLineHeight;

            string foldoutKey = GetFoldoutKey(property);
            bool expanded = SessionState.GetBool(foldoutKey, false);

            if (!expanded) {
                return line + Padding * 2; // only header
            }

            // Expanded height calculation
            float h = Padding * 2;
            h += line + Spacing;      // header
            h += line + Spacing;      // label
            h += line * 4;            // mouse buttons
            h += line + Spacing;      // enabled
            h += Spacing;

            SerializedProperty evt = property.FindPropertyRelative("evt");
            h += EditorGUI.GetPropertyHeight(evt) + Spacing;

            h += line + Spacing;      // timing header
            h += line + Spacing;      // delay
            h += line + Spacing;  // timeout
            h += line + Spacing;      // cooldown

            SerializedProperty cooldown = property.FindPropertyRelative("cooldown");
            if (cooldown.floatValue > 0f) {
                h += line + Spacing;  // buffer
            }

            return h;
        }

        private string GetFoldoutKey(SerializedProperty property) {
            // Combine target object instance ID with property path for uniqueness
            return $"ClickIt_Foldout_{property.serializedObject.targetObject.GetInstanceID()}_{property.propertyPath}";
        }
    }

    [CustomPropertyDrawer(typeof(ClickEventData))]
    internal class ClickEventDataDrawer : EventDataDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            DrawGUI(position, property, label, "Click");
        }
    }

    [CustomPropertyDrawer(typeof(ReleaseEventData))]
    internal class ReleaseEventDataDrawer : EventDataDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            DrawGUI(position, property, label, "Release");
        }
    }

    [CustomPropertyDrawer(typeof(ClickAwayEventData))]
    internal class ClickAwayEventDataDrawer : EventDataDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            DrawGUI(position, property, label, "ClickAway");
        }
    }
}
#endif
