using StdNounou.Stats;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class DummyAttributesDisplayer : MonoBehaviour
{
    [SerializeField] private MonoStatsHandler dummyStats;
    [SerializeField] private TextMeshPro tmp;
    private StatsHandler dummyStatsHandler;

    private StringBuilder sb = new StringBuilder();

    private void Awake()
    {
        dummyStatsHandler = dummyStats.StatsHandler;
    }

    private void Update()
    {
        // debug purpose only
        if (dummyStatsHandler == null) return;
        if (dummyStatsHandler.BaseStats == null) return;

        sb.Clear();
        sb.AppendLine("Attributes:");
        foreach (var item in dummyStatsHandler.BaseStats.Attributes)
            sb.AppendLine(item.ID);
        tmp?.SetText(sb.ToString());
    }
}
