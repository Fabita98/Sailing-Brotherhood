using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpTaken : MonoBehaviour
{
    [SerializeField]
    private Text speed, smallCannon, barrel, reload;
    public GameObject powerUpUI;

    public static PowerUpTaken Instance { get; private set; }


    private void Start()
    {           
    }   

    public void ShowSpeedPowerUpTaken(GameObject s)
    {                 
            speed.text = s.name + " gained a Speed boost!";
            StartCoroutine(HideAfterDelay(speed));               
    }

    public void ShowSmallcannonPowerUpTaken(GameObject s)
    {        
            smallCannon.text = s.name + " gained a granted Shot!";
            StartCoroutine(HideAfterDelay(smallCannon));              
    }
    
    public void ShowBarrelPowerUpTaken(GameObject s)
    {        
            barrel.text = s.name + " gained a Barrel!";
            StartCoroutine(HideAfterDelay(barrel));               
    }

    public void ShowReloadPowerUpTaken(GameObject s)
    {        
            reload.text = s.name + " gained a fast Reload!";
            StartCoroutine(HideAfterDelay(reload));               
    }

    public void ShowUI()
    {
        powerUpUI.SetActive(true);
    }

    public void HideUI()
    {
        powerUpUI.SetActive(false);
    }

    private IEnumerator HideAfterDelay(Text t)
    {
        yield return new WaitForSeconds(3);
        t.enabled = false;
    }
}