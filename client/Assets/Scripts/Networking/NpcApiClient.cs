using System;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


public class NpcApiClient : MonoBehaviour
{
    [SerializeField] string baseUrl;

    public IEnumerator CallDialogue(DialogueRequest req, Action<DialogueResponse> onSuccess, Action<string> onError)
    {
        yield return Send("dialogue", req, onSuccess, onError);
    }

    public IEnumerator CallEvaluation(EvaluateRequest req, Action<EvaluateResponse> onSuccess, Action<string> onError)
    {
        yield return Send("evaluate", req, onSuccess, onError);
    }

    private IEnumerator Send<TReq, TResp>(string endpoint, TReq body, Action<TResp> onSuccess, Action<string> onError)
    {
        var url = $"{baseUrl}/{endpoint}";
        var json = JsonUtility.ToJson(body);
        Debug.Log($"Sending request to {url} with body: {json}");
        var request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        // Always log response body and code to help debugging 400/500 responses
        var respText = request.downloadHandler != null ? request.downloadHandler.text : string.Empty;
        Debug.Log($"Response code: {request.responseCode}; text: {respText}");

        if (request.result != UnityWebRequest.Result.Success)
        {
            var message = $"HTTP {(long)request.responseCode} {request.error}. Server response: {respText}";
            onError?.Invoke(message);
        }
        else
        {
            try
            {
                var resp = JsonUtility.FromJson<TResp>(respText);
                onSuccess(resp);
            }
            catch (Exception ex)
            {
                var message = $"Failed to parse response JSON: {ex.Message}. Raw body: {respText}";
                Debug.LogError(message);
                onError?.Invoke(message);
            }
        }
    }
}
