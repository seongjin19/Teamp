using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRot : MonoBehaviour
{
    public enum AxisOfRotate
    {
        X, Y, Z
    }
    public List<PathCondition> pathCubes = new List<PathCondition>();

    public AxisOfRotate axisOfRotate;

    public float rotSpeed;

    bool isRotate;
    float pastAngle;


    // Start is called before the first frame update
    void Start()
    {
        isRotate = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit mouseHit;

            if (Physics.Raycast(mouseRay, out mouseHit))
            {
                if (mouseHit.transform == transform)
                {
                    isRotate = true;

                    pastAngle = getAngle();

                }
                else
                {
                    for(int i = 0; i < transform.childCount; i++)
                    {
                        if(mouseHit.transform == transform.GetChild(i))
                        {
                            isRotate = true;

                            pastAngle = getAngle();

                            break;
                        }
                    }
                }
            }
        }
        if (isRotate)
        {
            Vector2 rot = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            rot *= rotSpeed;

            // ���� ȸ��
            transform.Rotate(((axisOfRotate == AxisOfRotate.X) ? (rot.x) : (0)),
                    ((axisOfRotate == AxisOfRotate.Y) ? (rot.x) : (0)),
                    ((axisOfRotate == AxisOfRotate.Z) ? (rot.x) : (0)));


            if (Mathf.Abs(getAngle() - pastAngle) >= 30.0f)
            {
                pastAngle = getAngle();
            }

            // ���� Ȯ�� �� ť�� ��� ����
            for (int i = 0; i < pathCubes.Count; i++)
            {
                for (int j = 0; j < pathCubes[i].path.Count; j++)
                {
                    pathCubes[i].path[j].block.possiblePaths[pathCubes[i].path[j].index - 1].active =
                        transform.eulerAngles.Equals(pathCubes[i].angle);
                }
            }
            // ���콺�� ���� �� �̻� �������� ����
            if (Input.GetMouseButtonUp(0))
            {
                isRotate = false;

                float currentAngle = getAngle();
                float targetAngle;

                // �ڵ����� ���� ���� ã��
                for (int i = 270; i >= 0; i -= 90)
                {
                    if (currentAngle > i)
                    {
                        if (currentAngle > i + 45)
                        {
                            targetAngle = i + 90;
                        }
                        else
                        {
                            targetAngle = i;
                        }

                        // ���� �ڵ����� ����
                        StartCoroutine(Rotate(currentAngle, targetAngle, true));

                        break;
                    }
                }
            }
        }
    }
    IEnumerator Rotate(float startAngle, float finalAngle, bool outBack)
    {
        float timing = 0;

        // ƨ�� ����
        float firstAngle = (finalAngle > startAngle) ? (finalAngle + 12.5f) : (finalAngle - 12.5f);
        // ���� ����
        float nextAngle = (outBack) ? (firstAngle) : (finalAngle);

        // ȸ��
        while (timing < 1.0f)
        {
            float angle = Mathf.Lerp(startAngle, nextAngle, timing);

            setAngle(angle);

            timing += Time.deltaTime * rotSpeed;

            if (timing >= 1.0f)
            {
                setAngle(nextAngle);

                // �� �� ƨ�� ���
                if (nextAngle != finalAngle)
                {
                    startAngle = nextAngle;
                    nextAngle = finalAngle;
                    timing = 0;
                }
            }

            yield return null;
        }
        setAngle((finalAngle >= 360) ? (finalAngle - 360) : (finalAngle));

        // ���� Ȯ�� �� ť�� ��� ����
        for (int i = 0; i < pathCubes.Count; i++)
        {
            for (int j = 0; j < pathCubes[i].path.Count; j++)
            {
                pathCubes[i].path[j].block.possiblePaths[pathCubes[i].path[j].index-1].active =
                    transform.eulerAngles.Equals(pathCubes[i].angle);
            }
        }

        yield return null;
    }

    private void setAngle(float angle)
    {
        transform.rotation = Quaternion.Euler(new Vector3(
            (axisOfRotate == AxisOfRotate.X) ? angle : 0,
            (axisOfRotate == AxisOfRotate.Y) ? angle : 0,
            (axisOfRotate == AxisOfRotate.Z) ? angle : 0));
    }
    private float getAngle()
    {
        float ret = 0;

        switch (axisOfRotate)
        {
            case AxisOfRotate.X: ret = transform.eulerAngles.x; break;
            case AxisOfRotate.Y: ret = transform.eulerAngles.y; break;
            case AxisOfRotate.Z: ret = transform.eulerAngles.z; break;
        }

        return ret;
    } 
}

[System.Serializable]
public class PathCondition
{
    public Vector3 angle;
    public List<SinglePath> path;
}

[System.Serializable]
public class SinglePath
{
    public Walkable block;
    public int index;
}


