using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRace : MonoBehaviour
{
    [SerializeField] private Vector3 _rotation;
    public int countdownTime;
    public Object Obj;
    public AudioSource coinEffect;
    public float startTime;
    public int playAfter;
    public bool isLast=false;
    private Crest.BoatProbes boatprobes;
    public GameObject ship;
    

    IEnumerator CountdownStart()
    {
        while (countdownTime > 0)
        {
            _rotation.z = -720;
            if (playAfter==0) coinEffect.Play();
            yield return new WaitForSeconds(1f);        
            countdownTime--;
            playAfter--;
        }
        if (isLast)
        {
            boatprobes._playerControlled = true;
        }
        Destroy(Obj);
        
    }

    private void Awake()
    {
        boatprobes = ship.GetComponent<Crest.BoatProbes>();
    }
    void Start()
    {
        StartCoroutine(CountdownStart());

    }

 
    // Update is called once per frame
    void Update()
    {
        startTime = Time.time;
        transform.Rotate(_rotation * Time.deltaTime);
    }
}
