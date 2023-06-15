using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyTransitionMusic : MonoBehaviour
{
    public AudioSource audio1, audio2;
    private float duration;
    private float target;
    [SerializeField] private GameObject StartingTrans;
    public Button lobby, lobby2;
    // Start is called before the first frame update
    void Start()
    {
        audio1.Play();
        StartingTrans.SetActive(true);
        target = 0.4f;
        duration = 3f;
        StartCoroutine(FadeAudioSource.StartFade(audio1, duration, target));
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public void fade1()
    {
        duration = 2;
        target = 0;
        StartCoroutine(FadeAudioSource.StartFade(audio1, duration, target));
        audio2.PlayDelayed(2); 
        duration = 8;
        target = 0.5f;
        StartCoroutine(FadeAudioSource.StartFade(audio2, duration, target));
    }
   
}
