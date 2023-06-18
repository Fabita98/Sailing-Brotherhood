using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class WinningConditionNet : NetworkBehaviour
{
    public int TotalLaps;
    public int CurrentLaps;
    private int min, sec, thousends;
    //private StartRace startRace;
    public Text LapDisplayText, victory;
    //public bool isPlayer;
    private bool isRacing;
    public float minTimeBtwLaps, startTime;
    float timeBtwLaps, LapTime;
    bool CanDoLaps, raceStarted;
    private string min_s, sec_s, tho_s, lapTime_s, finalTime;
    //public GameObject coin;
    //[SerializeField] private Vector3 _rotation;
    private OnBoardBehaviourNet OBB1, OBB2;
    public GameObject ship1, ship2;

    void Start()
    {
        OBB1 = ship1.GetComponent<OnBoardBehaviourNet>();
        OBB2 = ship2.GetComponent<OnBoardBehaviourNet>();

    }
        //}
        //void Awake()
        //{
        //    startRace = coin.GetComponent<StartRace>();
        //}
        //// Update is called once per frame
        void Update()
        {
        OBB1 = ship1.GetComponent<OnBoardBehaviourNet>();
        OBB2 = ship2.GetComponent<OnBoardBehaviourNet>();

        if (raceStarted==false && OBB1.CrewmatesList.Count==2&& OBB2.CrewmatesList.Count==2) //da mettere a 2 - 2 
        {
            raceStarted = true;
            startTime = Time.time;
        }
        //    transform.Rotate(_rotation * Time.deltaTime);

        //    if (timeBtwLaps <= 0)
        //    {
        //        CanDoLaps = true;
        //        timeBtwLaps = minTimeBtwLaps;
        //    }
        //    else
        //    {
        //        timeBtwLaps -= Time.deltaTime;
        //    }

        if (raceStarted)
        {
            LapTime = Time.time - startTime;
            if (LapTime < 0.1) LapTime = 0;
            min = (int)LapTime / 60;
            if (min < 10) min_s = "0" + min.ToString(); else min_s = min.ToString();
            sec = (int)LapTime - min * 60;
            if (sec < 10) sec_s = "0" + sec.ToString(); else sec_s = sec.ToString();
            thousends = (int)(LapTime * 100) - min * 60 * 100 - sec * 100;
            if (thousends < 10) tho_s = "0" + thousends.ToString(); else tho_s = thousends.ToString();
            lapTime_s = "Current Lap Time: " + min_s + ":" + sec_s + ":" + tho_s;
            LapDisplayText.text = lapTime_s;
        }
    }
    void LoadEndGame()
    {
        Loader.LoadNetwork(Loader.Scene.EndRace);    
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Ship"))
        {
            isRacing = false;
            finalTime = lapTime_s;
            //CurrentLaps += 1;
            CanDoLaps = false;
            victory.text = other.gameObject.name+ " WON!";
            Invoke("LoadEndGame", 2.0f);

        } 
    }
}
