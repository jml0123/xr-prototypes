using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.Interaction.Toolkit;

public class MemPhoto : MonoBehaviour
{
    [SerializeField] public string _photoCaption = "I was waiting on the beach, staring at the sunrise";

    [SerializeField] private int _photoNumber = 1;

    [SerializeField] private Material _photoMaterial;
    [SerializeField] private MemDealerOpenAIManager _aiManager;
    [SerializeField] private TMP_Text _captionArea;
    [SerializeField] private TMP_Text _headerArea;
    private string _photoUrl;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    async public void GetMemoryData()
    {
        PhotoData imageData = await _aiManager.AddToCaptionsAndGenerateImage(_photoCaption);
        RenderImageData(imageData);
    }

    async void RenderImageData(PhotoData data)
    {
        // Get corresponding memory data from list of memories
        PhotoData _memoryData = data;
        _photoUrl = _memoryData.imageUrl;
        Debug.Log(_photoUrl);
        Debug.Log(_memoryData.caption);
        var _imageTexture = await GetRemoteTexture(_photoUrl);
        _photoMaterial.mainTexture = _imageTexture;
        _photoCaption = _memoryData.caption;
        _captionArea.text = _photoCaption + "...";
        _headerArea.text = _photoNumber.ToString();
    }

    public async Task<Texture> GetRemoteTexture ( string url )
    {
        using(var request = new UnityWebRequest(url))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Access-Control-Allow-Origin", "*");
            request.SendWebRequest();

            while (!request.isDone) await Task.Yield();

            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(request.downloadHandler.data);
            var sprite = Sprite.Create(texture, new Rect(0, 0, 256, 256), Vector2.zero, 1f);
            return sprite.texture;
        }
    }
}
