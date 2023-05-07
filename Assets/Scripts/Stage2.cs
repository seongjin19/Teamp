using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Stage2 : MonoBehaviour
{
    public GameObject obs;
    public GameObject s1;
    public GameObject s2;

    Vector3 t1;
    Vector3 t2;
    Vector3 t3;
    bool is_tag2;
    // Start is called before the first frame update
    void Start()
    {
        is_tag2 = false;
        t1 = new Vector3(0, 30, 0);
        t2 = new Vector3(0, 7.5f, 0);
        t3 = new Vector3(0, 7.5f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (is_tag2)
        {
            obs.transform.position = Vector3.MoveTowards(obs.transform.position, t1, 0.008f);
            s1.transform.position = Vector3.MoveTowards(s1.transform.position, t2, 0.008f);
            s2.transform.position = Vector3.MoveTowards(s2.transform.position, t3, 0.008f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("tag2"))
        {
            is_tag2 = true;
        }
    }
}
