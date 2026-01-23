using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZPackage;
using ZPackage.Helper;
using Random = UnityEngine.Random;
using UnityEngine.Pool;
using ZPackage.Utility;
using System.Linq;
using Dreamteck.Splines;
using UnityEngine.InputSystem;

public class Player : Mb
{
    GridNode lastSelectedSlot;
    public List<GridNode> selectedSlots;
    SplineComputer computer;
    [SerializeField] GameObject ConnectPF;
    Camera cam;
    LayerMask rayMask;
    GridController gridController;
    PieceController pieceController;
    RaycastHit hit;
    Ray ray;

    public Piece selectedPiece;
    bool piecePlaceChecked = false;
    private float pressStartTime;
    [SerializeField] bool isDragging = false;
    [SerializeField] bool pressed = false;
    public float holdTime = 0.1f;    // how long before it becomes a "hold"
    float moveHeight = 0.5f;     // height while dragging
    private static InputAction fKeyAction;
    public static InputAction FKeyAction
    {
        get
        {
            if (fKeyAction == null)
            {
                fKeyAction = InputSystem.actions.FindAction("F");
                fKeyAction.Enable();
            }
            return fKeyAction;
        }
    }
    private static InputAction mouse2Action;
    public static InputAction Mouse2Action
    {
        get
        {
            if (mouse2Action == null)
            {
                mouse2Action = InputSystem.actions.FindAction("Mouse2");
                mouse2Action.Enable();
            }
            return mouse2Action;
        }
    }
    private static InputAction bKeyAction;
    public static InputAction BKeyAction
    {
        get
        {
            if (bKeyAction == null)
            {
                bKeyAction = InputSystem.actions.FindAction("B");
                bKeyAction.Enable();
            }
            return bKeyAction;
        }
    }
    private static InputAction testGridAction;
    public static InputAction TestGridAction
    {
        get
        {
            if (testGridAction == null)
            {
                testGridAction = InputSystem.actions.FindAction("TestGrid");
                testGridAction.Enable();
            }
            return testGridAction;
        }
    }
    void OnEnable()
    {
        cam = FindFirstObjectByType<Camera>();
    }

    void Start()
    {
        gridController = FindAnyObjectByType<GridController>();
        pieceController = FindAnyObjectByType<PieceController>();
        // cam = Camera.main;
        rayMask = LayerMask.GetMask("Piece");
        InitializeKeyActions();
        // computer = Instantiate(ConnectPF, transform.position, Quaternion.identity).GetComponent<SplineComputer>();
    }

    private void InitializeKeyActions()
    {
        var input = FindAnyObjectByType<PlayerInput>();
        input.SwitchCurrentActionMap("Player");
        FKeyAction.performed += ctx =>
        {
            Debug.Log("F key pressed - CheckConnected called");
            Z.LS.CurrentLevel.CheckColumnsRows();
        };
        Mouse2Action.performed += ctx =>
        {
            Debug.Log("Mouse2 button pressed - ClearLine called");
            Vector3 mousePos = MP;
            mousePos.z = cam.transform.position.y;
            Vector3 worldMouse = cam.ScreenToWorldPoint(mousePos).SwitchYZ();
            Z.LS.CurrentLevel.FillSlot(worldMouse);
        };
        BKeyAction.performed += ctx =>
        {
            Debug.Log("B key pressed - ClearCurrentPath called");
            Vector3 mousePos = MP;
            mousePos.z = cam.transform.position.y;
            Vector3 worldMouse = cam.ScreenToWorldPoint(mousePos).SwitchYZ();
            Z.LS.CurrentLevel.Block(worldMouse);
        };
        //Q
        TestGridAction.performed += ctx =>
        {
            Debug.Log("Test Grid key pressed");
            Vector3 mousePos = MP;
            mousePos.z = cam.transform.position.y;
            Vector3 worldMouse = cam.ScreenToWorldPoint(mousePos).SwitchYZ();
            Z.LS.CurrentLevel.TestGrid(worldMouse);
        };
    }
    float silhTime = 0;
    // Update is called once per frame
    void Update()
    {
        if (IsPlaying)
        {
            // Old();
            if (IsDown)
            {
                pressed = true;
                pressStartTime = Time.time;

                // Raycast to find object
                ray = cam.ScreenPointToRay(MP);
                if (Physics.Raycast(ray, out hit, 30, rayMask))
                {
                    if (hit.collider.attachedRigidbody != null && hit.collider.attachedRigidbody.GetComponent<Piece>())
                    {

                        selectedPiece = hit.collider.attachedRigidbody.GetComponent<Piece>();
                    }
                }
            }
            // Holding logic
            else if (pressed && selectedPiece != null)
            {
                float pressedDuration = Time.time - pressStartTime;

                // Once hold threshold passed → begin dragging
                if (!isDragging && pressedDuration > holdTime)
                {
                    isDragging = true;
                    pressed = false;
                    selectedPiece.StartDrag(true);
                }
                // if (isDragging)
                // {

                // }
            }
            else if (isDragging && selectedPiece != null)
            {
                ray = cam.ScreenPointToRay(MP);
                Plane ground = new Plane(Vector3.up, Vector3.zero);

                if (ground.Raycast(ray, out float dist))
                {
                    Vector3 point = ray.GetPoint(dist);
                    point.y = moveHeight;  // floating while dragging
                    selectedPiece.transform.position = point;
                }
                float lastSilh = Time.time - silhTime;
                if (lastSilh > 0.5f && gridController.IsPlaceAble(selectedPiece, out List<GridNode> freeSlots))
                {
                    silhTime = Time.time;
                    GameObject silh = selectedPiece.GetSilhoutte();
                    PlaceSilh(freeSlots, silh);
                    // silh.SetActive(true);
                    // for (int i = 0; i < silh.transform.childCount; i++)
                    // {
                    //     silh.transform.GetChild(i).transform.position = freeSlots[i].transform.position;
                    // }
                }
                // if (!piecePlaceChecked)
                // {
                //     piecePlaceChecked = true;
                //     if (gridController.IsPlaceAbleSomeWhere(selectedPiece, out List<GridNode> placeAbleNodes))
                //     {

                //         GameObject silh = selectedPiece.GetSilhoutte();
                //         silh.SetActive(true);
                //         for (int i = 0; i < silh.transform.childCount; i++)
                //         {
                //             silh.transform.GetChild(i).transform.position = placeAbleNodes[i].transform.position;
                //         }

                //     }
                // }
            }
            if (IsUp)
            {
                // Debug.Log("up");
                if (selectedPiece != null)
                {
                    // // float pressedDuration = Time.time - pressStartTime;

                    // // if (!isDragging && pressedDuration < holdTime)
                    // if (!isDragging)
                    // {
                    //     // TAP → ROTATE
                    //     selectedPiece.GetComponent<Piece>().Rotate();
                    // }
                    // else 
                    if (isDragging)
                    {
                        if (gridController.IsPlaceAble(selectedPiece, out List<GridNode> freeSlots))
                        {
                            gridController.Place(selectedPiece, freeSlots);
                            Z.LS.CurrentLevel.CheckColumnsRows();
                            pieceController.NotifyPlaced(selectedPiece);
                            Destroy(selectedPiece.gameObject);
                        }
                        else
                        {
                            PlaceObject();
                        }
                    }
                }
                // ClearLine();
                pressed = false;
                isDragging = false;
                selectedPiece = null;
                piecePlaceChecked = false;
            }
            if (IsDownMouse2)
            {
                // gridController.Grid.getwo
            }
            if (Time.time - lastPlaceTime > 10 && lastSuggestTime + 5 < Time.time)
            {
                lastSuggestTime = Time.time;
                List<Piece> pieces = pieceController.GetPieces();
                foreach (var item in pieces)
                {
                    if (gridController.IsPlaceAbleSomeWhere(item, out List<GridNode> placeAbleNodes))
                    {
                        GameObject silh = item.GetSilhoutte();
                        PlaceSilh(placeAbleNodes, silh);
                        item.HideSilhoutteAfterDelay(2f);
                    }
                }
            }
        }
    }

    private void PlaceSilh(List<GridNode> placeAbleNodes, GameObject silh)
    {
        silh.SetActive(true);
        for (int i = 0; i < silh.transform.childCount; i++)
        {
            silh.transform.GetChild(i).transform.position = placeAbleNodes[i].transform.position;
        }
    }

    float lastSuggestTime = 0;
    private void ClearLine()
    {
        lastSelectedSlot = null;
        selectedSlots.Clear();
        computer.SetPoints(new SplinePoint[0]);
        computer.Rebuild();
        // ray = cam.ScreenPointToRay(MP);
        // if (Physics.Raycast(ray, out hit, 30, rayMask))
        // {
        //     if (hit.collider.attachedRigidbody.GetComponent<Piece>())
        //     {
        //         hit.collider.attachedRigidbody.GetComponent<Piece>().Rotate();
        //     }
        // }
    }
    float lastPlaceTime = 0;
    private void PlaceObject()
    {
        selectedPiece.GetSilhoutte().SetActive(false);
        if (pieceController.HasSlot(selectedPiece))
        {
            pieceController.GotoSlot(selectedPiece);
        }
        lastPlaceTime = Time.time;
        // // Snap to ground
        // Vector3 pos = selectedObject.transform.position;
        // pos.y = 0;
        // selectedObject.transform.position = pos;
    }

    private void drawLine()
    {
        Vector3 mousePos = MP;
        mousePos.z = cam.transform.position.y;
        Vector3 worldMouse = cam.ScreenToWorldPoint(mousePos).SwitchYZ();
        // print(worldMouse);
        GridNode foundSlot = gridController.Grid.GetGridObject(worldMouse);
        if (foundSlot != lastSelectedSlot && foundSlot != null && !selectedSlots.Contains(foundSlot))
        {
            if (lastSelectedSlot != null)
            {

                lastSelectedSlot.GetComponent<Renderer>().material.color = Color.gray;
            }
            lastSelectedSlot = foundSlot;
            lastSelectedSlot.GetComponent<Renderer>().material.color = Color.red;
            // Vector3 lastPos = selectedSlots.Count > 0 ? selectedSlots.Last().transform.position : foundSlot.transform.position;
            // Vector3 forward = (foundSlot.transform.position - lastPos).normalized;
            List<SplinePoint> knots = new List<SplinePoint>();
            SplinePoint knot = new SplinePoint(foundSlot.transform.position);
            knots.Add(knot);
            selectedSlots.Add(lastSelectedSlot);
            foreach (var item in knots)
            {
                computer.SetPoint(computer.pointCount, item);
            }
            // computer.Spline = spline;
            computer.GetComponent<SplineMesh>().GetChannel(0).count = computer.pointCount * 4;
            computer.Rebuild();
        }
    }

    private bool isTurn(Slot foundSlot, out Vector3 initialDir, out Vector3 currentDir)
    {
        if (selectedSlots.Count >= 2)
        {
            GridNode lastOne = selectedSlots[^1];
            GridNode lastTwo = selectedSlots[^2];
            initialDir = (lastOne.transform.position - lastTwo.transform.position).normalized;
            currentDir = (foundSlot.transform.position - lastOne.transform.position).normalized;
            if (initialDir != currentDir)
            {
                return true;
            }
        }
        initialDir = Vector3.zero;
        currentDir = Vector3.zero;
        return false;
    }
}
