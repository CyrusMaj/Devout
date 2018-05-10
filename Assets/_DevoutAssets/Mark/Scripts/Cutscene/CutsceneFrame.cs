using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CutsceneFrame : MonoBehaviour {

	private string _text;
	private Image _img;

	public CutsceneFrame(Image img, string text)
	{
		_text = text;
		_img = img;
	}
		
	public Image getImg()
	{
		return _img;
	}

	public string getTxt()
	{
		return _text;
	}

}
