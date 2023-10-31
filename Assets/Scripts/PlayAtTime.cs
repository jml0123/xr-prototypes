using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAtTime : MonoBehaviour
{
    [SerializeField] private AudioSource _audio;

    [SerializeField] private float _time;
    // Start is called before the first frame update
    void Start()
    {
        _audio.time = _time;
        _audio.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
