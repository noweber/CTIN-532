using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Analytics : MonoBehaviour
{
    [SerializeField] private string FormUrl = "https://docs.google.com/forms/d/e/1FAIpQLSf87TbtUGgXFu_xCDdBgColsfrZbFf2BAsZXFl-QQaws7NlpA/formResponse";

    public static Analytics Instance;

    public Analytics()
    {
        Instance = this;
    }

    public void SendAnalyticsData(string sessionID, int districtNumber)
    {
        StartCoroutine(
            PostAnalyticsData(sessionID,
            districtNumber.ToString()));
    }


    private IEnumerator PostAnalyticsData(string sessionID, string districtNumber)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.1379325124", sessionID);
        form.AddField("entry.589367142", districtNumber);
        return PostAnalyticsForm(form, FormUrl);
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

