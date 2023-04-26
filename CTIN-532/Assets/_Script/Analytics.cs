using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Analytics : MonoBehaviour
{
    private string districtAnalyticsFormUrl = "https://docs.google.com/forms/d/e/1FAIpQLSebIN4Ar6_UmES-rdJAYE1zjNuoApQSxyFkZczH1mVYTIczcg/formResponse";

    private string unitAnalyticsFormUrl = "https://docs.google.com/forms/d/e/1FAIpQLSdAJTULFPQTS3NR21Kyu6x8lpCSwlaakdoBjMqZztTlkwIacA/formResponse";

    public static Analytics Instance;

    [SerializeField] private string SessionId;

    public Analytics()
    {
        Instance = this;
        if (SessionId == null)
        {
            SessionId = Guid.NewGuid().ToString();
        }
    }

    public void SendDistrictAnalytics(int districtNumber)
    {
        StartCoroutine(
            PostDistrictAnalyticsData(SessionId,
            districtNumber.ToString()));
    }


    public void SendUnitAnalytics(string unitSpawned)
    {
        StartCoroutine(
            PostUnitAnalyticsData(SessionId,
            unitSpawned));
    }

    private IEnumerator PostDistrictAnalyticsData(string sessionID, string districtNumber)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.1379325124", sessionID);
        form.AddField("entry.589367142", districtNumber);
        return PostAnalyticsForm(form, districtAnalyticsFormUrl);
    }
    private IEnumerator PostUnitAnalyticsData(string sessionID, string unitSpawned)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.1379325124", sessionID);
        form.AddField("entry.589367142", unitSpawned);
        return PostAnalyticsForm(form, unitAnalyticsFormUrl);
    }


    private IEnumerator PostAnalyticsForm(WWWForm analyticsForm, string formUrl)
    {
        if (analyticsForm == null)
        {
            throw new ArgumentNullException(nameof(analyticsForm));
        }

        using (UnityWebRequest www = UnityWebRequest.Post(formUrl, analyticsForm))
        {

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete.");
            }

        }
    }
}

