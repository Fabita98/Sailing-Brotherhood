using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DelayShowing : MonoBehaviour
{
    [SerializeField]
    private Button ready;

    private void Start()
    {
        ready.GetComponent<Image>().enabled = false;
    }

    public void Wrapper()
    {
        StartCoroutine(ShowAfterDelay());
    }

    private IEnumerator ShowAfterDelay()
    {
        yield return new WaitForSeconds(4);
        ready.GetComponent<Image>().enabled = true;
    }


}