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

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(request.error);
        }
        else
        {
            var resp = JsonUtility.FromJson<TResp>(request.downloadHandler.text);
            onSuccess(resp);
        }
    }
}
