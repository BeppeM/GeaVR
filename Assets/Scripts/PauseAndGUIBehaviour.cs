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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class PauseAndGUIBehaviour : MonoBehaviour {

    public GameObject[] GuiToShowPause;
    public GameObject[] GuiToShowSettingsMenu;
    public GameObject[] GuiToShowModeMenu;
    public GameObject[] GuiToShowToolMenu;
    public GameObject[] GuiToShowPlacemark;
    public GameObject[] GuiToHide;
    public GameObject[] GuiToHideInMenu;
    public GameObject[] GuiCoordinate;
    public GameObject textToBlink1;
    public GameObject speedTextPanel;
    public GameObject GuiGpsInfoOnMap;
    public GameObject GuiCameraControlUI;
    public GameObject NotificationText;
    public GameObject NotificationPanel;


    public bool isBTCoroutineRunning = false;
    public static bool isPause = false;
    public static bool isSettingsMenu = false;
    public static bool isModeMenu = false;
    public static bool isToolMenu = false;
    public static bool isCoordinate = false;
    public static bool isPlacemark = false;
    public static bool isTopographicGraph = false;
    public static bool isGPS2 = false;
    public static bool isGpsInfoOnMap = false;

    public static bool isGuiCameraControlUI = false;
    

    public bool isMePause = true;
    public bool isMeCoordinate = true;
    public bool isMeSettingsMenu = true;
    public bool isMeModeMenu = true;
    public bool isMeToolMenu = true;
    public bool isMePlacemark = true;
    public bool isMeTopographicGraph = true;
    public bool isMeGpsInfoOnMap = true;
    public static bool isMeGuiCameraControlUI = true;



    public static bool isMeGPS2 = false;

    //public GameObject line;
    public GameObject [] GuiDeactivateForScreenshot;

    public static bool isScreenshot;
    private bool isUsingOculus;

    void Start()
    {
        Cursor.visible = false;
        if (StateSingleton.stateView == StateSingleton.StateView.MODE2D_PLUS_OCULUS || StateSingleton.stateView == StateSingleton.StateView.MODE2D_PLUS_3DVP_PLUS_OCULUS)
        {
            foreach (GameObject gts in GuiToHide)
            {
                gts.SetActive(false);
                    
            }
            isUsingOculus = true;
        }
    }

    // Update is called once per frame
    void Update () {
        if (isPause!=isMePause)
        {
            isMePause = isPause;
            Pause(isMePause);
        }
		
		  if (isGPS2!=isMeGPS2)
        {
            isMeGPS2 = isGPS2;
            Pause(isMePause);
        }


        if(isScreenshot)
        {
            foreach (GameObject gc in GuiDeactivateForScreenshot)
            {
                gc.SetActive(!isScreenshot);
            }

        }      

        if (isCoordinate!=isMeCoordinate)
        {
            isMeCoordinate = isCoordinate;
            foreach (GameObject gc in GuiCoordinate)
            {
                gc.SetActive(isMeCoordinate);
            }
        }

        if (isGpsInfoOnMap != isMeGpsInfoOnMap)
        {
            isMeGpsInfoOnMap = isGpsInfoOnMap;
            //foreach (GameObject gc in GuiGpsInfoOnMap)
            //{
            GuiGpsInfoOnMap.SetActive(isMeGpsInfoOnMap);
            //}
        }

        if (isPlacemark!=isMePlacemark)
        {
            isMePlacemark = isPlacemark;
            Placemark(isMePlacemark);
        }

        if (isSettingsMenu != isMeSettingsMenu)
        {
            isMeSettingsMenu = isSettingsMenu;
            SettingsMenu(isMeSettingsMenu);
        }

        if (isModeMenu !=isMeModeMenu)
        {
            isMeModeMenu = isModeMenu;
            ModeMenu(isMeModeMenu);
        }

        if (isToolMenu != isMeToolMenu)
        {
            isMeToolMenu = isToolMenu;
            ToolMenu(isMeToolMenu);
        }


        if (isGuiCameraControlUI != isMeGuiCameraControlUI)
        {
            isMeGuiCameraControlUI = isGuiCameraControlUI;
            GuiCameraControlUIMenu(isMeGuiCameraControlUI);
        }
     

        if ((Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.Z)) && !isBTCoroutineRunning)
        {
            isBTCoroutineRunning = true;
            StartCoroutine(BlinkText(textToBlink1, KeyCode.X, KeyCode.Z));
        }

        if ((Input.GetKey(KeyCode.N) || Input.GetKey(KeyCode.M)) && !isBTCoroutineRunning)
        {
            isBTCoroutineRunning = true;
            StartCoroutine(BlinkText(speedTextPanel, KeyCode.N, KeyCode.M));
        }
    }
	
	
	public void PauseforScreenshot(bool value)
	{
	}

    public void Pause(bool value)
    {

        Cursor.visible = value;
        if (StateSingleton.stateView!=StateSingleton.StateView.MODE2D_PLUS_OCULUS)
            Time.timeScale = (value) ? 0 : 1;
        foreach (GameObject gts in GuiToShowPause)
            gts.SetActive(value);
        foreach (GameObject gts in GuiToHide)
            gts.SetActive(!value && !isUsingOculus);

   

    }

    public void SettingsMenu(bool value)
    {

        Cursor.visible = value;
        if (StateSingleton.stateView != StateSingleton.StateView.MODE2D_PLUS_OCULUS)
            Time.timeScale = (value) ? 0 : 1;
        foreach (GameObject gts in GuiToHideInMenu)
        {
            gts.SetActive(false && !isUsingOculus);
        }

        foreach (GameObject gts in GuiToShowSettingsMenu)
            gts.SetActive(value);
    }


    public void ModeMenu(bool value)
    {

        Cursor.visible = value;
        if (StateSingleton.stateView != StateSingleton.StateView.MODE2D_PLUS_OCULUS)
            Time.timeScale = (value) ? 0 : 1;
        foreach (GameObject gts in GuiToHideInMenu)
        {
            gts.SetActive(false && !isUsingOculus);
        }

        foreach (GameObject gts in GuiToShowModeMenu)
            gts.SetActive(value);

    }

    public void GuiCameraControlUIMenu(bool value)
    {
        if (StateSingleton.stateView != StateSingleton.StateView.MODE2D_PLUS_OCULUS)
            Time.timeScale = (value) ? 0 : 1;

        GuiCameraControlUI.SetActive(value);
    }
    
    public void ToolMenu(bool value)
    {





        Cursor.visible = value;
        if (StateSingleton.stateView != StateSingleton.StateView.MODE2D_PLUS_OCULUS)
            Time.timeScale = (value) ? 0 : 1;
      
        foreach (GameObject gts in GuiToHideInMenu)
        {
                gts.SetActive(false && !isUsingOculus);
        }

        foreach (GameObject gts in GuiToShowToolMenu)
            gts.SetActive(value);



    }

    public void Placemark(bool value)
    {
        Cursor.visible = value;
        if (StateSingleton.stateView != StateSingleton.StateView.MODE2D_PLUS_OCULUS)
            Time.timeScale = (value) ? 0 : 1;
        foreach (GameObject gts in GuiToShowPlacemark)
            gts.SetActive(value);
        foreach (GameObject gts in GuiToHide)
            gts.SetActive(!value && !isUsingOculus);
    }

    IEnumerator BlinkText(GameObject t, params KeyCode[] keys)
    {
        if (!t.activeSelf)
        {
            t.gameObject.SetActive(true);
            foreach(KeyCode key in keys)
            {
                while (Input.GetKey(key))
                    yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(2);
            t.gameObject.SetActive(isMeCoordinate);
        }
        isBTCoroutineRunning = false;
    }

   

}
