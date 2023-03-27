using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using USCG.Core.Telemetry;

public class DistrictMetricsTelemetryManager : Singleton<DistrictMetricsTelemetryManager>
{
    public int CurrentDistrict = -1;

    // TODO: Remove this reference and refactor
    public TextMeshProUGUI DistrictNumber;

    // Keep a reference to the metrics we create.
    private MetricId districtNumberStarted = default;
    private MetricId districtNumberLoss = default;

    private void Start()
    {
        districtNumberStarted = TelemetryManager.instance.CreateSampledMetric<int>("districtNumberStarted");
        districtNumberLoss = TelemetryManager.instance.CreateSampledMetric<int>("districtNumberLoss");
    }

    public void StartNextDistrict()
    {
        CurrentDistrict++;
        TelemetryManager.instance.AddMetricSample(districtNumberStarted, CurrentDistrict);
    }

    public void LossAtDistrict()
    {
        TelemetryManager.instance.AddMetricSample(districtNumberLoss, CurrentDistrict);
    }

    public void ResetDistrict()
    {
        CurrentDistrict = -1;
    }

    private void FixedUpdate()
    {
        if (!string.Equals(DistrictNumber.text, CurrentDistrict.ToString()))
        {
            DistrictNumber.text = CurrentDistrict.ToString();
        }
    }
}