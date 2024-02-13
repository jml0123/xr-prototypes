using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float _lerpSpeed = 0.4f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.rotation = Quaternion.Lerp(
            gameObject.transform.rotation,
            Camera.main.transform.rotation,
            Time.deltaTime * _lerpSpeed);
        gameObject.transform.position = Vector3.Lerp(
            gameObject.transform.position,
            Camera.main.transform.position,
            Time.deltaTime * _lerpSpeed);
    }
}
