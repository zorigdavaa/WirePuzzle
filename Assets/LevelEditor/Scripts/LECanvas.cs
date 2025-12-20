using UnityEngine;
using UnityEngine.UI;

public class LECanvas : MonoBehaviour
{
    public Button CreateLevelButton;
    public Button LoadLevelButton;
    public Button SaveLevelButton;
    LevelEditor LevelEditor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LevelEditor = FindAnyObjectByType<LevelEditor>();
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
    }

    // Update is called once per frame
    void Update()
    {

    }
}
