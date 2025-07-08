using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using UnityEngine.Networking;
using TMPro;


#if UNITY_EDITOR
using UnityEditor; // Only include in editor
#endif

public class SkinChanger : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField usernameInputField;
    public Button downloadSkinButton;
    public Button uploadSkinButton;

    [Header("Target Model Renderers")]
    public Renderer[] targetRenderers;

    [Header("Panels")]
    public GameObject skinChangerPanel;
    public GameObject successPanel;
    public GameObject failPanel;
    
    public TMP_Text successText;
    public TMP_Text failText;


    private void Start()
    {
        downloadSkinButton.onClick.AddListener(HandleDownloadSkin);
        uploadSkinButton.onClick.AddListener(HandleUploadSkin);
    }

    void HandleDownloadSkin()
    {
        string username = usernameInputField.text.Trim();
        if (string.IsNullOrEmpty(username))
        {
            Debug.LogWarning("⚠ Username input is empty!");
            return;
        }

        var skinDownloader = FindObjectOfType<SkinDownloader>();
        skinDownloader.DownloadSkinForUsername(username, (texture) =>
        {
            ApplySkinToCharacter(texture);

            // Save the texture to a file
            string savePath = Path.Combine(Application.persistentDataPath, "player_skin.png");
            byte[] pngData = texture.EncodeToPNG();
            File.WriteAllBytes(savePath, pngData);
        });
    }

    void HandleUploadSkin()
    {
        string path = OpenFilePanel("Select Skin PNG", "", "png");
        if (!string.IsNullOrEmpty(path))
        {
            StartCoroutine(LoadAndApplySkinFromFile(path));
        }
    }

    IEnumerator LoadAndApplySkinFromFile(string filePath)
    {
        using UnityWebRequest request = UnityWebRequestTexture.GetTexture("file://" + filePath);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            skinChangerPanel.SetActive(false);
            failPanel.SetActive(true);
            failText.text = "Failed to load texture from file: " + request.error;
            yield break;
        }

        Texture2D texture = DownloadHandlerTexture.GetContent(request);

        //make sure the skin is 64x
        if (texture.width != 64 || texture.height != 64)
        {
            skinChangerPanel.SetActive(false);
            failPanel.SetActive(true);

            failText.text = $"Invalid skin size: {texture.width}x{texture.height}. Expected 64x64.";
            yield break;
        }

        ApplySkinToCharacter(texture);

        // Save uploaded texture to persistent data path
        string savePath = Path.Combine(Application.persistentDataPath, "player_skin.png");
        byte[] pngData = texture.EncodeToPNG();
        File.WriteAllBytes(savePath, pngData);
    }

    void ApplySkinToCharacter(Texture2D texture)
    {
        if (targetRenderers != null && targetRenderers.Length > 0)
        {
            foreach (var renderer in targetRenderers)
            {
                if (renderer != null)
                {
                    Material mat = new Material(renderer.sharedMaterial);
                    mat.mainTexture = texture;
                    renderer.material = mat;
                }
            }

            skinChangerPanel.SetActive(false);
            successPanel.SetActive(true);
            successText.text = "Skin successfully loaded";
        }
    }

#if UNITY_EDITOR
    string OpenFilePanel(string title, string directory, string extension)
    {
        return UnityEditor.EditorUtility.OpenFilePanel(title, directory, extension);
    }
#else
    string OpenFilePanel(string title, string directory, string extension)
    {
        Debug.LogError("📁 File picker not implemented for this platform.");
        return null;
    }
#endif
}