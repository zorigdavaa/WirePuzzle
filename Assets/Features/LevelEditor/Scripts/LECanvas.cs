using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using ZPackage;

public class LECanvas : GenericSingleton<LECanvas>
{
    public Button CreateLevelButton;
    public Button LoadLevelButton;
    public Button SaveLevelButton;
    public Button BeforeButton;
    public Button NextButton;
    public Button SaveCurrent;
    public Button AddCurrent;
    public Button AddRow;
    public Button AddColumn;
    public Slider XSlider;
    public Slider YSlider;
    LevelEditor LevelEditor;
    public Camera cam;
    void OnEnable()
    {
        cam = Camera.main;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var input = FindAnyObjectByType<PlayerInput>();
        input.SwitchCurrentActionMap("LevelEditor");
        LevelEditor = FindAnyObjectByType<LevelEditor>();
        InputAction qq = InputSystem.actions.FindAction("QQ");
        qq.Enable();
        qq.performed += ctx =>
        {
            Vector3 mousePos = Pointer.current.position.ReadValue();
            mousePos.z = cam.transform.position.y;
            Vector3 worldMouse = cam.ScreenToWorldPoint(mousePos).SwitchYZ();
            GridNode gridNode = Z.GridController.Grid.GetGridObject(worldMouse);
            if (gridNode != null)
            {
                gridNode.Slot.ChangeTypeToNext();
            }
        };
        InputAction XToggle = InputSystem.actions.FindAction("Toggle");
        XToggle.Enable();
        XToggle.performed += ctx =>
        {
            Vector3 mousePos = Pointer.current.position.ReadValue();
            mousePos.z = cam.transform.position.y;
            Vector3 worldMouse = cam.ScreenToWorldPoint(mousePos).SwitchYZ();
            GridNode gridNode = Z.GridController.Grid.GetGridObject(worldMouse);
            if (gridNode != null)
            {
                gridNode.Slot.Toggle();
            }
        };
        ButtonsListener();
    }

    private void ButtonsListener()
    {
        CreateLevelButton.onClick.AddListener(() =>
        {
            LevelEditor.CreateLevel((int)XSlider.value, (int)YSlider.value);
            CreateLevelButton.transform.parent.gameObject.SetActive(false);
        });
        LoadLevelButton.onClick.AddListener(() =>
        {
            LevelEditor.LoadLevel();
            LoadLevelButton.transform.parent.gameObject.SetActive(false);
        });
        SaveLevelButton.onClick.AddListener(() =>
        {
            LevelEditor.SaveLevel();
            SaveLevelButton.transform.parent.gameObject.SetActive(false);
        });
        BeforeButton.onClick.AddListener(() =>
        {
            LevelEditor.Before();
        });
        NextButton.onClick.AddListener(() =>
        {
            LevelEditor.Next();
        });
        SaveCurrent.onClick.AddListener(() =>
        {
            LevelEditor.SaveCurrent();
        });
        AddCurrent.onClick.AddListener(() =>
        {
            LevelEditor.AddCurrent();
        });
        AddRow.onClick.AddListener(() =>
        {
            LevelEditor.AddRow();
        });
        AddColumn.onClick.AddListener(() =>
        {
            LevelEditor.AddColumn();
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
