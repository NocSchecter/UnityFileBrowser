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

    //UI
    public GameObject prefab;
    public GameObject folderPanel;
    public GameObject filePanel;
    public ScrollRect scrollRect;
    public TextMeshProUGUI textPath;
    private bool scrolling;

    //Hard drives
    private string[] hardDrive;
    private List<string> directories;
    private string currentPath;
    private List<string> files;
    public string currentFile;
    public List<string> extensions;
    private bool selectDrive;

    //Events
    public delegate void FileSelectedEventHandler(string path);
    public static event FileSelectedEventHandler FileSelected;

    public void Up()
    {
        if (currentPath == Path.GetPathRoot(currentPath))
        {
            selectDrive = true;
            ClearFolderContent();
            ClearFileContent();
            Build();
        }
        else
        {
            currentPath = Directory.GetParent(currentPath).FullName;

            ClearFolderContent();
            ClearFileContent();
            Build();
        }

        textPath.text = currentPath;
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
            if (extensions.Contains(Path.GetExtension(file)))
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
        for (int i = 0; i < directories.Count; i++)
        {
            AddDirectoryItem(i);
            yield return null;
        }
    }

    IEnumerator RefreshFiles()
    {
        for (int i = 0; i < files.Count; i++)
        {
            AddFileItem(i);
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

        if (selectDrive)
            item.GetComponentInChildren<TextMeshProUGUI>().text = directories[index];
        else
            item.GetComponentInChildren<TextMeshProUGUI>().text = Path.GetFileName(directories[index]);

        item.transform.SetParent(folderPanel.transform, false);

    }

    private void OnDirectorySelected(int index)
    {
        if (selectDrive)
        {
            currentPath = hardDrive[index];
            selectDrive = false;
        }
        else
        {
            currentPath = directories[index];
        }

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

    //Metodos de incializacion
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

        if (selected != null)
        {
            if (selected.transform.IsChildOf(transform))
            {
                if (Input.GetKeyDown(KeyCode.Joystick1Button1) || Input.GetKeyDown(KeyCode.Escape))
                    //Up();

                    if (Mathf.Abs(Input.GetAxis("Vertical")) > .3f)
                    {
                        if (selected.transform.IsChildOf(transform))
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
                    }
                    else if (scrolling)
                    {
                        if (scrollRect.verticalNormalizedPosition > .99f || scrollRect.verticalNormalizedPosition < .01f)
                            scrollRect.StopMovement();
                        scrolling = false;
                    }
            }
        }
    }
}
