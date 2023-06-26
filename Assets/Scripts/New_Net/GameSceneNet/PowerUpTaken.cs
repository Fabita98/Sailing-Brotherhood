using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpTaken : MonoBehaviour
{
    [SerializeField]
    private Text speed, smallCannon, barrel, reload;
    public GameObject powerUpUI;
    public GameObject shipA,shipB;

    public static PowerUpTaken Instance { get; private set; }


    private void Start()
    {
        HideUI();   
    }   

    public void ShowSpeedPowerUpTaken(GameObject s)
    {       if (s == shipA || s == shipB)
        {
            ShowUI();
            speed.text = s.name + " gained a Speed boost!";
            StartCoroutine(HideAfterDelay(speed));
        }       
    }

    public void ShowSmallcannonPowerUpTaken(GameObject s)
    {       if (s == shipA || s == shipB)
        {
            ShowUI();
            smallCannon.text = s.name + " gained a granted Shot!";
            StartCoroutine(HideAfterDelay(smallCannon));
        }       
    }
    
    public void ShowBarrelPowerUpTaken(GameObject s)
    {       if (s == shipA || s == shipB)
        {
            ShowUI();
            barrel.text = s.name + " gained a Barrel!";
            StartCoroutine(HideAfterDelay(barrel));
        }       
    }

    public void ShowReloadPowerUpTaken(GameObject s)
    {       if (s == shipA || s == shipB)
        {
            ShowUI();
            reload.text = s.name + " gained a fast Reload!";
            StartCoroutine(HideAfterDelay(reload));
        }       
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