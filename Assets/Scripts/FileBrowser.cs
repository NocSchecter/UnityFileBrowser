using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class FileBrowser : MonoBehaviour
{
    #region UI

    public GameObject prefab;
    public GameObject folderPanel;
    public GameObject filePanel;
    public ScrollRect scrollRect;
    public TextMeshProUGUI textPath;
    private bool scrolling;

    #endregion

    #region HARD DRIVES

    private string[] hardDrive;
    private List<string> directories;
    [SerializeField] private string currentPath;
    private List<string> files;
    public string currentFile;
    public List<string> extensions;
    private bool selectDrive;

    #endregion

    #region EVENTS

    public delegate void FileSelectedEventHandler(string path);
    public static event FileSelectedEventHandler FileSelected;

    #endregion

    #region FILE SYSTEM METHODS

    public void Up()
    {
        if(currentPath == string.Empty)
            return;

        if (currentPath == Path.GetPathRoot(currentPath))
        {
            selectDrive = true;

            ClearFolderContent();
            ClearFileContent();
            Build();

            textPath.text = "";
        }
        else
        {
            currentPath = Directory.GetParent(currentPath)?.FullName;

            ClearFolderContent();
            ClearFileContent();
            Build();
        }

        textPath.text = currentPath;
    }

    public void GoToRoot()
    {
        string rootPath = Path.GetPathRoot(currentPath);
        if (rootPath != null && rootPath != "")
        {
            selectDrive = true;
            currentPath = "";

            ClearFolderContent();
            ClearFileContent();
            Build();

            textPath.text = currentPath;
        }
    }

    private void Build()
    {
        directories.Clear();
        files.Clear();

        if (selectDrive)
        {
            directories.AddRange(hardDrive);
            StopAllCoroutines();
            StartCoroutine(RefreshDirectories());
            return;
        }

        try
        {
            directories.AddRange(Directory.GetDirectories(currentPath));
            AddFilesFromCurrentDirectory();
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }

        StopAllCoroutines();
        StartCoroutine(RefreshFiles());
        StartCoroutine(RefreshDirectories());
    }

    private void AddFilesFromCurrentDirectory()
    {
        foreach (string file in Directory.GetFiles(currentPath))
        {
            if (extensions.Contains(Path.GetExtension(file).ToLower()))
                files.Add(file);
        }
    }

    //Metodos para reajustar los directoios y archivos
    private void ClearFolderContent()
    {
        Button[] children = folderPanel.GetComponentsInChildren<Button>();
        foreach (Button child in children)
            Destroy(child.gameObject);

    }

    private void ClearFileContent()
    {
        Button[] children = filePanel.GetComponentsInChildren<Button>();
        foreach (Button child in children)
            Destroy(child.gameObject);
    }

    IEnumerator RefreshDirectories()
    {
        int count = directories.Count;
        List<int> directoryIndices = new List<int>();

        for (int i = 0; i < count; i++)
            directoryIndices.Add(i);

        foreach (int index in directoryIndices)
        {
            AddDirectoryItem(index);
            yield return null;
        }
    }

    IEnumerator RefreshFiles()
    {
        int count = files.Count;
        List<int> fileIndeces = new List<int>();

        for (int i = 0; i < count; i++)
            fileIndeces.Add(i);

        foreach (int index in fileIndeces)
        {
            AddFileItem(index);
            yield return null;
        }
    }

    // Metodos para los directorios
    private void AddDirectoryItem(int index)
    {
        GameObject item = Instantiate(prefab);

        Button button = item.GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            OnDirectorySelected(index);
        });

        string textToShow = selectDrive ? directories[index] : Path.GetFileName(directories[index]);
        item.GetComponentInChildren<TextMeshProUGUI>().text = textToShow;

        item.transform.SetParent(folderPanel.transform, false);

    }

    private void OnDirectorySelected(int index)
    {
        currentPath = selectDrive ? hardDrive[index] : directories[index];
        selectDrive = false;

        textPath.text = currentPath;

        ClearFolderContent();
        ClearFileContent();
        Build();
    }

    //Metodos para los archivos
    private void AddFileItem(int index)
    {
        GameObject item = Instantiate(prefab);

        Button button = item.GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            OnFileSelected(index);
        });

        item.GetComponentInChildren<TextMeshProUGUI>().text = Path.GetFileName(files[index]);
        item.transform.SetParent(filePanel.transform, false);
    }

    private void OnFileSelected(int index)
    {
        string path = files[index];

        if (FileSelected != null)
            FileSelected.Invoke(path);

        currentFile = path;
    }

    #endregion

    #region UNITY EVENTS

    private void Awake()
    {
        directories = new List<string>();
        files = new List<string>();

        hardDrive = Directory.GetLogicalDrives();

        selectDrive = (string.IsNullOrEmpty(currentPath) || !Directory.Exists(currentPath));

        Build();
    }

    private void Update()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        scrollRect.movementType = ScrollRect.MovementType.Elastic;

        if (selected != null && selected.transform.IsChildOf(transform))
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button1) || Input.GetKeyDown(KeyCode.Escape))
            {
                if (Mathf.Abs(Input.GetAxis("Vertical")) > .3f)
                    ScrollIfNecessary(selected);
                else
                    StopScrolling();
            }
        }
    }

    #endregion

    #region UI UPDATE

    void ScrollIfNecessary(GameObject selected)
    {
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        RectTransform rt = selected.GetComponent<RectTransform>();
        Vector2 dif = scrollRect.transform.position - rt.position;

        if (Mathf.Abs(dif.y) > .5f)
        {
            Vector2 scrollVelocity = Vector2.zero;
            scrollVelocity.y = dif.y * 3;
            scrollRect.velocity = scrollVelocity;
        }

        scrolling = true;
    }

    void StopScrolling()
    {
        if (scrolling && (scrollRect.verticalNormalizedPosition > .99f || scrollRect.verticalNormalizedPosition < .01f))
            scrollRect.StopMovement();

        scrolling = false;
    }

    #endregion
}
