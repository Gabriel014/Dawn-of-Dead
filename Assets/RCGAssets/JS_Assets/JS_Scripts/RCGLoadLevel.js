//This script includes functions for loading levels and URLs. It's intended for use with UI Buttons
#pragma strict

//Load a URL
function LoadURL( urlName:String )
{
	Application.OpenURL(urlName);
}

//Load a level from the scene hierarchy
function LoadLevel( levelName:String )
{
	Application.LoadLevel(levelName);
}

//This function restarts the current level
function RestartLevel()
{
	Application.LoadLevel(Application.loadedLevelName);
}