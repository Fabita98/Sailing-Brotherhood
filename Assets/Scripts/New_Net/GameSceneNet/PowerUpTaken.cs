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
    
    public void ShowSpeedPowerUpTaken(GameObject s)
    {
            Show(speed);
            speed.text = s.name + " gained a Speed boost!";
            StartCoroutine(HideAfterDelay(speed));               
    }

    public void ShowSmallcannonPowerUpTaken(GameObject s)
    {                    
            Show(smallCannon);
            smallCannon.text = s.name + " gained a granted Shot!";
            StartCoroutine(HideAfterDelay(smallCannon));              
    }
    
    public void ShowBarrelPowerUpTaken(GameObject s)
    {                    
            Show(barrel);
            barrel.text = s.name + " gained a Barrel!";
            StartCoroutine(HideAfterDelay(barrel));               
    }

    public void ShowReloadPowerUpTaken(GameObject s)
    {        
            Show(reload);
            reload.text = s.name + " gained a fast Reload!";
            StartCoroutine(HideAfterDelay(reload));               
    }

    public void ShowUI()
    {
        powerUpUI.SetActive(true);
    }
    
    public void Show(Text t)
    {
        t.enabled = true;
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