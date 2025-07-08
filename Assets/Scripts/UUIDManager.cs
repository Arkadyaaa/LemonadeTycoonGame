using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class UUIDManager : MonoBehaviour
{
    [Header("Text file path (inside StreamingAssets)")]
    public string fileName = "skin_list.txt";

    [Header("Event triggered when UUIDs are fetched")]
    public UUIDEvent OnUUIDsFetched = new UUIDEvent();

    private List<string> usernames = new List<string>();

    public void StartFetching()
    {
        LoadUsernamesFromFile();
        OnUUIDsFetched.AddListener(FindObjectOfType<SkinDownloader>().DownloadSkinsFromUUIDs);
        StartCoroutine(FetchAllUUIDs());
    }

    void LoadUsernamesFromFile()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"Could not find file: {filePath}");
            return;
        }

        usernames.Clear();
        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            if (!string.IsNullOrEmpty(trimmed))
                usernames.Add(trimmed);
        }

        Debug.Log($"Loaded {usernames.Count} usernames from {filePath}");
    }

    IEnumerator FetchAllUUIDs()
    {
        List<MinecraftProfile> allProfiles = new List<MinecraftProfile>();
        int total = usernames.Count;
        int batchSize = 10;

        for (int i = 0; i < total; i += batchSize)
        {
            List<string> batch = usernames.GetRange(i, Mathf.Min(batchSize, total - i));
            yield return StartCoroutine(GetUUIDBatch(batch, allProfiles));
        }

        Debug.Log($"Fetched {allProfiles.Count} profiles:");
        foreach (var profile in allProfiles)
        {
            Debug.Log($"Name: {profile.name}, ID: {profile.id}");
        }   

        // Send the UUIDs to skin downloader
        List<string> uuidList = allProfiles.ConvertAll(p => p.id);
        OnUUIDsFetched.Invoke(uuidList);
    }

    IEnumerator GetUUIDBatch(List<string> batch, List<MinecraftProfile> resultList)
    {
        string url = "https://api.minecraftservices.com/minecraft/profile/lookup/bulk/byname";
        string jsonPayload = JsonArrayHelper.ToJson(batch.ToArray());

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Batch failed: {request.error}");
        }
        else
        {
            string json = request.downloadHandler.text;
            MinecraftProfile[] profiles = JsonArrayHelper.FromJson<MinecraftProfile>(json);
            resultList.AddRange(profiles);
        }
    }

    [System.Serializable]
    public class MinecraftProfile
    {
        public string id;
        public string name;
    }

    [System.Serializable]
    public class UUIDEvent : UnityEvent<List<string>> { }

    public static class JsonArrayHelper
    {
        public static T[] FromJson<T>(string json)
        {
            string wrapped = "{\"array\":" + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrapped);
            return wrapper.array;
        }

        public static string ToJson<T>(T[] array)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < array.Length; i++)
            {
                sb.Append($"\"{array[i]}\"");
                if (i < array.Length - 1)
                    sb.Append(",");
            }
            sb.Append("]");
            return sb.ToString();
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }
    }
}
