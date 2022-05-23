using UnityEditor;
using UnityEngine;

namespace SorceressSpell.LibrarIoh.Unity.Managers.Audio.Editor
{
    [CustomPropertyDrawer(typeof(AudioClipEntry))]
    public class AudioClipEntryDrawer : PropertyDrawer
    {
        #region Methods

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            float IdWidthSpacing = AudioClipEntryDrawerConstants.IdWidth + AudioClipEntryDrawerConstants.Spacing;

            Rect idRect = new Rect(position.x, position.y, AudioClipEntryDrawerConstants.IdWidth, position.height);
            Rect audioClipRect = new Rect(position.x + IdWidthSpacing, position.y, position.width - IdWidthSpacing, position.height);

            EditorGUI.PropertyField(idRect, property.FindPropertyRelative("HookName"), GUIContent.none);
            EditorGUI.PropertyField(audioClipRect, property.FindPropertyRelative("AudioClip"), GUIContent.none);

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        #endregion Methods
    }
}
