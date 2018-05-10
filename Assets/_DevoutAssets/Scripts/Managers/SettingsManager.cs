using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// Written by Duke Im
/// On 2017-02-28
/// 
/// <summary>
/// Manage game settings such as quality, sound and etc
/// Singleton
/// </summary>
public class SettingsManager : MonoBehaviour
{
	public static int MINIMUM_SCREEN_WIDTH = 1024;
	public static SettingsManager SINGLETON;

	public Slider _audioVolume;
	public Slider _graphicQuality;
	public Slider _mouseSensitivity;
	public Toggle _fullScreen;

	public Dropdown _ResolutionDropDown;
	//	Resolution[] _resolutions;
	List<Resolution> _resolutions;

	// Use this for initialization
	void Start ()
	{
		if (!SINGLETON) {
			SINGLETON = this;
		} else {
			Debug.LogWarning ("WARNING : More than one SettingsManager found");
		}

		loadPlayerPref ();
		initResolutionDropDown ();
	}

	void loadPlayerPref ()
	{
		QualitySettings.SetQualityLevel ((int)PlayerPrefs.GetFloat ("Quality"));
		AudioListener.volume = PlayerPrefs.GetFloat ("Volume");
		if (PlayerPrefs.GetInt ("ScreenResolutionWidth") >= MINIMUM_SCREEN_WIDTH) {
			if (PlayerPrefs.GetInt ("FullScreen") == 1)
				Screen.SetResolution (PlayerPrefs.GetInt ("ScreenResolutionWidth"), PlayerPrefs.GetInt ("ScreenResolutionHeight"), true);
			else
				Screen.SetResolution (PlayerPrefs.GetInt ("ScreenResolutionWidth"), PlayerPrefs.GetInt ("ScreenResolutionHeight"), false);
		}

		int qualityLevel = QualitySettings.GetQualityLevel ();
		_audioVolume.value = AudioListener.volume;
		_graphicQuality.value = qualityLevel;
		_fullScreen.isOn = Screen.fullScreen;
		if (PlayerPrefs.GetFloat ("MouseSensitivity") != null)
			_mouseSensitivity.value = PlayerPrefs.GetFloat ("MouseSensitivity");
		else
			_mouseSensitivity.value = 1f;
	}

	void initResolutionDropDown ()
	{
		_ResolutionDropDown.options.Clear ();

		_resolutions = Screen.resolutions.ToList ();
		_resolutions.RemoveAll (x => x.width < MINIMUM_SCREEN_WIDTH);

		for (int i = 0; i < _resolutions.Count; i++) {
			_ResolutionDropDown.options.Add (new Dropdown.OptionData (resToString (_resolutions [i])));

			_ResolutionDropDown.value = i;
		}	

		_ResolutionDropDown.value = _resolutions.FindIndex (x => x.width == Screen.width && x.height == Screen.height);
//		print (Screen.width + "x" + Screen.height);

		_ResolutionDropDown.onValueChanged.AddListener (delegate { 
			Screen.SetResolution (_resolutions [_ResolutionDropDown.value].width, _resolutions [_ResolutionDropDown.value].height, Screen.fullScreen);
			SaveSettings ();
		});
	}

	string resToString (Resolution res)
	{
		return res.width + " x " + res.height;
	}

	//	public void SetScreenResolution(){
	//		Screen.SetResolution (_resolutions [_ResolutionDropDown.value].width, _resolutions [_ResolutionDropDown.value].height, Screen.fullScreen);
	//	}

	public void SaveSettings ()
	{
		PlayerPrefs.SetFloat ("Quality", QualitySettings.GetQualityLevel ());
		PlayerPrefs.SetFloat ("Volume", AudioListener.volume);
		PlayerPrefs.SetInt ("ScreenResolutionWidth", Screen.width);
		PlayerPrefs.SetInt ("ScreenResolutionHeight", Screen.height);
		PlayerPrefs.SetInt ("FullScreen", Screen.fullScreen ? 1 : 0);
		if(GameController.GC != null)
			PlayerPrefs.SetFloat ("MouseSensitivity", GameController.GC.MouseSensitivity);
	}

	public void SetMouseSensitivity(float value){
		GameController.GC.MouseSensitivity = value;
		SaveSettings ();
	}

	public void SetQuality (float value)
	{
		QualitySettings.SetQualityLevel ((int)value);	
		SaveSettings ();	
	}

	public void SetAudioVolume (float value)
	{
		AudioListener.volume = value;
		SaveSettings ();
	}

	public void FullScreen (bool isFullScreen)
	{
		Screen.fullScreen = isFullScreen;
		SaveSettings ();
	}

	void OnApplicationQuit ()
	{
		SaveSettings ();
	}
}
