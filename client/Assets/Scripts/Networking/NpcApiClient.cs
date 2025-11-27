using System;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class NpcApiClient : MonoBehaviour
{
    [Header("Backend config")]
    public string baseUrl;

    public IEnumerator CallDialogue(
        DialogueRequest req,
        Action<DialogueResponse> onSuccess,
        Action<string> onError = null
    )
    {
        string json = JsonUtility.ToJson(req);
        string url = $"{baseUrl}/dialogue";

        using (var request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                string err = $"Dialogue request failed: {request.error} - {request.downloadHandler.text}";
                Debug.LogError(err);
                onError?.Invoke(err);
                yield break;
            }

            string respText = request.downloadHandler.text;
            Debug.Log($"Raw dialogue response: {respText}");

            DialogueResponse resp = JsonUtility.FromJson<DialogueResponse>(respText);
            if (resp == null)
            {
                string err = "Failed to parse dialogue response";
                Debug.LogError(err);
                onError?.Invoke(err);
                yield break;
            }

            onSuccess?.Invoke(resp);
        }
    }
}
