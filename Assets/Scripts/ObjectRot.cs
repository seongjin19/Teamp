using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRot : MonoBehaviour
{
    public GameObject SecondFloar;

    public enum AxisOfRotate
    {
        X, Y, Z
    }
    public List<PathCondition> pathCubes = new List<PathCondition>();
    public List<PathCondition> pathCubes1 = new List<PathCondition>();
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

            // 오브젝트 회전
            if (axisOfRotate == AxisOfRotate.X)
            {
                transform.Rotate(rot.x, 0, 0);
            }
            else if (axisOfRotate == AxisOfRotate.Y)
            {
                transform.Rotate(0, rot.x, 0);
            }
            else if (axisOfRotate == AxisOfRotate.Z)
            {
                transform.Rotate(0, 0, rot.x);
            }


            if (Mathf.Abs(getAngle() - pastAngle) >= 30.0f)
            {
                pastAngle = getAngle();
            }

           
            if (Input.GetMouseButtonUp(0))
            {
                isRotate = false;

                float currentAngle = getAngle();
                float targetAngle;

                // 자동으로 적절한 각도 설정
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

                        // 각도 설정 입력
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

        // 마우스를 놓았을 때 반대로 살짝 이동
        float firstAngle;
        if (finalAngle > startAngle)
        {
            firstAngle = finalAngle + 12.5f;
        }
        else
        {
            firstAngle = finalAngle - 12.5f;
        }

        // 다음 각도
        float nextAngle;
        if (outBack)
        {
            nextAngle = firstAngle;
        }
        else
        {
            nextAngle = finalAngle;
        }

    
        while (timing < 1.0f)
        {
            float angle = Mathf.Lerp(startAngle, nextAngle, timing);

            setAngle(angle);

            timing += Time.deltaTime * rotSpeed;

            if (timing >= 1.0f)
            {
                setAngle(nextAngle);

                if (nextAngle != finalAngle)
                {
                    startAngle = nextAngle;
                    nextAngle = finalAngle;
                    timing = 0;
                }
            }

            yield return null;
        }

        if (finalAngle >= 360)
        {
            setAngle(finalAngle - 360);
        }
        else
        {
            setAngle(finalAngle);
        }

        // 각도 확인 후 큐브 경로 설정
        if (SecondFloar.GetComponent<Stage2>().GetIs_tag2() == false)
        {
            for (int i = 0; i < pathCubes.Count; i++)
            {
                for (int j = 0; j < pathCubes[i].path.Count; j++)
                {
                    pathCubes[i].path[j].block.possiblePaths[pathCubes[i].path[j].index - 1].active =
                        transform.eulerAngles.Equals(pathCubes[i].angle);
                }
            }
        }
        else
        {
            for (int i = 0; i < pathCubes.Count; i++)
            {
                for (int j = 0; j < pathCubes[i].path.Count; j++)
                {
                    pathCubes[i].path[j].block.possiblePaths[pathCubes[i].path[j].index - 1].active = false;
                        
                }
            }
            for (int i = 0; i < pathCubes1.Count; i++)
            {
                for (int j = 0; j < pathCubes1[i].path.Count; j++)
                {
                    pathCubes1[i].path[j].block.possiblePaths[pathCubes1[i].path[j].index - 1].active =
                        transform.eulerAngles.Equals(pathCubes1[i].angle);
                }
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


