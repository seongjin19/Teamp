using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Space]
    [Header("Cube Information")]
    // ���� ��ġ�� ť��
    public Transform currentCube;
    // ���콺 Ŭ���� ť��
    public Transform clickedCube;

    private Animator animator;

    [Space]
    [Header("PathFinding")]
    // �÷��̾ ���� �̵��� ���
    public List<Transform> finalPath = new List<Transform>();

    // �̵� �� ����� ����
    Walkable pastCube;
    Walkable nextCube;
    float timing = 0;

    [Space]
    public float moveSpeed;

    void Start()
    {
        animator = GetComponent<Animator>();

        // �÷��̾ ��� �ִ� ť�� ����
        RayCastDown();

        // �÷��̾� ���̾� ����
        LayerCheck(currentCube);
    }

    void Update()
    {
        // �÷��̾ ��� �ִ� ť�� ����
        RayCastDown();

        // ���� ��� �ִ� ť�갡 �����̴� ���
        if (currentCube.GetComponent<Walkable>().movingGround)
        {
            // �÷��̾ �� �ڽ����� �ִ´�
            transform.parent = currentCube.parent;
        }
        else
        {
            transform.parent = null;
        }

        // ���콺 Ŭ���� ���ӸŴ��� �÷��� üũ
        if (Input.GetMouseButtonDown(1))
        {
                animator.SetBool("is_walk", true);
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit mouseHit;

                // ���� �߻�!!
                if (Physics.Raycast(mouseRay, out mouseHit))
                {
                    // Ŭ���� ���� Path�� ���
                    if (mouseHit.transform.GetComponent<Walkable>() != null)
                    {
                        // Ŭ���� ���
                        //SoundManager.instance.play("Navi", 0.5f);

                        // Ŭ���� ť�� ��ġ ����
                        clickedCube = mouseHit.transform;

                        // ��� �ʱ�ȭ
                        finalPath.Clear();

                        // ��ã�� ����
                        FindPath();
                    }
                }
        }

        FollowPath();
    }

    // ��ã��
    private void FindPath()
    {
        // ���� �̵��� ť��
        List<Transform> nextCubes = new List<Transform>();
        // ���� ť��
        List<Transform> pastCubes = new List<Transform>();

        // ���� ť���� ����� ť�� ������ŭ ����
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

        // Ŭ���� ť��� ���� ť�갡 ������
        // ��ǥ ��ǥ�� ������ ��
        if (current == clickedCube)
        {
            return;
        }

        // ���� ť���� �̵� ������ ť�길ŭ �ݺ�
        for (int i = 0; i < current.GetComponent<Walkable>().possiblePaths.Count; i++)
        {
            WalkPath walkPath = current.GetComponent<Walkable>().possiblePaths[i];

            // �̹� ������ ���� �ƴϰ� ���� ����Ǿ� �ִٸ�
            if (!visitedCubes.Contains(walkPath.target) && walkPath.active)
            {
                // ���� �˻� ť���
                nextCubes.Add(walkPath.target);

                // �̵��� ��ο� �߰�
                walkPath.target.GetComponent<Walkable>().previousBlock = current;
            }

        }

        // �湮�� ť�� ����Ʈ�� ���� ť�� �߰�
        visitedCubes.Add(current);

        // ����Ʈ�� �ϳ��� �ִٸ�
        if (nextCubes.Count > 0)
        {
            ExploreCube(nextCubes, visitedCubes);
        }
    }

    // ��� ����
    private void BuildPath()
    {
        Transform cube = clickedCube;

        // Ŭ���� ť�갡 ����ť��� ���� ���� ������
        while (cube != currentCube)
        {
            // ���� �̵��� ��ο� ����
            finalPath.Add(cube);

            // Ŭ���� ť���� ����ť�갡 None�� ��
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

        // Ÿ���� üũ�Ͽ� �÷��̾� ���̾� ����
        LayerCheck(nextCube.transform);

        // ���� ����
        transform.position = Vector3.Lerp(pastCube.GetWalkPoint(), nextCube.GetWalkPoint(), timing);

        // ���� ��� ����
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

    // ���� �÷��̾ ��� �ִ� ť�� ã�� �Լ�
    public void RayCastDown()
    {
        // �÷��̾� ���� ������ ����
        Vector3 rayPos = transform.position;
        rayPos.y += transform.localScale.y * 0.5f;

        // ���� ����, ������ �Ʒ�
        Ray playerRay = new Ray(rayPos, -transform.up);
        RaycastHit playerHit;

        // ���� �߻�!!
        if (Physics.Raycast(playerRay, out playerHit))
        {
            // ������ ��� �ִٸ�
            if (playerHit.transform.GetComponent<Walkable>() != null)
            {
                currentCube = playerHit.transform;
            }
        }
    }

    // ������ �˻��Ͽ� �÷��̾��� ���̾� ����
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

    // �ڽ� ������Ʈ���� ��� ���̾� ����
    void SetLayerObject(Transform tr, string layerName)
    {
        tr.gameObject.layer = LayerMask.NameToLayer(layerName);

        for (int i = 0; i < tr.childCount; i++)
        {
            // �ڽ��� �ڽĵ���� ����
            SetLayerObject(tr.GetChild(i), layerName);
        }
    }
}
