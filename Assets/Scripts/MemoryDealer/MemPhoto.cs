using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class MemPhoto : MonoBehaviour
{
    [SerializeField] private string _photoCaption = "I was...";

    [SerializeField] private int _photoNumber = 0;

    [SerializeField] private Material _photoMaterial;
    [SerializeField] private MemDealerOpenAIManager _aiManager;
    
    private string _photoUrl;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    async void RenderImageData()
    {
        // Get corresponding memory data from list of memories
        PhotoData _memoryData = _aiManager._memoriesForStory[_photoNumber];
        _photoUrl = _memoryData.imageUrl;
        var _imageTexture = await GetRemoteTexture(_photoUrl);
        _photoMaterial.mainTexture = _imageTexture;
        _photoCaption = _memoryData.caption;
    }

    public static async Task<Texture2D> GetRemoteTexture ( string url )
    {
        using( UnityWebRequest www = UnityWebRequestTexture.GetTexture(url) )
        {
            var asyncOp = www.SendWebRequest();

            while( asyncOp.isDone==false )
                await Task.Delay( 1000/30 );//30 hertz
        
            if( www.isNetworkError || www.isHttpError )
            {
                Debug.Log( $"{www.error}, URL:{www.url}" );
                return null;
            }
            else
            {
                return DownloadHandlerTexture.GetContent(www);
            }
        }
    }
}
