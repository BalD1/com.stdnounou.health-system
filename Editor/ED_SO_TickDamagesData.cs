using UnityEditor;
using StdNounou.Core.Editor;
using StdNounou.Health;

[CustomEditor(typeof(SO_TickDamagesData))]
public class ED_SO_TickDamagesData : Editor
{
	private SO_TickDamagesData targetScript;

    private float simulationDamages;
    private float simulationCritChances;
    private float simulationCritMultiplier;

    private bool runSimulation;

    private void OnEnable()
    {
        targetScript = (SO_TickDamagesData)target;
    }
    
    public override void OnInspectorGUI()
    {
        ReadOnlyDraws.EditorScriptDraw(typeof(ED_SO_TickDamagesData), this);
        base.DrawDefaultInspector();

        runSimulation = EditorGUILayout.Toggle("Run Simulation", runSimulation);
        if (runSimulation) DamagesSimulation();

        serializedObject.ApplyModifiedProperties();
    }

    private void DamagesSimulation()
    {
        SimpleDraws.HorizontalLine();
        EditorGUILayout.LabelField("Simulation", EditorStyles.boldLabel);

        using (var ownerStatsV = new EditorGUILayout.VerticalScope("GroupBox"))
        {
            simulationDamages = EditorGUILayout.FloatField("Simulation Damages", simulationDamages);
            using (var critH = new EditorGUILayout.HorizontalScope())
            {
                simulationCritChances = EditorGUILayout.FloatField("Simulation Crit Chances", simulationCritChances);
                simulationCritMultiplier = EditorGUILayout.FloatField("Simulation Crit Multiplier", simulationCritMultiplier);
            }
        }

        float resultsDamages = GetResultValue(targetScript.Damages, simulationDamages);
        float resultsCritChances = GetResultValue(targetScript.CritChances, simulationCritChances);
        float resultsCritMultiplier = GetResultValue(targetScript.CritMultiplier, simulationCritMultiplier);
        int totalActivations = targetScript.TicksLifetime / targetScript.RequiredTicksToTrigger + 1;

        using (var simulationResults = new  EditorGUILayout.VerticalScope("GroupBox"))
        {
            if (resultsCritChances <= 0 )
            {
                EditorGUILayout.LabelField("Damages per ticks : " + resultsDamages);
                EditorGUILayout.LabelField("Total activations : " + totalActivations);
                EditorGUILayout.LabelField("Total damages : " + resultsDamages * totalActivations);
                EditorGUILayout.LabelField("DPS : " + resultsDamages / (targetScript.RequiredTicksToTrigger * .25f));
            }
            else
            {
                float lowestDPT = resultsDamages;
                float highestDPT = resultsDamages * resultsCritMultiplier;

                float lowestTD = lowestDPT * totalActivations;
                float highestTD = highestDPT * totalActivations;

                float lowestDPS = lowestDPT / (targetScript.RequiredTicksToTrigger * .25f);
                float highestDPS = highestDPT / (targetScript.RequiredTicksToTrigger * .25f);

                EditorGUILayout.LabelField("Total activations : " + totalActivations);
                using (var lowestResults = new EditorGUILayout.VerticalScope("GroupBox"))
                {
                    EditorGUILayout.LabelField("Lowest Results", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("Damages per ticks : " + lowestDPT);
                    EditorGUILayout.LabelField("Total damages : " + lowestTD);
                    EditorGUILayout.LabelField("DPS : " + lowestDPS);
                }
                using (var highestResults = new EditorGUILayout.VerticalScope("GroupBox"))
                {
                    EditorGUILayout.LabelField("Highest Results", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("Damages per ticks : " + highestDPT);
                    EditorGUILayout.LabelField("Total damages : " + highestTD);
                    EditorGUILayout.LabelField("DPS : " + highestDPS);
                }
            }
        }
    }

    private float GetResultValue(SO_TickDamagesData.S_TickStat stat, float simulationValue)
    {
        switch (stat.StatType)
        {
            case SO_TickDamagesData.E_TickStatType.Fixed:
                simulationValue = stat.Value;
                break;

            case SO_TickDamagesData.E_TickStatType.Additive:
                simulationValue += stat.Value;
                break;

            case SO_TickDamagesData.E_TickStatType.Multiplier:
                simulationValue *= stat.Value;
                break;
        }
        return simulationValue;
    }
}