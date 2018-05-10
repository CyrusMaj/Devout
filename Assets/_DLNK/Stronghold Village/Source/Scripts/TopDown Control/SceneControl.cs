// TOPDOWN CONTROL SCRIPTS 1.0 www.dlnkworks.com 2016(c) This script is licensed as free content in the pack. Support is not granted while this is not part of the art pack core. Is licensed for commercial purposes while not for resell.

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneControl : MonoBehaviour {

    // public vars
    [Header("[Scene Settings]")]
    public Light SunLight;
    public Material SkyBackground;
    public AudioSource BackgroundMusic;
    public GameObject Fxs;
    [Header("[Player & Camera]")]
    public TownManager Town;
    public Collider PlayerCharacter;
    public GameObject FPSCamera;
    public GameObject TopDownCamera;
    [Header("[Interface]")]
    public KeyCode CameraSwitchKey = KeyCode.F5;
    public KeyCode HelpKey = KeyCode.F1;
    public KeyCode DecoCheap = KeyCode.F2;
    public KeyCode DoorKey;
    public Text LocationGUI;
    public Text CameraGUI;
    public Text HelpText;

    // hidden vars
    [HideInInspector]
    public bool FPSMode = false;

    // Use this for initialization
    void Start () {
        // Set Player Character on Town Manager script
        Town.PlayerCollider = PlayerCharacter;
        // Set Help Text
        HelpText.text = "[" + HelpKey + "] Show/Hide help. [" + DoorKey + "] Open / close doors. [" + CameraSwitchKey + "] Switch camera. [" + DecoCheap + "] Decoration Quality";
        // Set DoorKeycode on TownManager
        Town.DoorKeyCode = DoorKey;
	}
	
	// Update is called once per frame
	void Update () {

        // Show/Hide Help
        if (Input.GetKeyUp(HelpKey))
        {
            if (HelpText.isActiveAndEnabled)
            {
                LocationGUI.gameObject.SetActive(false);
                CameraGUI.gameObject.SetActive(false);
                HelpText.gameObject.SetActive(false);
            }
            else
            {
                LocationGUI.gameObject.SetActive(true);
                CameraGUI.gameObject.SetActive(true);
                HelpText.gameObject.SetActive(true);
            }
        }
        // Show Player Location on gui
        LocationGUI.text = (Town.PlayerLocation + " [Floor " + Town.ActualFloor + "]");

        // Camera Switch key pressed
        if (Input.GetKeyUp(CameraSwitchKey))
        {
            if (TopDownCamera.activeInHierarchy)
                // Change from TopDown to FPS
        {
            Town.FPSMode = true;
            TopDownCamera.SetActive(false);
            FPSCamera.gameObject.SetActive(true);
            CameraGUI.text = "Handheld Camera (FPS)";
        }
        else
                 // Change from FPS to TopDown
        {
            Town.FPSMode = false;
            TopDownCamera.SetActive(true);
            FPSCamera.gameObject.SetActive(false);
            CameraGUI.text = "Top Down Camera";
        }
    }
        // Change Decoration Quality on Town Manager with Hotkey

        if (Input.GetKeyUp(DecoCheap))
        {
            if (Town.CheapDecoration)
                Town.CheapDecoration = false;
            else
                Town.CheapDecoration = true;
        }

    }
}
