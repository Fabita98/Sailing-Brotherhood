using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyTransitionMusic : MonoBehaviour
{
    public AudioSource audio;
    private float duration;
    private float target;
    [SerializeField] private GameObject StartingTrans;
    // Start is called before the first frame update
    void Start()
    {
        audio.Play();
        StartingTrans.SetActive(true);
        target = 0.7f;
        duration = 3f;
        StartCoroutine(FadeAudioSource.StartFade(audio, duration, target));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
