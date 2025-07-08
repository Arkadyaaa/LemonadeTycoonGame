using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

public class SkinDownloader : MonoBehaviour
{
    public void DownloadSkinsFromUUIDs(List<string> uuids)
    {
        StartCoroutine(DownloadAllSkins(uuids));
    }

    IEnumerator DownloadAllSkins(List<string> uuids)
    {
        foreach (string uuid in uuids)
        {
            yield return StartCoroutine(GetSkinFromUUID(uuid));
        }

        Debug.Log("✅ All skins downloaded.");
    }

    IEnumerator GetSkinFromUUID(string uuid)
    {
        string url = $"https://sessionserver.mojang.com/session/minecraft/profile/{uuid}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"❌ Failed to get skin for UUID {uuid}: {request.error}");
            yield break;
        }

        string json = request.downloadHandler.text;
        SessionProfile profile = JsonUtility.FromJson<SessionProfile>(json);

        if (profile == null || profile.properties == null || profile.properties.Length == 0)
        {
            Debug.LogWarning($"⚠ No properties for UUID {uuid}");
            yield break;
        }

        string base64 = profile.properties[0].value;
        string decodedJson = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
        TextureData textureData = JsonUtility.FromJson<TextureData>(decodedJson);

        if (textureData.textures?.SKIN?.url == null)
        {
            Debug.LogWarning($"⚠ No skin URL for UUID {uuid}");
            yield break;
        }

        string skinUrl = textureData.textures.SKIN.url;
        string model = textureData.textures.SKIN.metadata?.model ?? "steve";
        string username = textureData.profileName;

        // Download skin texture
        UnityWebRequest textureRequest = UnityWebRequestTexture.GetTexture(skinUrl);
        yield return textureRequest.SendWebRequest();

        if (textureRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"❌ Failed to download skin from {skinUrl}: {textureRequest.error}");
            yield break;
        }

        Texture2D skinTexture = DownloadHandlerTexture.GetContent(textureRequest);
        byte[] pngData = skinTexture.EncodeToPNG();

        if (pngData == null)
        {
            Debug.LogError($"❌ Failed to encode skin for {username}");
            yield break;
        }

        // Save the file
        string folder = model == "slim" ? "Alex" : "Steve";
        string savePath = Path.Combine(Application.persistentDataPath, "Skins", folder);
        Directory.CreateDirectory(savePath);

        string filePath = Path.Combine(savePath, $"{username}.png");
        File.WriteAllBytes(filePath, pngData);

        Debug.Log($"✅ Saved skin for {username} to {filePath}");
    }

    public void DownloadSkinForUsername(string username, System.Action<Texture2D> onSkinReady)
    {
        StartCoroutine(DownloadSkinCoroutine(username, onSkinReady));
    }

    private IEnumerator DownloadSkinCoroutine(string username, System.Action<Texture2D> onSkinReady)
    {
        string url = "https://api.minecraftservices.com/minecraft/profile/lookup/bulk/byname";
        string jsonPayload = UUIDManager.JsonArrayHelper.ToJson(new string[] { username });

        UnityWebRequest uuidRequest = new UnityWebRequest(url, "POST");
        uuidRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonPayload));
        uuidRequest.downloadHandler = new DownloadHandlerBuffer();
        uuidRequest.SetRequestHeader("Content-Type", "application/json");

        yield return uuidRequest.SendWebRequest();

        if (uuidRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"❌ UUID request failed: {uuidRequest.error}");
            yield break;
        }

        var profiles = UUIDManager.JsonArrayHelper.FromJson<UUIDManager.MinecraftProfile>(uuidRequest.downloadHandler.text);
        if (profiles.Length == 0)
        {
            Debug.LogWarning("⚠ No UUID found.");
            yield break;
        }

        string uuid = profiles[0].id;

        string profileUrl = $"https://sessionserver.mojang.com/session/minecraft/profile/{uuid}";
        UnityWebRequest profileRequest = UnityWebRequest.Get(profileUrl);
        yield return profileRequest.SendWebRequest();

        if (profileRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"❌ Profile fetch failed: {profileRequest.error}");
            yield break;
        }

        string json = profileRequest.downloadHandler.text;
        var sessionProfile = JsonUtility.FromJson<SessionProfile>(json);
        string base64 = sessionProfile.properties[0].value;
        string decodedJson = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(base64));
        var textureData = JsonUtility.FromJson<TextureData>(decodedJson);

        string skinUrl = textureData.textures.SKIN.url;
        UnityWebRequest textureRequest = UnityWebRequestTexture.GetTexture(skinUrl);
        yield return textureRequest.SendWebRequest();

        if (textureRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"❌ Skin download failed: {textureRequest.error}");
            yield break;
        }

        Texture2D skinTexture = DownloadHandlerTexture.GetContent(textureRequest);
        onSkinReady?.Invoke(skinTexture);
    }

    // === JSON Structures ===
    [Serializable]
    public class SessionProfile
    {
        public string id;
        public string name;
        public Property[] properties;
    }

    [Serializable]
    public class Property
    {
        public string name;
        public string value;
    }

    [Serializable]
    public class TextureData
    {
        public string profileName;
        public Textures textures;
    }

    [Serializable]
    public class Textures
    {
        public Skin SKIN;
    }

    [Serializable]
    public class Skin
    {
        public string url;
        public Metadata metadata;
    }

    [Serializable]
    public class Metadata
    {
        public string model;
    }
}
