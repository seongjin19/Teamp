using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walkable : MonoBehaviour
{
    // 이동 가능한 경로를 저장하는 리스트. WalkPath 오브젝트들의 리스트로 구성
    public List<WalkPath> possiblePaths = new List<WalkPath>();

    //지나온 길(이전 블록 저장)
    public Transform previousBlock;
    // Start is called before the first frame update

    public bool dontRotate;

    public bool movingGround = false;

    // 현재 블록의 이동 가능한 지점 반환
    public Vector3 GetWalkPoint()
    {
        return transform.position + transform.up * 0.5f - transform.up;
    }

    // 이동 위치를 시각적으로 표현
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(GetWalkPoint(), new Vector3(0.1f, 0.1f, 0.1f));

        for(int i = 0; i < possiblePaths.Count; i++)
        {
            if (possiblePaths[i].active)
            {
                // 이동 가능한 선 그리기
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
