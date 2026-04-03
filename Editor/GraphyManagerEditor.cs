/* ---------------------------------------
 * Author:          Martin Pane (martintayx@gmail.com) (@tayx94)
 * Contributors:    https://github.com/Tayx94/graphy/graphs/contributors
 * Project:         Graphy - Ultimate Stats Monitor
 * Date:            02-Apr-26
 * Studio:          Tayx
 *
 * Git repo:        https://github.com/Tayx94/graphy
 *
 * This project is released under the MIT license.
 * Attribution is not required, but it is always welcomed!
 * -------------------------------------*/

using UnityEditor;
using UnityEngine;

namespace Tayx.Graphy
{
    [CustomEditor(typeof(GraphyManager))]
    internal class GraphyManagerEditor : Editor
    {
        private SerializedProperty m_graphyMode;
        private SerializedProperty m_enableOnStartup;
        private SerializedProperty m_keepAlive;
        private SerializedProperty m_background;
        private SerializedProperty m_backgroundColor;

        private SerializedProperty m_togglePresetAction;
        private SerializedProperty m_toggleActiveAction;

        private SerializedProperty m_modulePresets;
        private SerializedProperty m_activePresetIndex;

        private void OnEnable()
        {
            m_graphyMode = serializedObject.FindProperty("m_graphyMode");
            m_enableOnStartup = serializedObject.FindProperty("m_enableOnStartup");
            m_keepAlive = serializedObject.FindProperty("m_keepAlive");
            m_background = serializedObject.FindProperty("m_background");
            m_backgroundColor = serializedObject.FindProperty("m_backgroundColor");

            m_togglePresetAction = serializedObject.FindProperty("TogglePresetAction");
            m_toggleActiveAction = serializedObject.FindProperty("ToggleActiveAction");

            m_modulePresets = serializedObject.FindProperty("m_modulePresets");
            m_activePresetIndex = serializedObject.FindProperty("m_activePresetIndex");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawGraphyHeader();

            EditorGUILayout.PropertyField(m_graphyMode);
            EditorGUILayout.PropertyField(m_enableOnStartup);
            EditorGUILayout.PropertyField(m_keepAlive);
            EditorGUILayout.PropertyField(m_background);

            using (new EditorGUI.DisabledScope(!m_background.boolValue))
            {
                EditorGUILayout.PropertyField(m_backgroundColor);
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_togglePresetAction);
            EditorGUILayout.PropertyField(m_toggleActiveAction);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_activePresetIndex);
            EditorGUILayout.PropertyField(m_modulePresets, true);

            serializedObject.ApplyModifiedProperties();
        }

        private static void DrawGraphyHeader()
        {
            GUILayout.Space(20f);

            if (GraphyEditorStyle.ManagerLogoTexture != null)
            {
                GUILayout.Label(
                    GraphyEditorStyle.ManagerLogoTexture,
                    new GUIStyle(GUI.skin.label)
                    {
                        alignment = TextAnchor.UpperCenter,
                    });

                GUILayout.Space(10f);
                return;
            }

            EditorGUILayout.LabelField("[ GRAPHY - MANAGER ]", GraphyEditorStyle.HeaderStyle1);
            GUILayout.Space(10f);
        }
    }
}
