using USCG.Core.Telemetry;

public class DistrictMetricsTelemetryManager : Singleton<DistrictMetricsTelemetryManager>
{
    // Keep a reference to the metrics we create.
    private MetricId districtNumberStarted = default;
    private MetricId districtNumberLoss = default;

    private GameManager gameManager;

    private void Start()
    {
        districtNumberStarted = TelemetryManager.instance.CreateSampledMetric<int>("districtNumberStarted");
        districtNumberLoss = TelemetryManager.instance.CreateSampledMetric<int>("districtNumberLoss");
    }

    public void TrackDistrictStartMetric()
    {
        TelemetryManager.instance.AddMetricSample(districtNumberStarted, gameManager.NumberOfDistrictLevelsCleared);
    }

    public void TrackDistrictLossMetric()
    {
        TelemetryManager.instance.AddMetricSample(districtNumberLoss, gameManager.NumberOfDistrictLevelsCleared);
    }
}