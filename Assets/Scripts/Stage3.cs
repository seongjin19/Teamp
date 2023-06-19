using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage3 : MonoBehaviour
{
    public GameObject pB;

    bool on_board;

    // Start is called before the first frame update
    void Start()
    {
        on_board = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(on_board)
        {
            if (pB.transform.rotation != Quaternion.Euler(-90.0f, 0.0f, 0.0f))
            {
                pB.transform.Rotate(new Vector3(-90.0f, 0.0f, 0.0f) * Time.deltaTime * 0.5f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            on_board = true;
        }
    }

}


