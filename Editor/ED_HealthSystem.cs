using UnityEditor;
using StdNounou.Core.Editor;

namespace StdNounou.Health.Editor
{
    [CustomEditor(typeof(HealthSystem))]
    public class ED_HealthSystem : UnityEditor.Editor
    {
        private HealthSystem targetScript;

        private SerializedProperty ED_debugMode;

        private void OnEnable()
        {
            targetScript = (HealthSystem)target;

            ED_debugMode = serializedObject.FindProperty(nameof(ED_debugMode));
        }

        public override void OnInspectorGUI()
        {
            ReadOnlyDraws.EditorScriptDraw(typeof(ED_HealthSystem), this);
            base.DrawDefaultInspector();

            if (!ED_debugMode.boolValue) return;
            DisplayHP();
            DisplayTickDamages();

            serializedObject.ApplyModifiedProperties();
        }

        private void DisplayHP()
        {
            SimpleDraws.HorizontalLine();
            EditorGUILayout.LabelField("Current Health :", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"{targetScript.CurrentHealth} / {targetScript.CurrentMaxHealth}");
        }

        private void DisplayTickDamages()
        {
            SimpleDraws.HorizontalLine();
            EditorGUILayout.LabelField("Unique Ticks :", EditorStyles.boldLabel);

            if (targetScript.UniqueTickDamages != null && targetScript.UniqueTickDamages.Count > 0)
            {
                using (var uniqueTicks = new EditorGUILayout.VerticalScope("GroupBox"))
                {
                    foreach (var item in targetScript.UniqueTickDamages)
                    {
                        EditorGUILayout.LabelField(item.Value.Data.ID);
                    }
                }
            }

            EditorGUILayout.LabelField("Stackable Ticks :", EditorStyles.boldLabel);
            if (targetScript.StackableTickDamages != null && targetScript.StackableTickDamages.Count > 0)
            {
                using (var uniqueTicks = new EditorGUILayout.VerticalScope("GroupBox"))
                {
                    foreach (var item in targetScript.StackableTickDamages)
                    {
                        if (item.Value.Count == 0) continue;
                        EditorGUILayout.LabelField($"{item.Value[0].Data.ID}x{item.Value.Count}");
                    }
                }
            }

        }
    } 
}