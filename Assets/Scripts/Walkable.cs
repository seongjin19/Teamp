using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walkable : MonoBehaviour
{
    public List<WalkPath> possiblePaths = new List<WalkPath>();

    //지나온 길
    public Transform previousBlock;
    // Start is called before the first frame update

    public bool dontRotate;

    public bool movingGround = false;
    public Vector3 GetWalkPoint()
    {
        return transform.position + transform.up * 0.5f - transform.up;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(GetWalkPoint(), new Vector3(0.1f, 0.1f, 0.1f));

        for(int i = 0; i < possiblePaths.Count; i++)
        {
            if (possiblePaths[i].active)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(GetWalkPoint(), possiblePaths[i].target.GetComponent<Walkable>().GetWalkPoint());
            }
        }
    }
}

[System.Serializable]
public class WalkPath
{
    public Transform target;
    public bool active = true;
}
