using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinningCondition : MonoBehaviour
{
    public int TotalLaps;
    public int CurrentLaps;
    private int min, sec, thousends;
    private StartRace startRace;
    public Text LapDisplayText, victory;
    public bool isPlayer;
    private bool isRacing=true;
    public float minTimeBtwLaps;
    float timeBtwLaps, LapTime;
    bool CanDoLaps;
    private string min_s, sec_s, tho_s, lapTime_s, finalTime;
    public GameObject coin;
    [SerializeField] private Vector3 _rotation;

    void Start()
    {

    }
    void Awake()
    {
        startRace = coin.GetComponent<StartRace>();
    }
        // Update is called once per frame
        void Update()
    {
        transform.Rotate(_rotation * Time.deltaTime);

        if (timeBtwLaps <= 0)
        {
            CanDoLaps = true;
            timeBtwLaps = minTimeBtwLaps;
        }
        else
        {
            timeBtwLaps -= Time.deltaTime;
        }

        if (isPlayer&isRacing)
        {
            LapTime = Time.time-startRace.startTime;
            if (LapTime < 0.1) LapTime = 0;
            min = (int)LapTime / 60;
            if (min < 10) min_s = "0" + min.ToString(); else min_s = min.ToString();
            sec = (int)LapTime - min * 60;
            if (sec < 10) sec_s = "0" + sec.ToString(); else sec_s = sec.ToString();
            thousends = (int)(LapTime*100) - min * 60*100 - sec*100;
            if (thousends < 10) tho_s = "0" + thousends.ToString(); else tho_s = thousends.ToString();
            lapTime_s = "Current Lap Time: "+min_s+":"+sec_s+":"+tho_s;
            LapDisplayText.text = lapTime_s;
        }
    }
    void LoadEndGame()
    {
        SceneManager.LoadScene("EndRace");
        //SceneManager.UnloadSceneAsync("SampleScene");
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (CanDoLaps)
        {
            isRacing = false;
            Debug.Log("You won!");
            finalTime = lapTime_s;
            CurrentLaps += 1;
            CanDoLaps = false;
            victory.text = "YOU WON!";
            Invoke("LoadEndGame", 2.0f);
            
        }
    }
}
