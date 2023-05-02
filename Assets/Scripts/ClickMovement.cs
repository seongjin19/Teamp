using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickMovement : MonoBehaviour
{
    private Camera camera;

    private bool isMove;
    private Vector3 destination;

    private void Awake()
    {
        camera = Camera.main;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            RaycastHit hit;
            if(Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition),out hit))
            {
                var center = hit.collider.bounds.center;
                SetDestination(center);
            }
        }
        Move();
    }
    private void SetDestination(Vector3 dest)
    {
        destination = dest;
        isMove = true;
    }

    private void Move()
    {
        if (isMove)
        {
            if(Vector3.Distance(destination, transform.position)<= 0.5f)
            {
                isMove = false;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                return;
            }
        var dir = destination - transform.position;
        transform.position += dir.normalized * Time.deltaTime * 5f;
        }
    }
}
