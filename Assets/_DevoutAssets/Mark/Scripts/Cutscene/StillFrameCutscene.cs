using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StillFrameCutscene : MonoBehaviour {

	public List<CutsceneFrame> _frames;
	private CutsceneFrame currFrame;
	private int currFrameInd = 0;

//	protected virtual void Start()
//	{
//		_frames = frames;
//		currFrame = _frames [0];
//	}

	public void AdvanceFrame()
	{
		currFrameInd++;
		currFrame = _frames [currFrameInd];
	}

	public CutsceneFrame getCurrFrame()
	{
		return currFrame;
	}

	public void LoadFrame(CutsceneUIHandler ui)
	{
		ui.setImg (currFrame.getImg ());
		ui.setText (currFrame.getTxt ());
	}
}
