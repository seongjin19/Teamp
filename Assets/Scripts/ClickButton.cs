using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickButton : MonoBehaviour
{
    public GameObject clickImage;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BlinkImage());
    }

    // Update is called once per frame
    public IEnumerator BlinkImage()
    {
        while (true)
        {
            clickImage.SetActive(false);
            yield return new WaitForSeconds(0.7f);
            clickImage.SetActive(true);
            yield return new WaitForSeconds(0.7f);
        }
    }
}
