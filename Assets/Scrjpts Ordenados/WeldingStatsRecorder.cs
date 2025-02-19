using System.Collections.Generic;
using UnityEngine;

public class WeldingStatsRecorder : MonoBehaviour
{
    public List<float> RecordedAngles = new List<float>();
    public List<float> RecordedArcLengths = new List<float>();
    public List<float> SpeedMeasurements = new List<float>();

    public void RecordStats(float angle, float arcLength, float speed)
    {
        RecordedAngles.Add(angle);
        RecordedArcLengths.Add(arcLength);
        SpeedMeasurements.Add(speed);
    }
}