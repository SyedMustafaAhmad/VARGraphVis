using System.Globalization;
using UnityEngine;

namespace ForceDirectedDiagram.Scripts.Helpers
{
	//This script calculates the average framerate and displays it in the upper right corner of the screen;
	public class FPSCounter : MonoBehaviour {

		//Framerate is calculated using this interval;
		[SerializeField] private float checkInterval = 1f;

		//Variables to keep track of passed time and frames;
		private int _currentPassedFrames;
		private float _currentPassedTime;

		//Current framerate;
		private float _currentFrameRate;
		private string _currentFrameRateString = "";
		
		// Update;
		private void Update () {

			//Increment passed frames;
			_currentPassedFrames ++;

			//Increment passed time;
			_currentPassedTime += Time.deltaTime;

			//If passed time has reached 'checkInterval', recalculate framerate;
			if(_currentPassedTime >= checkInterval)
			{
				//Calculate frame rate;
				_currentFrameRate = _currentPassedFrames/_currentPassedTime;

				//Reset counters;
				_currentPassedTime = 0f;
				_currentPassedFrames = 0;

				//Clamp to two digits behind comma;
				_currentFrameRate *= 100f;
				_currentFrameRate = (int)_currentFrameRate;
				_currentFrameRate /= 100f;

				//Calculate framerate string to display later;
				_currentFrameRateString = _currentFrameRate.ToString(CultureInfo.InvariantCulture);
			}
		}

		//Render framerate in the upper right corner of the screen;
		private void OnGUI()
		{
			GUI.contentColor = Color.black;

			const float labelSize = 40f;
			const float offset = 2f;

			GUI.Label(new Rect(Screen.width - labelSize + offset, Screen.height - 30f + offset, labelSize, 30f), _currentFrameRateString);

			GUI.contentColor = Color.white;

			GUI.Label(new Rect(Screen.width - labelSize, Screen.height - 30f, labelSize, 30f), _currentFrameRateString);
		}
	}
}