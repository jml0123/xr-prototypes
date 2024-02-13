using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryCollider : MonoBehaviour
{
    // Start is called before the first frame update
    private bool _memoriesPopulated = false;
    private int _expectedNumMemories = 0;
    private List<GameObject> _registeredMemories = new List<GameObject>();
    private bool _requestedFullStory = false;
    [SerializeField] private MemDealerOpenAIManager _aiManager;
    void Start()
    {
        _expectedNumMemories = _aiManager.numMemories;
    }

    // Update is called once per frame
    void Update()
    {
        if (_requestedFullStory) return;
        if (_registeredMemories.Count == _expectedNumMemories)
        {
            _memoriesPopulated = true;
            Debug.Log("Memories Full. Getting Full Story");
            GetFullStory();
            _requestedFullStory = true;
        }
    }

    private void GetFullStory()
    {
        _aiManager.GetFullStoryUsingCaptions();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision Detected");
        if (other.TryGetComponent<MemoryInteractor>(out MemoryInteractor interactor))
        {
            Debug.Log("MEMORY DETECTED");
            if (!_registeredMemories.Contains(interactor.gameObject))
            {
                _registeredMemories.Add(interactor.gameObject);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        var radius = GetComponent<SphereCollider>().radius;
        Gizmos.DrawWireSphere(transform.position, radius * 4f);
    }
}
