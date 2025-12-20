using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using ZPackage;

public class LECanvas : MonoBehaviour
{
    public Button CreateLevelButton;
    public Button LoadLevelButton;
    public Button SaveLevelButton;
    public Button BeforeButton;
    public Button NextButton;
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
        ButtonsListener();
    }

    private void ButtonsListener()
    {
        CreateLevelButton.onClick.AddListener(() =>
        {
            LevelEditor.CreateLevel();
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
    }

    // Update is called once per frame
    void Update()
    {

    }
}
