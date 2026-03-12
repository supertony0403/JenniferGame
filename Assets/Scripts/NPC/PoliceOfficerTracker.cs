using System.Collections.Generic;
using UnityEngine;

public class PoliceOfficerTracker : MonoBehaviour
{
    public static PoliceOfficerTracker Instance { get; private set; }

    private readonly HashSet<int> _officersInSight = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void ReportSight(int officerId, bool inSight)
    {
        bool wasSeen = _officersInSight.Count > 0;
        if (inSight) _officersInSight.Add(officerId);
        else _officersInSight.Remove(officerId);
        bool isSeen = _officersInSight.Count > 0;
        if (wasSeen != isSeen) WutMeter.Instance?.SetPoliceInSight(isSeen);
    }
}
