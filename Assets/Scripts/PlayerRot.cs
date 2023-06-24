using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.rotation = Quaternion.Euler(0.0f, other.gameObject.transform.rotation.y, other.gameObject.transform.rotation.z);
        }
    }
}
