using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using System.Diagnostics;

public class DrawLine : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    GameObject LineGO;

    bool StartDrawing;

    Vector3 MousePos;

    LineRenderer LR;

    [SerializeField]
    Material LineMat;

    int CurrentIndex;

    [SerializeField]
    Camera cam;

    [SerializeField] Transform fencePrefab;

    [SerializeField] Transform fence_blockPrefab;

    [SerializeField] Vector3 lastBlockDrawStartPoint;


    [SerializeField] private bool drawBlockWall = true;
    [SerializeField] private float blockDrawDist = 0.1f;

    Transform LastInstantiated_Collider;
    Vector3 lastFenceDrawStartPoint;

    [Space]

    [SerializeField] private float drawDistance = 30000f;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private List<GameObject> generatedFence = new List<GameObject>();

    [Space]

    [SerializeField] private Transform displayLineParent;
    LineRenderer displayLR;
    [SerializeField] private float displayLRDrawDistance = 1;

    int displayLRCurrentIndex;

    [SerializeField] private bool drawDisplayLine = false;
    [SerializeField] private Material defaultMat;

    [Tooltip("오브젝트를 통과하여 펜스를 그리는것을 허용하는지")]
    [SerializeField] private bool enablePassDrawObject = false;
    [SerializeField] private LayerMask fenceObstacleMask;

    [Space]

    [SerializeField] private Text debug_distStartToMouse;
    [SerializeField] private Text debug_maxDist;

    [Space]
    [SerializeField] public UnityEvent OnDrawStartEvent;
    [SerializeField] public UnityEvent OnDrawEndEvent;


    // 1. 클릭시 클릭한 지점에 먼저 투명한 펜스하나를 생성함
    // 2. 클릭한 상태로 드래그하여 drawDistance의 길이보다 커지면
    // 3. 생성되어있는 펜스를 현재 마우스 위치를 바라보게 하고
    // 4. 시작지점 부터 마우스 위치까지 길이로 펜스 길이를 만듬

    void Start()
    {
        LineGO = new GameObject();
        LineGO.tag = "Obstacle";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            drawBlockWall = !drawBlockWall;
        }


        if (StartDrawing)
        {

            Ray cast = cam.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit3;

            Vector3 Dist = MousePos;

            if (Physics.Raycast(cast, out hit3, Mathf.Infinity, groundLayer))
            {
                Dist = MousePos - hit3.point;
            }

            if (LastInstantiated_Collider != null)
            {
                FixFencePosition(LastInstantiated_Collider.gameObject, hit3.point);
            }

#if UNITY_EDITOR
            UnityEngine.Debug.DrawLine(lastFenceDrawStartPoint + new Vector3(0, 0.3f, 0), hit3.point + new Vector3(0, 0.3f, 0), Color.white);
#endif

            float Distance_SqrMag = Dist.sqrMagnitude;

            //print(Distance_SqrMag);

            if (LastInstantiated_Collider.gameObject.activeSelf)
            {
                RaycastHit mousePositionHit2;
                Vector3 mousePos2 = Vector3.zero;

                if (Physics.Raycast(cast, out mousePositionHit2, Mathf.Infinity, groundLayer))
                {
                    mousePos2 = mousePositionHit2.point;
                }

                if (redFence.Count > 0)
                {
                    if (!Physics.Raycast(redFence[0].transform.position + new Vector3(0, 0.3f, 0), ((mousePos2 + new Vector3(0, 0.3f, 0)) - (redFence[0].transform.position + new Vector3(0, 0.3f, 0))).normalized, Vector3.Distance(redFence[0].transform.position, mousePos2), fenceObstacleMask))
                    {
                        ClearRedFence();

                        MousePos = LastInstantiated_Collider.transform.position;
                    }
                    else
                    {
                        if (drawBlockWall)
                            CheckingRedFenceLength(redFence, lastBlockDrawStartPoint, mousePos2);
                        else
                            CheckingRedFenceLength(redFence, lastFenceDrawStartPoint, mousePos2);
                    }
                }

                if (drawBlockWall)
                {
                    if (Physics.Raycast(lastBlockDrawStartPoint + new Vector3(0, 0.3f, 0), ((mousePos2 + new Vector3(0, 0.3f, 0)) - (lastBlockDrawStartPoint + new Vector3(0, 0.3f, 0))).normalized, Vector3.Distance(lastBlockDrawStartPoint, mousePos2), fenceObstacleMask))
                    {
                        LastInstantiated_Collider.gameObject.SetActive(false);

                        if (redFence == null)
                            DrawRedFence(lastBlockDrawStartPoint);

                        CheckingRedFenceLength(redFence, lastBlockDrawStartPoint, mousePos2);
                    }
                }
                else
                {
                    if (Physics.Raycast(lastFenceDrawStartPoint + new Vector3(0, 0.3f, 0), ((mousePos2 + new Vector3(0, 0.3f, 0)) - (lastFenceDrawStartPoint + new Vector3(0, 0.3f, 0))).normalized, Vector3.Distance(lastFenceDrawStartPoint, mousePos2), fenceObstacleMask))
                    {
                        LastInstantiated_Collider.gameObject.SetActive(false);

                        if (redFence == null)
                            DrawRedFence(lastFenceDrawStartPoint);

                        CheckingRedFenceLength(redFence, lastFenceDrawStartPoint, mousePos2);
                    }
                }
            }

            //선 그리기 활성화 되었을때만 작동
            if (displayLRDrawDistance > Distance_SqrMag && drawDisplayLine)
            {
                RaycastHit hit;

                Physics.Raycast(cast, out hit, Mathf.Infinity, groundLayer);

                displayLR.SetPosition(displayLRCurrentIndex, hit.point + new Vector3(0, 0.3f, 0));

                displayLRCurrentIndex++;

                displayLR.positionCount = displayLRCurrentIndex + 1;

                RaycastHit hit2;

                Physics.Raycast(cast, out hit2, Mathf.Infinity, groundLayer);

                displayLR.SetPosition(displayLRCurrentIndex, hit2.point + new Vector3(0, 0.3f, 0));
            }

            //print("Distance_SqrMag : " + Distance_SqrMag + " / drawDistance : " + drawDistance);
            debug_distStartToMouse.text = "Distance_SqrMag : " + Distance_SqrMag;
            debug_maxDist.text = "drawDistance : " + drawDistance;

            if (drawBlockWall)
            {
                if (Distance_SqrMag > blockDrawDist)
                {
                    RaycastHit mousePositionHit;

                    if (Physics.Raycast(cast, out mousePositionHit, Mathf.Infinity, groundLayer))
                    {
                        MousePos = mousePositionHit.point;
                    }

                    // 장애물이 감지된 경우
                    if (Physics.Raycast(lastBlockDrawStartPoint + new Vector3(0, 0.3f, 0), ((MousePos + new Vector3(0, 0.3f, 0)) - (lastBlockDrawStartPoint + new Vector3(0, 0.3f, 0))).normalized, Vector3.Distance(lastBlockDrawStartPoint, MousePos), fenceObstacleMask))
                    {
                        if (redFence == null)
                            DrawRedFence(lastBlockDrawStartPoint);

                        RaycastHit mousePositionHit2;
                        Vector3 mousePos2 = Vector3.zero;

                        if (Physics.Raycast(cast, out mousePositionHit2, Mathf.Infinity, groundLayer))
                        {
                            mousePos2 = mousePositionHit2.point;
                        }

                        CheckingRedFenceLength(redFence, lastBlockDrawStartPoint, mousePos2);
                    }
                    else
                    {
                        ClearRedFence();

                        RaycastHit hit;

                        Physics.Raycast(cast, out hit, Mathf.Infinity, groundLayer);

                        LR.SetPosition(CurrentIndex, hit.point + new Vector3(0, 0.1f, 0));

                        if (LastInstantiated_Collider != null)
                        {
                            Vector3 CurLinePos2 = LR.GetPosition(CurrentIndex);
                            //LastInstantiated_Collider.gameObject.SetActive(true);

                            LastInstantiated_Collider.LookAt(CurLinePos2);

                            if (LastInstantiated_Collider.rotation.y == 0)
                            {
                                //Debug.Log(LastInstantiated_Collider);
                                LastInstantiated_Collider.eulerAngles = new Vector3(LastInstantiated_Collider.rotation.eulerAngles.x, 90, LastInstantiated_Collider.rotation.eulerAngles.z);
                            }

                            LastInstantiated_Collider.localScale = new Vector3(LastInstantiated_Collider.localScale.x, LastInstantiated_Collider.localScale.y, Vector3.Distance(LastInstantiated_Collider.position, CurLinePos2));
                            if (!drawDisplayLine)
                                LastInstantiated_Collider.gameObject.SetActive(true);

                            print(Vector3.Distance(LastInstantiated_Collider.position, CurLinePos2));

                            StageManager.instance.UseFenceGague(Vector3.Distance(LastInstantiated_Collider.position, CurLinePos2) * 1.7f);
                        }

                        BuildFence(LR.GetPosition(CurrentIndex));

                        CurrentIndex++;
                        LR.positionCount = CurrentIndex + 1;
                        LR.SetPosition(CurrentIndex, mousePositionHit.point);

                        if (StageManager.instance.currentFenceGague <= 0)
                            EndDraw();
                    }
                }
            }
            else
            {
                //펜스가 그려지는 거리만큼 이동해 새로운 펜스를 생성함
                if (Distance_SqrMag > drawDistance)
                {
                    RaycastHit mousePositionHit;

                    if (Physics.Raycast(cast, out mousePositionHit, Mathf.Infinity, groundLayer))
                    {
                        MousePos = mousePositionHit.point;
                    }

                    // 장애물이 감지된 경우
                    if (Physics.Raycast(lastFenceDrawStartPoint + new Vector3(0, 0.3f, 0), ((MousePos + new Vector3(0, 0.3f, 0)) - (lastFenceDrawStartPoint + new Vector3(0, 0.3f, 0))).normalized, Vector3.Distance(lastFenceDrawStartPoint, MousePos), fenceObstacleMask))
                    {
                        print("사이에 장애물이 감지되어 펜스를 생성할 수 없습니다.");

                        //빨간색으로 장애물에 걸리는 펜스 그려주기

                        if (redFence == null)
                            DrawRedFence(lastFenceDrawStartPoint);

                        RaycastHit mousePositionHit2;
                        Vector3 mousePos2 = Vector3.zero;

                        if (Physics.Raycast(cast, out mousePositionHit2, Mathf.Infinity, groundLayer))
                        {
                            mousePos2 = mousePositionHit2.point;
                        }

                        CheckingRedFenceLength(redFence, lastFenceDrawStartPoint, mousePos2);
                    }
                    else
                    {
                        ClearRedFence();

                        print("current fence : " + CurrentIndex + "/ dist :" + Distance_SqrMag);

                        RaycastHit hit;

                        Physics.Raycast(cast, out hit, Mathf.Infinity, groundLayer);

                        //그리는 길이가 울타리 2개 길이 이상일 경우 나누어서 그려주기
                        if (Vector3.Distance(lastFenceDrawStartPoint, mousePositionHit.point) > (drawDistance * 1.6f) * 2)
                        {
                            print("울타리 2개 이상 한번에 그리기");

                            var length = Vector3.Distance(lastFenceDrawStartPoint, mousePositionHit.point);

                            print(length + " " + (drawDistance * 1.6f));

                            int drawFenceCount = Mathf.FloorToInt(length / (drawDistance * 1.6f)) + 1;
                            int coordCount = drawFenceCount + 1;

                            Vector3[] coords = new Vector3[coordCount];

                            print("울타리 개수 : " + drawFenceCount);
                            print("점 개수 : " + coordCount);

                            coords[0] = lastFenceDrawStartPoint;
                            coords[coordCount - 1] = mousePositionHit.point + new Vector3(0, 0.1f, 0);

                            for (int j = 1; j < drawFenceCount; j++)
                            {
                                coords[j] = GetNextCoordinate(coords[j - 1], coords[coordCount - 1], drawDistance * 1.6f);
                            }

                            for (int i = 1; i < drawFenceCount; i++)
                            {
                                LR.SetPosition(CurrentIndex, coords[i]);

                                print("점 번호 : " + CurrentIndex + " 위치 : " + coords[i]);
                                if (LastInstantiated_Collider != null)
                                {
                                    Vector3 CurLinePos2 = LR.GetPosition(CurrentIndex);
                                    //LastInstantiated_Collider.gameObject.SetActive(true);

                                    bool lastfence = false;

                                    if (i == drawFenceCount - 1)
                                        lastfence = true;

                                    FixFencePosition(LastInstantiated_Collider.gameObject, CurLinePos2, lastfence);

                                    //LastInstantiated_Collider.gameObject.SetActive(false);
                                    // LastInstantiated_Collider.LookAt(CurLinePos2);

                                    // if (LastInstantiated_Collider.rotation.y == 0)
                                    // {
                                    //     //Debug.Log(LastInstantiated_Collider);
                                    //     LastInstantiated_Collider.eulerAngles = new Vector3(LastInstantiated_Collider.rotation.eulerAngles.x, 90, LastInstantiated_Collider.rotation.eulerAngles.z);
                                    // }

                                    // LastInstantiated_Collider.localScale = new Vector3(LastInstantiated_Collider.localScale.x, LastInstantiated_Collider.localScale.y, Vector3.Distance(LastInstantiated_Collider.position, CurLinePos2) * 1.6f);
                                    // if (!drawDisplayLine)
                                    //     LastInstantiated_Collider.gameObject.SetActive(true);

                                    // print(Vector3.Distance(LastInstantiated_Collider.position, CurLinePos2) * 1.7f);

                                    StageManager.instance.UseFenceGague(Vector3.Distance(LastInstantiated_Collider.position, CurLinePos2) * 1.7f);
                                }

                                BuildFence(LR.GetPosition(CurrentIndex));

                                CurrentIndex++;
                                LR.positionCount = CurrentIndex + 1;
                                LR.SetPosition(CurrentIndex, mousePositionHit.point);
                            }
                        }
                        else
                        {
                            print("작은 울타리 그림 -> 번호 : " + CurrentIndex + " / 길이 : " + Distance_SqrMag);
                            //LR.SetPosition(CurrentIndex, cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z + 10f)));
                            LR.SetPosition(CurrentIndex, hit.point + new Vector3(0, 0.1f, 0));

                            if (LastInstantiated_Collider != null)
                            {
                                Vector3 CurLinePos2 = LR.GetPosition(CurrentIndex);
                                //LastInstantiated_Collider.gameObject.SetActive(true);

                                LastInstantiated_Collider.LookAt(CurLinePos2);

                                if (LastInstantiated_Collider.rotation.y == 0)
                                {
                                    //Debug.Log(LastInstantiated_Collider);
                                    LastInstantiated_Collider.eulerAngles = new Vector3(LastInstantiated_Collider.rotation.eulerAngles.x, 90, LastInstantiated_Collider.rotation.eulerAngles.z);
                                }

                                LastInstantiated_Collider.localScale = new Vector3(LastInstantiated_Collider.localScale.x, LastInstantiated_Collider.localScale.y, Vector3.Distance(LastInstantiated_Collider.position, CurLinePos2) * 1.6f);
                                if (!drawDisplayLine)
                                    LastInstantiated_Collider.gameObject.SetActive(true);

                                print(Vector3.Distance(LastInstantiated_Collider.position, CurLinePos2) * 1.7f);

                                StageManager.instance.UseFenceGague(Vector3.Distance(LastInstantiated_Collider.position, CurLinePos2) * 1.7f);
                            }

                            BuildFence(LR.GetPosition(CurrentIndex));

                            CurrentIndex++;
                            LR.positionCount = CurrentIndex + 1;
                            LR.SetPosition(CurrentIndex, mousePositionHit.point);
                        }

                        if (StageManager.instance.currentFenceGague <= 0)
                            EndDraw();
                        //}

                        SoundManager.instance.GenerateAudioAndPlaySFX("fence", Vector3.zero);
                    }
                }
            }
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (StartDrawing)
            return;

        try
        {
            if (GameManager.gameStart || !GameManager.enableDrawFence)
                return;

            Ray cast = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Mouse의 포지션을 Ray cast 로 변환

            RaycastHit hit;

            if (Physics.Raycast(cast, out hit, Mathf.Infinity, groundLayer))
            {
                StartDrawing = true;

                MousePos = hit.point;
                lastFenceDrawStartPoint = hit.point;
                lastBlockDrawStartPoint = hit.point;

                if (LineGO.GetComponent<LineRenderer>() != null)
                    Destroy(LineGO.GetComponent<LineRenderer>());

                LR = LineGO.AddComponent<LineRenderer>();

                LR.enabled = false;

                LR.startWidth = 0.1f;

                LR.material = LineMat;

                if (displayLineParent.gameObject.GetComponent<LineRenderer>() != null)
                    Destroy(displayLineParent.gameObject.GetComponent<LineRenderer>());

                displayLR = displayLineParent.gameObject.AddComponent<LineRenderer>();

                if (!drawDisplayLine)
                    displayLR.enabled = false;

                displayLR.startWidth = 0.1f;

                displayLR.material = LineMat;

                BuildFence(hit.point + new Vector3(0, 0.1f, 0));

                print("Start Drawing");

                OnDrawStartEvent.Invoke();
            }
        }
        catch (Exception ex)
        {
            //Get a StackTrace object for the exception
            StackTrace st = new StackTrace(ex, true);

            //Get the first stack frame
            StackFrame frame = st.GetFrame(0);

            //Get the file name
            string fileName = frame.GetFileName();

            //Get the method name
            string methodName = frame.GetMethod().Name;

            //Get the line number from the stack frame
            int line = frame.GetFileLineNumber();

            //Get the column number
            int col = frame.GetFileColumnNumber();

            UnityEngine.Debug.LogError(line + " " + col + " " + frame + " " + fileName + " " + methodName);
        }
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        EndDraw();
    }

    /// <summary>
    /// 펜스 그리는 함수
    /// </summary>
    private void BuildFence(Vector3 positon)
    {
        if (drawBlockWall)
        {
            LastInstantiated_Collider = Instantiate(fence_blockPrefab, positon, Quaternion.identity, LineGO.transform);
            lastBlockDrawStartPoint = positon;
        }
        else
        {
            LastInstantiated_Collider = Instantiate(fencePrefab, positon, Quaternion.identity, LineGO.transform);
            lastFenceDrawStartPoint = positon;
        }


        if (!drawDisplayLine)
            LastInstantiated_Collider.gameObject.SetActive(false);

        generatedFence.Add(LastInstantiated_Collider.gameObject);

        MoreMountains.NiceVibrations.MMVibrationManager.Haptic(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
    }

    private List<GameObject> redFence = new List<GameObject>();
    [SerializeField] private GameObject redFencePrefab;
    [SerializeField] private GameObject redBlockPrefab;


    //빨간 펜스 생성
    private void DrawRedFence(Vector3 position)
    {
        if (drawBlockWall)
            redFence.Add(Instantiate(redBlockPrefab, position, Quaternion.identity, LineGO.transform));
        else
            redFence.Add(Instantiate(redFencePrefab, position, Quaternion.identity, LineGO.transform));
    }

    /// <summary>
    /// 빨간 울타리 개수, 위치, 각도, 크기 조정 함수
    /// <summary>
    private void CheckingRedFenceLength(List<GameObject> fence, Vector3 startPoint, Vector3 endPoint)
    {
        float length = Vector3.Distance(startPoint, endPoint);

        int fenceCount = 1;
        int coordCount = 2;

        if (length > drawDistance * 1.6f)
        {
            fenceCount = Mathf.FloorToInt(length / (drawDistance * 1.6f)) + 1;
            coordCount = fenceCount + 1;
        }

        Vector3[] coords = new Vector3[coordCount];

        coords[0] = startPoint;
        coords[coordCount - 1] = endPoint;

        //좌표 위치 배열에 넣기
        if (coordCount > 2)
        {
            for (int i = 1; i < coordCount; i++)
            {
                coords[i] = GetNextCoordinate(coords[i - 1], coords[coordCount - 1], drawDistance * 1.6f);
            }
        }

        // List fence의 개수와 fenceCount의 개수를 비교해 개수가 다르면 기존의 울타리를 전부 삭제하고 새로 생성함
        // 개수가 같다면 기존의 울타리의 위치와 각도, 크기를 조정함
        if (fence.Count != fenceCount)
        {
            for (int i = 0; i < fence.Count; i++)
            {
                Destroy(fence[i]);
            }
            fence.Clear();

            for (int i = 0; i < fenceCount; i++)
            {
                GameObject newFence;
                if (drawBlockWall)
                    newFence = Instantiate(redBlockPrefab, coords[i], Quaternion.identity, LineGO.transform);
                else
                    newFence = Instantiate(redFencePrefab, coords[i], Quaternion.identity, LineGO.transform);

                fence.Add(newFence);

                bool lastfence = false;

                if (i == fenceCount - 1)
                    lastfence = true;

                FixFencePosition(newFence, coords[i + 1], lastfence);
            }
        }
        else
        {
            for (int i = 0; i < fence.Count; i++)
            {
                bool lastfence = false;

                if (i == fenceCount - 1)
                    lastfence = true;

                fence[i].transform.position = coords[i];
                FixFencePosition(fence[i], coords[i + 1], lastfence);
            }
        }
    }

    /// <summary>
    /// 울타리의 다음 좌표 구하기
    /// <summary>
    private Vector3 GetNextCoordinate(Vector3 start, Vector3 end, float distance)
    {
        return start + (end - start).normalized * distance;
    }

    /// <summary>
    /// 울타리 수정
    /// <summary>
    private void FixFencePosition(GameObject fence, Vector3 nextCoord, bool lastfence = false)
    {
        if (LastInstantiated_Collider != null)
            LastInstantiated_Collider.gameObject.SetActive(false);

        fence.SetActive(true);

        fence.transform.LookAt(new Vector3(nextCoord.x, fence.transform.position.y, nextCoord.z));

        if (fence.transform.rotation.y == 0)
        {
            fence.transform.eulerAngles = new Vector3(fence.transform.rotation.eulerAngles.x, 90, fence.transform.rotation.eulerAngles.z);
        }

        if (lastfence)
        {
            Ray cast = cam.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            Vector3 mousePos = Vector3.zero;

            if (Physics.Raycast(cast, out hit, Mathf.Infinity, groundLayer))
            {
                mousePos = hit.point;
            }

            if (drawBlockWall)
                fence.transform.localScale = new Vector3(fence.transform.localScale.x, fence.transform.localScale.y, Vector3.Distance(fence.transform.position, mousePos));
            else
                fence.transform.localScale = new Vector3(fence.transform.localScale.x, fence.transform.localScale.y, Vector3.Distance(fence.transform.position, mousePos));
            print(Vector3.Distance(fence.transform.position, nextCoord) * 1.6f);
        }
        else
        {
            print(Vector3.Distance(fence.transform.position, nextCoord) * 1.6f);
            if (drawBlockWall)
                fence.transform.localScale = new Vector3(fence.transform.localScale.x, fence.transform.localScale.y, Vector3.Distance(fence.transform.position, nextCoord));
            else
                fence.transform.localScale = new Vector3(fence.transform.localScale.x, fence.transform.localScale.y, Vector3.Distance(fence.transform.position, nextCoord) * 1.6f);
        }
    }

    /// <summary>
    /// 빨간색 펜스 정리
    /// </summary>
    private void ClearRedFence()
    {
        if (redFence.Count > 0)
        {
            for (int i = 0; i < redFence.Count; i++)
            {
                Destroy(redFence[i]);
            }
            redFence.Clear();
        }
    }

    /// <sumamry>
    /// 울타리 그리기 완료후 정리작업
    /// <summary>
    private void EndDraw()
    {
        if (StartDrawing)
        {
            StartDrawing = false;

            if (LineGO.GetComponent<Rigidbody>() != null)
                Destroy(LineGO.GetComponent<Rigidbody>());

            // 리지드바디 생성후 기능 추가 
            Rigidbody rb = LineGO.AddComponent<Rigidbody>();
            rb.mass = 2;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            LR.useWorldSpace = false;

            if (LastInstantiated_Collider != null)
            {
                if (!LastInstantiated_Collider.gameObject.activeSelf)
                    Destroy(LastInstantiated_Collider.gameObject);

                print(LastInstantiated_Collider.localScale.z);

                // 레드펜스가 있으면 장애물이 막고 있으므로 울타리 게이지를 소모하지 않도록 함
                if (redFence.Count == 0)
                    StageManager.instance.UseFenceGague(LastInstantiated_Collider.localScale.z);
            }

            LastInstantiated_Collider = null;

            StageManager.instance.drawedFence = LineGO;

            LineGO = new GameObject();

            CurrentIndex = 0;
            displayLRCurrentIndex = 0;

            Destroy(displayLR);

            for (int i = 0; i < generatedFence.Count; i++)
            {
                generatedFence[i].SetActive(true);
                if (!drawBlockWall)
                    generatedFence[i].GetComponentInChildren<MeshRenderer>().material = defaultMat;
                generatedFence[i].GetComponentInChildren<Collider>().enabled = true;
            }

            generatedFence.Clear();
            LR.enabled = false;

            // 스테이지 시작
            StageManager.instance.StageStart();

            ClearRedFence();

            OnDrawEndEvent.Invoke();
        }
    }
}