using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuToLobby : MonoBehaviour
{
    public AudioSource audio;
    public float duration;
    public float target;
    [SerializeField] private GameObject EndingTrans;
    // Start is called before the first frame update
    void Start()
    {
        audio.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            EndingTrans.SetActive(true);
            StartCoroutine(FadeAudioSource.StartFade(audio, duration, target));
            Invoke("change_scene",2);           
        }
    }
    void change_scene()
    {
        SceneManager.LoadScene("Lobby");
    }
}
