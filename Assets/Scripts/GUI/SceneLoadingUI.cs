﻿/******************************************************************************
 *
 *                      GeaVR
 *                https://www.geavr.eu/
 *             https://github.com/GeaVR/GeaVR
 * 
 * GeaVR is an open source software that allows the user to experience a wide 
 * range of geological and geomorphological sites in immersive virtual reality,
 * including data collection.
 *
 * Main Developers:      
 * 
 *     Fabio Luca Bonali (fabio.bonali@unimib.it)
 *     Martin Kearl (martintkearl@gmail.com)
 *     Fabio Roberto Vitello (fabio.vitello@inaf.it)
 * 
 * Developed thanks to the contribution of following projects:
 *
 *     ACPR15T4_ 00098 “Agreement between the University of Milan Bicocca and the 
 *     Cometa Consortium for the experimentation of cutting-edge interactive 
 *     technologies for the improvement of science teaching and dissemination” of 
 *     Italian Ministry of Education, University and Research (ARGO3D)
 *     PI: Alessandro Tibaldi (alessandro.tibaldi@unimib.it)
 *     
 *     Erasmus+ Key Action 2 2017-1-UK01-KA203- 036719 “3DTeLC – Bringing the  
 *     3D-world into the classroom: a new approach to Teaching, Learning and 
 *     Communicating the science of geohazards in terrestrial and marine 
 *     environments”
 *     PI: Malcolm Whitworth (malcolm.Whitworth@port.ac.uk)
 * 
 ******************************************************************************
 * Copyright (c) 2016-2022
 * GPL-3.0 License
 *****************************************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SharpKml.Dom;
using SharpKml.Engine;
using System.Linq;
using System.Xml;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Globalization;
using SimpleFileBrowser;


/*
* A UI component for the control of terrain loading
*/
public class SceneLoadingUI : MonoBehaviour {


    /* A list of folder paths to terrain data */
    List<string> folderPaths = new List<string>();
    int firstFolderPathIndex = 0;

    /* internal data for the currently selected scene */
    int selectedFolderIndex = -1;
    string currentScenePath;

    private float xStart;
    private float yStart;
    private float zStart;
    private float scalingFactor;

    private Decimal longitude;
    private Decimal latitiude;
    private Decimal altitude;
    private Decimal altOffset;

    /* A reference to an instance of the TerriainLoaderPrefab this UI controls */
    public TerrainLoader terrainLoader;

    /* input field for new folder paths to add to the list of loadable terrains */
    public TMP_InputField TMProInputField;

    /* buttons trigger loading of scenes */
    public GameObject button1;
    public GameObject button2;
    public GameObject button3;
    public GameObject goButton;

    /* input field for GPS locations */
    public TMP_InputField longitudeTextbox;
    public TMP_InputField latitudeTextbox;
    public TMP_InputField altitudeTextbox;
    public TMP_InputField altitudeOffsetTextbox;
    public TMP_InputField modelScaleTextbox;

    /**
    * Initialise component
    */
    void Start()
    {
        LoadSceneListFromFile();
        UpdateButtons();
        Cursor.visible = true;
    }

    /**
    * Update to check for inputs
    */
    void Update()
    {
        Cursor.visible = true;
    }

    /**
    * Set import location and run Terrain Loader
    */
    public void SetSelection(int index)
    {
        // set scene in terrain loader
        if (firstFolderPathIndex + index < folderPaths.Count)
        {
            currentScenePath = folderPaths[firstFolderPathIndex + index].TrimEnd('\r', '\n');
            selectedFolderIndex = firstFolderPathIndex + index;
            UpdateButtons();
            LoadSceneData();

            Debug.Log(currentScenePath);
        }
    }

    /**
    * Set import location and run Terrain Loader
    */
    public void SetAndLoadScene()
    {
        Cursor.visible = false;

        // set scene in terrain loader
        if (selectedFolderIndex != -1 && selectedFolderIndex < folderPaths.Count)
        {
            // set static value
            TerrainLoader.importLocation = folderPaths[selectedFolderIndex].TrimEnd('\r', '\n');

            Debug.Log(TerrainLoader.importLocation);

            // run terrain loader - will remove current terrain
            StartCoroutine("SceneLoader");


            // disable instances of this component
            SceneLoadingUI[] foundCanvasObjects = FindObjectsOfType<SceneLoadingUI>();
            foreach (var sceneUI in foundCanvasObjects)
            {
                sceneUI.gameObject.SetActive(false);
            }
        }
    }

	IEnumerator SceneLoader()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        //LoadingObj.gameObject.SetActive(true);
        while (!operation.isDone)
        {
            //LoadingObj.text = "Caricamento al " + (operation.progress * 100).ToString("00.00") + "%";
            yield return new WaitForEndOfFrame();
        }
    }
    /**
    * Update buttons to show a slice of the folderPaths List
    */
    public void UpdateButtons()
    {
        // hide/show if needed
        button1.GetComponent<Image>().color = Color.white;
        button2.GetComponent<Image>().color = Color.white;
        button3.GetComponent<Image>().color = Color.white;

        // button 1
        if (firstFolderPathIndex < folderPaths.Count)
        {
            button1.SetActive(true);
            button1.GetComponentInChildren<Text>().text = folderPaths[firstFolderPathIndex].TrimEnd('\r', '\n');

            if (selectedFolderIndex != -1 && selectedFolderIndex - firstFolderPathIndex == 0)
            {
                button1.GetComponent<Image>().color = Color.green;
            }
        }
        else
        {
            button1.SetActive(false);
        }

        // button 2
        if (firstFolderPathIndex + 1 < folderPaths.Count)
        {
            button2.SetActive(true);
            button2.GetComponentInChildren<Text>().text = folderPaths[firstFolderPathIndex + 1].TrimEnd('\r', '\n');

            if (selectedFolderIndex != -1 && selectedFolderIndex - firstFolderPathIndex == 1)
            {
                button2.GetComponent<Image>().color = Color.green;
            }
        }
        else
        {
            button2.SetActive(false);
        }

        // button 3
        if (firstFolderPathIndex + 2 < folderPaths.Count)
        {
            button3.SetActive(true);
            button3.GetComponentInChildren<Text>().text = folderPaths[firstFolderPathIndex + 2].TrimEnd('\r', '\n');

            if (selectedFolderIndex != -1 && selectedFolderIndex - firstFolderPathIndex == 2)
            {
                button3.GetComponent<Image>().color = Color.green;
            }
        }
        else
        {
            button3.SetActive(false);
        }

        if (selectedFolderIndex != -1 && goButton != null)
        {
            goButton.GetComponent<Image>().color = Color.green;
        }
    }

    /**
    * Update buttons to show a slice of the folderPaths List
    */
    public void SelectionUp()
    {
        firstFolderPathIndex = Math.Max(0, firstFolderPathIndex - 1); // clamp to 0
        UpdateButtons();
    }

    /**
    * Update buttons to show a slice of the folderPaths List
    */
    public void SelectionDown()
    {
        firstFolderPathIndex = Math.Min(folderPaths.Count, firstFolderPathIndex + 1); // clamp to 0
        UpdateButtons();
    }


    public void BrowseFileSystem()
    {
        StartCoroutine(ShowLoadDialogCoroutine());

    }
    IEnumerator ShowLoadDialogCoroutine()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: file, Allow multiple selection: true
        // Initial path: default (Documents), Title: "Load File", submit button text: "Load"
        yield return FileBrowser.WaitForLoadDialog(true, null, "Select Scene Folder", "Select");

        if (FileBrowser.Success)
        {
            TMProInputField.text = FileBrowser.Result;
        }
    }

    /**
    * Paster from clipboard into textfield
    */
    public void PasteFromClipboard()
    {
        string path = GUIUtility.systemCopyBuffer;
        path = path.TrimEnd('\r', '\n');

        TMProInputField.text = @path;
    }

    /**
    * Add path in text field to List of folder paths
    */
    public void AddNewScene()
    {
        string text = TMProInputField.text;

        //remove placeholder note
        if (folderPaths.Contains("Please add folder locations to sceneList.txt located in the program directory"))
        {
            folderPaths.Remove("Please add folder locations to sceneList.txt located in the program directory");
        }

        if (folderPaths.Contains(text) == false && text != "")
        {
            folderPaths.Add(text);
            SaveSceneListToFile();

            firstFolderPathIndex = Math.Max(0, folderPaths.Count-3); 
            UpdateButtons();
        }
    }

    /**
    * save current FolderPaths List to external file, the file will be saved relative to the executable
    */
    public void SaveSceneListToFile()
    {
        //string directory = Application.persistentDataPath;
        var directory = new DirectoryInfo(Application.dataPath);
        var filePath = Path.Combine(directory.Parent.FullName, "sceneList.txt");

        //remove placeholder note
        if (folderPaths.Contains("Please add folder locations to sceneList.txt located in the program directory"))
        {
            folderPaths.Remove("Please add folder locations to sceneList.txt located in the program directory");
        }

        string output = "";

        foreach (string scene in folderPaths)
        {
            output = output + scene + "\n";
        }

        output = output.TrimEnd('\r', '\n');

        System.IO.File.WriteAllText(filePath, output);
    }

    /**
    * Load from external file into FolderPaths List, the file will be loaded relative to the executable
    */
    public void LoadSceneListFromFile()
    {
        //string directory = Application.persistentDataPath;
        var directory = new DirectoryInfo(Application.dataPath);
        var filePath = Path.Combine(directory.Parent.FullName, "sceneList.txt");

        if (File.Exists(filePath))
        {
            String fileData = System.IO.File.ReadAllText(filePath);
            String[] lines = fileData.Split("\n"[0]);

            folderPaths = new List<string>(lines);

            if (folderPaths.Count == 1)
            {
                if (folderPaths[0] == "")
                {
                    folderPaths[0] = "Please add folder locations to sceneList.txt located in the program directory";
                }
            }
        }
        else
        {
            SaveSceneListToFile();
            folderPaths.Add("Please add folder locations to sceneList.txt located in the program directory");
        }
    }

    /**
    * Set import location and run Terrain Loader
    */
    public void ClearIndex( int index)
    {
        if (firstFolderPathIndex + index < folderPaths.Count)
        {
            folderPaths.RemoveAt(firstFolderPathIndex + index);
        }
        SaveSceneListToFile();
        ClearSceneData();
        selectedFolderIndex = -1;
        currentScenePath = "";
        UpdateButtons();
    }

    /**
    * Set import location and run Terrain Loader
    */
    public void ClearList()
    {
        //string directory = Application.persistentDataPath;
        var directory = new DirectoryInfo(Application.dataPath);
        var filePath = Path.Combine(directory.Parent.FullName, "sceneList.txt");

        string output = "Please add folder locations to sceneList.txt located in the program directory";

        folderPaths = new List<string>(1);
        folderPaths[0] = output;

        System.IO.File.WriteAllText(filePath, output);

        // update ui
        ClearSceneData();
        selectedFolderIndex = -1;
        currentScenePath = "";
        UpdateButtons();
    }

    /**
    * Set import location and run Terrain Loader
    */
    public void ExitToOS()
    {
#if UNITY_EDITOR
         UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /**
    * Set import location and run Terrain Loader
    */
    public void LoadSceneData()
    {
        ////////////////////////////////////////////////////
        // load kml and xml data for GPS positions
        ////////////////////////////////////////////////////
        Decimal west_dd = 0.0m;
        Decimal east_dd = 0.0m;
        Decimal south_dd = 0.0m;
        Decimal north_dd = 0.0m;

        Decimal west_uu = 0.0m;
        Decimal east_uu = 0.0m;
        Decimal south_uu = 0.0m;
        Decimal north_uu = 0.0m;

        Decimal alt_min_dd = 0.0m;
        Decimal alt_max_dd = 0.0m;
        Decimal alt_min_uu = 0.0m;
        Decimal alt_max_uu = 0.0m;

        Decimal offset_alt = 0m;

        //fv
        // open xml
        Debug.Log("loading doc.kml");
        XmlDocument newKml = new XmlDocument();
        try
        {
            newKml.Load(currentScenePath + "\\doc.kml");
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            newKml = null;
        }

        if (newKml != null)
        {

            Debug.Log("in loading doc.kml");

            XmlNode root = newKml.DocumentElement;
            Debug.Log("root "+ root.Name);
            var nodeLatLonAltBoxt = root.FirstChild.FirstChild.FirstChild;

          
            if (nodeLatLonAltBoxt.HasChildNodes)
            {
                west_dd = Decimal.Parse(nodeLatLonAltBoxt["west"].InnerText, new CultureInfo("en-GB"));
                east_dd = Decimal.Parse(nodeLatLonAltBoxt["east"].InnerText, new CultureInfo("en-GB"));
                south_dd = Decimal.Parse(nodeLatLonAltBoxt["south"].InnerText, new CultureInfo("en-GB"));
                north_dd = Decimal.Parse(nodeLatLonAltBoxt["north"].InnerText, new CultureInfo("en-GB"));

                alt_min_dd = Decimal.Parse(nodeLatLonAltBoxt["minAltitude"].InnerText, new CultureInfo("en-GB"));
                alt_max_dd = Decimal.Parse(nodeLatLonAltBoxt["maxAltitude"].InnerText, new CultureInfo("en-GB"));



            }
        }
        // open xml
        Debug.Log("loading tiles.xml");
        XmlDocument newXml = new XmlDocument();
        try
        {
            newXml.Load(currentScenePath + "\\tiles.xml");
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            newXml = null;
        }

        if (newXml != null)
        {
            XmlNode root = newXml.DocumentElement;
            var nodeList = root.SelectNodes("/document/folder/extent");
            Debug.Log("root " + root.Name);
            Debug.Log("node: " + nodeList.Count);
            if (nodeList.Count > 0)
            {
                west_uu = Decimal.Parse(nodeList[0]["xmin"].InnerText, new CultureInfo("en-GB"));
                east_uu = Decimal.Parse(nodeList[0]["xmax"].InnerText, new CultureInfo("en-GB"));
                south_uu = Decimal.Parse(nodeList[0]["ymin"].InnerText, new CultureInfo("en-GB"));
                north_uu = Decimal.Parse(nodeList[0]["ymax"].InnerText, new CultureInfo("en-GB"));

                alt_min_uu = Decimal.Parse(nodeList[0]["zmin"].InnerText, new CultureInfo("en-GB"));
                alt_max_uu = Decimal.Parse(nodeList[0]["zmax"].InnerText, new CultureInfo("en-GB"));


            }
        }

        // open scene xml
        Debug.Log("loading scene.xml");
        XmlDocument sceneXml = new XmlDocument();
        try
        {
            sceneXml.Load(currentScenePath + "\\scene.xml");
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            sceneXml = null;
        }

        if (sceneXml != null)
        {
            XmlNode root = sceneXml.DocumentElement;
            var nodeList = root.SelectNodes("/document/folder");

            if (nodeList.Count > 0)
            {
                offset_alt = Decimal.Parse(nodeList[0]["offset"].InnerText, new CultureInfo("en-GB"));
                var positionNodes = nodeList[0]["position"];
                xStart = float.Parse(positionNodes["x"].InnerText, new CultureInfo("en-GB"));
                yStart = float.Parse(positionNodes["y"].InnerText, new CultureInfo("en-GB"));
                zStart = float.Parse(positionNodes["z"].InnerText, new CultureInfo("en-GB"));
            }
        }
        // set GPS data
        PositionController.SetStaticGPSLatLonAltBox(
            west_dd,
            east_dd,
            south_dd,
            north_dd,

            west_uu,
            east_uu,
            south_uu,
            north_uu,

            alt_min_dd,
            alt_max_dd,
            alt_min_uu,
            alt_max_uu,

            offset_alt
        );

        Vector3 point = new Vector3(xStart, yStart, zStart);        
        Vector3Decimal GPSvector = PositionController.CalculateGPSPosition(point);

        //FV inverted y and z
        longitudeTextbox.text = GPSvector.x.ToString();
        altitudeTextbox.text = GPSvector.y.ToString();
        latitudeTextbox.text = GPSvector.z.ToString();
        altitudeOffsetTextbox.text = offset_alt.ToString();

        longitude = GPSvector.x;
        altitude = GPSvector.y;
        latitiude = GPSvector.z;
        altOffset = offset_alt;
    }

    /**
    * Set import location and run Terrain Loader
    */
    public void SaveSceneData()
    {
        longitude = Decimal.Parse(longitudeTextbox.text);
        altitude = Decimal.Parse(altitudeTextbox.text);
        latitiude = Decimal.Parse(latitudeTextbox.text);

        // calculate position
        Vector3Decimal point = PositionController.CalculateUnityPosition(new Vector3Decimal(longitude, altitude, latitiude));        
        TerrainLoader.playerStart = (Vector3)point;
    }

    /**
    * Set import location and run Terrain Loader
    */
    public void ClearSceneData()
    {
        longitudeTextbox.text = "0.0000000000";
        altitudeTextbox.text = "0.0000000000";
        latitudeTextbox.text = "0.0000000000";
        altitudeOffsetTextbox.text = "0.0000000000";
        modelScaleTextbox.text = "0.0000000000";

        longitude = 0m;
        altitude = 0m;
        latitiude = 0m;
        altOffset = 0m;
        scalingFactor = 1f;
    }
}
