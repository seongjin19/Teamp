using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // 현재 위치한 큐브
    public Transform currentCube;
    // 마우스 클릭한 큐브
    public Transform clickedCube;

    private Animator animator;

    
    // 플레이어가 실제 이동할 경로 저장(이동할 큐브들의 Transform을 요소로 저장)
    public List<Transform> finalPath = new List<Transform>();

    // 이동 시 사용할 변수
    Walkable pastCube;
    Walkable nextCube;
    float timing = 0;

    public float moveSpeed;

    void Start()
    {
        animator = GetComponent<Animator>();

        // 플레이어가 밟고 있는 큐브 설정
        RayCastDown();

        // 플레이어 레이어 설정
        LayerCheck(currentCube);
    }

    void Update()
    {
        // 플레이어 아래에 있는 큐브 확인
        RayCastDown();

        // 플레이어가 밟고 있는 큐브가 움직이는 경우
        if (currentCube.GetComponent<Walkable>().movingGround)
        {
            // 플레이어를 그 자식으로 넣는다
            transform.parent = currentCube.parent;
        }
        else
        {
            transform.parent = null;
        }

        
        if (Input.GetMouseButtonDown(1))
        {
                animator.SetBool("is_walk", true);
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit mouseHit;

                
                if (Physics.Raycast(mouseRay, out mouseHit))
                {
                    // 클릭한 곳이 Path인 경우
                    if (mouseHit.transform.GetComponent<Walkable>() != null)
                    {
                        clickedCube = mouseHit.transform;

                        // 경로 초기화
                        finalPath.Clear();

                        FindPath();
                    }
                }
        }
        // 탐색 종료 후 플레이어 이동
        FollowPath();
    }

    // 길찾기
    private void FindPath()
    {
        // 다음 이동할 큐브 리스트
        List<Transform> nextCubes = new List<Transform>();
        // 이전 큐브 리스트
        List<Transform> pastCubes = new List<Transform>();

        // 현재 큐브의 연결된 큐브 갯수만큼 루프
        for (int i = 0; i < currentCube.GetComponent<Walkable>().possiblePaths.Count; i++)
        {
            WalkPath walkPath = currentCube.GetComponent<Walkable>().possiblePaths[i];

            if (walkPath.active)
            {
                nextCubes.Add(walkPath.target);

                walkPath.target.GetComponent<Walkable>().previousBlock = currentCube;
            }
        }

        pastCubes.Add(currentCube);

        ExploreCube(nextCubes, pastCubes);
        BuildPath();
    }


    private void ExploreCube(List<Transform> nextCubes, List<Transform> visitedCubes)
    {
        Transform current = nextCubes.First();
        nextCubes.Remove(current);

        // 클릭한 큐브와 현재 큐브가 같으면 목표 발판에 도착
        if (current == clickedCube)
        {
            return;
        }

        // 현재 큐브의 이동 가능한 큐브만큼 반복
        for (int i = 0; i < current.GetComponent<Walkable>().possiblePaths.Count; i++)
        {
            WalkPath walkPath = current.GetComponent<Walkable>().possiblePaths[i];

            // 이미 지나온 길이 아니고 길이 연결되어 있다면
            if (!visitedCubes.Contains(walkPath.target) && walkPath.active)
            {
                // 다음 검색 큐브로
                nextCubes.Add(walkPath.target);

                // 이동할 경로에 추가
                walkPath.target.GetComponent<Walkable>().previousBlock = current;
            }

        }

        // 방문한 큐브 리스트에 현재 큐브 추가
        visitedCubes.Add(current);

        // 리스트에 원소가 하나라도 있다면 재귀적으로 호출하여 큐브 탐색
        if (nextCubes.Count > 0)
        {
            ExploreCube(nextCubes, visitedCubes);
        }
    }

    // 경로 생성
    private void BuildPath()
    {
        Transform cube = clickedCube; //(변수를 클릭한 큐브로 초기화)

        // cube가 현재큐브와 같지 않을 때까지 반복
        while (cube != currentCube)
        {
            // 실제 이동할 경로 리스트에 현재cube 삽입
            finalPath.Add(cube);

            // 클릭한 큐브의 이전큐브가 None일 때 종료
            if (cube.GetComponent<Walkable>().previousBlock != null)
            {
                cube = cube.GetComponent<Walkable>().previousBlock;
            }
            else
            {
                break;
            }
        }

        if (finalPath.Count > 0)
        {
            bool walk = false;

            for (int i = 0; i < currentCube.GetComponent<Walkable>().possiblePaths.Count; i++)
            {
                WalkPath walkCube = currentCube.GetComponent<Walkable>().possiblePaths[i];

                if (walkCube.target.Equals(finalPath[finalPath.Count - 1]) && walkCube.active)
                {
                    walk = true;
                    break;
                }
            }

            if (!walk)
            {
                finalPath.Clear();
            }
            else
            {
                pastCube = currentCube.GetComponent<Walkable>();
                nextCube = finalPath[finalPath.Count - 1].GetComponent<Walkable>();

                transform.LookAt(nextCube.GetWalkPoint());

                timing = 0;
            }
        }
    }

    void FollowPath()
    {
        if (finalPath.Count == 0)
        {
            return;
        }

        // 타일을 체크하여 플레이어 레이어 설정
        LayerCheck(nextCube.transform);

        // 보간 적용 (플레이어 실제 이동)
        transform.position = Vector3.Lerp(pastCube.GetWalkPoint(), nextCube.GetWalkPoint(), timing);

        // 다음 경로 설정(플레이어가 이동 경로의 마지막 큐브에 도착)
        if (timing >= 1.0f)
        {
            timing = 0;

            pastCube = finalPath.Last().GetComponent<Walkable>();

            finalPath.Last().GetComponent<Walkable>().previousBlock = null;
            finalPath.RemoveAt(finalPath.Count - 1);

            if (finalPath.Count > 0)
            {
                nextCube = finalPath.Last().GetComponent<Walkable>();

                if (!currentCube.GetComponent<Walkable>().dontRotate)
                {
                    transform.LookAt(nextCube.GetWalkPoint());
                }

                return;
            }
            else
            {
                LayerCheck(nextCube.transform);
                animator.SetBool("is_walk", false);
            }
        }

        timing += Time.deltaTime * moveSpeed;
    }

    // 현재 플레이어가 밟고 있는 큐브 찾는 함수
    public void RayCastDown()
    {
        // 플레이어 센터 포지션 생성
        Vector3 rayPos = transform.position;
        rayPos.y += transform.localScale.y * 0.5f;

        
        Ray playerRay = new Ray(rayPos, -transform.up);
        RaycastHit playerHit;

        if (Physics.Raycast(playerRay, out playerHit))
        {
            // 플레이어가 발판을 밟고 있다면
            if (playerHit.transform.GetComponent<Walkable>() != null)
            {
                currentCube = playerHit.transform;
            }
        }
    }

    // 발판을 검사하여 플레이어의 레이어 설정
    void LayerCheck(Transform cube)
    {
        bool isTop = false;

        if (cube.childCount > 0)
        {
            isTop = true;
        }

        if (!isTop)
        {
            for (int i = 0; i < cube.GetComponent<Walkable>().possiblePaths.Count; i++)
            {
                if (cube.GetComponent<Walkable>().possiblePaths[i].target.childCount > 0)
                {
                    isTop = true;
                    break;
                }
            }
        }

        if (isTop)
        {
            SetLayerObject(transform, "Top");
        }
        else
        {
            SetLayerObject(transform, "Default");
        }
    }

    // 자식 오브젝트까지 모두 레이어 설정
    void SetLayerObject(Transform tr, string layerName)
    {
        tr.gameObject.layer = LayerMask.NameToLayer(layerName);

        for (int i = 0; i < tr.childCount; i++)
        {
            // 자식의 자식들까지 설정
            SetLayerObject(tr.GetChild(i), layerName);
        }
    }
}
