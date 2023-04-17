using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Analytics : MonoBehaviour
{
    [SerializeField] private string DistrictAnalyticsFormUrl = "https://docs.google.com/forms/d/e/1FAIpQLSf87TbtUGgXFu_xCDdBgColsfrZbFf2BAsZXFl-QQaws7NlpA/formResponse";

    [SerializeField] private string UnitAnalyticsFormUrl = "https://docs.google.com/forms/d/e/1FAIpQLSeeWml880g5GFBPQkPahLM7-g0FCtqdnpy09iSb0nN3NhMWWg/formResponse";

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
        return PostAnalyticsForm(form, DistrictAnalyticsFormUrl);
    }
    private IEnumerator PostUnitAnalyticsData(string sessionID, string unitSpawned)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.1379325124", sessionID);
        form.AddField("entry.589367142", unitSpawned);
        return PostAnalyticsForm(form, UnitAnalyticsFormUrl);
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

