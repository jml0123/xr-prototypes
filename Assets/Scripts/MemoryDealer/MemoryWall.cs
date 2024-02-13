using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MemoryWall : MonoBehaviour
{
    [SerializeField] private TMP_Text _memoryWallText;

    [SerializeField] private MemDealerOpenAIManager _aiManager;

    private bool _storyReady = false;

    private bool _storyRendered = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_storyRendered) return;
        if (_storyReady == false)
        {
            _storyReady = _aiManager.CheckIfStoryIsReady();
        }
        else
        {
            RenderStory();
        }
    }

    void RenderStory()
    {
        _memoryWallText.text = _aiManager.generatedStoryString + "...";
        Debug.Log(_aiManager.generatedStoryString);
        _storyRendered = true;
    }
}
