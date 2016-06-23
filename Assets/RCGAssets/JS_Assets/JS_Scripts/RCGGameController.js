//This script controls the game, starting it, following game progress, and finishing it with game over.
//It also creates lanes with moving objects and items as the palyer progresses.
#pragma strict

//Using the new Unity 4.6 UI
import UnityEngine.UI;

//The player object
var playerObjects:Transform[];
var currentPlayer:int = 0;

//The camera that follows the player
var cameraObject:Transform;

//The object that holds the movement buttons. If swipe controls are used, this object is deactivated
var moveButtonsObject:Transform;

//Should swipe controls be used instead of click/tap controls?
var swipeControls:boolean = false;

//The start and end positions of the touches, when using swipe controls
internal var swipeStart:Vector3;
internal var swipeEnd:Vector3;

//The swipe distance; How far we need to swipe before detecting movement
var swipeDistance:float = 10;

//How long to wait before the swipe command is cancelled
var swipeTimeout:float = 1;
internal var swipeTimeoutCount:float;

//A list of lanes that that randomly appear as the player moves forward
var lanes:Lane[];
internal var lanesList:Lane[];

public class Lane
{
	var laneObject:Transform;
	var laneChance:float = 1;
	var laneWidth:float = 1;
	var itemChance:float = 1;
}

//A list of the objects that can be dropped
var objectDrops:ObjectDrop[];
internal var objectDropList:Transform[];

public class ObjectDrop
{
	//The object that can be dropped
	var droppedObject:Transform;
	
	//The drop chance of the object
	var dropChance:int = 1;
}

//The offset left and right on the lane
var objectDropOffset:int = 4;

//How many lanes to create before starting the game
var precreateLanes:int = 20;
internal var nextLanePosition:float = 1;

//The score and score text of the player
var score:int = 0;
var scoreText:Transform;
internal var highScore:int = 0;

//The player prefs record of the total coins we have ( not high score, but total coins we collected in all games )
var coinsPlayerPrefs:String = "Coins";

//The overall game speed
var gameSpeed:float = 1;

//How many points the player needs to collect before leveling up
var levelUpEveryCoins:int = 5;
internal var increaseCount:int = 0;

//This is the speed of the camera that keeps advancing on the player and kills him instantly if it reaches him
var deathLineObject:Transform;
var deathLineSpeed:float = 1;
var deathLineSpeedIncrease:float = 1;
var deathLineSpeedMax:float = 1.5;

//Various canvases for the UI
var gameCanvas:Transform;
var pauseCanvas:Transform;
var gameOverCanvas:Transform;

//Is the game over?
internal var isGameOver:boolean = false;

//The level of the main menu that can be loaded after the game ends
var mainMenuLevelName:String = "MainMenu";

//Various sounds
var soundLevelUp:AudioClip;
var soundGameOver:AudioClip;

//The tag of the sound source
var soundSourceTag:String = "GameController";

var confirmButton:String = "Submit";

//The button that pauses the game. Clicking on the pause button in the UI also pauses the game
var pauseButton:String = "Cancel";
internal var isPaused:boolean = false;

internal var index:int = 0;

function Start() 
{
	//Update the score without adding to it
	ChangeScore(0);
	
	//Hide the game over canvas
	if ( gameOverCanvas )    gameOverCanvas.gameObject.SetActive(false);
	
	//Get the highscore for the player
	highScore = PlayerPrefs.GetInt(Application.loadedLevelName + "_HighScore", 0);
	
	//Calculate the chances for the lanes to appear
	var totalLanes:int = 0;
	var totalLanesIndex:int = 0;
	
	//Calculate the total number of drops with their chances
	for ( index = 0 ; index < lanes.Length ; index++ )
	{
		totalLanes += lanes[index].laneChance;
	}
	
	//Create a new list of the objects that can be dropped
	lanesList = new Lane[totalLanes];
	
	//Go through the list again and fill out each type of drop based on its drop chance
	for ( index = 0 ; index < lanes.Length ; index++ )
	{
		var laneChanceCount:int = 0;
		
		while ( laneChanceCount < lanes[index].laneChance )
		{
			lanesList[totalLanesIndex] = lanes[index];
			
			laneChanceCount++;
			
			totalLanesIndex++;
		}
	}
	
	//Calculate the chances for the objects to drop
	var totalDrops:int = 0;
	var totalDropsIndex:int = 0;
	
	//Calculate the total number of drops with their chances
	for ( index = 0 ; index < objectDrops.Length ; index++ )
	{
		totalDrops += objectDrops[index].dropChance;
	}
	
	//Create a new list of the objects that can be dropped
	objectDropList = new Transform[totalDrops];
	
	//Go through the list again and fill out each type of drop based on its drop chance
	for ( index = 0 ; index < objectDrops.Length ; index++ )
	{
		var dropChanceCount:int = 0;
		
		while ( dropChanceCount < objectDrops[index].dropChance )
		{
			objectDropList[totalDropsIndex] = objectDrops[index].droppedObject;
			
			dropChanceCount++;
			
			totalDropsIndex++;
		}
	}
	
	//Get the currently selected player from PlayerPrefs
	currentPlayer = PlayerPrefs.GetInt("CurrentPlayer", currentPlayer);
	
	//Set the current player object
	SetPlayer(currentPlayer);
	
	// If the player object is not already assigned, Assign it from the "Player" tag
	//if( playerObjects[currentPlayer] == null )    playerObjects[currentPlayer] = GameObject.FindGameObjectWithTag("Player").transform;
	
	//If the player object is not already assigned, Assign it from the "Player" tag
	if ( cameraObject == null )    cameraObject = GameObject.FindGameObjectWithTag("MainCamera").transform;
	
	//Create a few lanes at the start of the game
	createLane(precreateLanes);
	
	//If swipe controls are on, deactivate button controls
	if ( swipeControls == true && moveButtonsObject )    moveButtonsObject.gameObject.SetActive(false);
	
	//Pause the game at the start
	Pause();
}

function Update() 
{
	//If the game is over, listen for the Restart and MainMenu buttons
	if ( isGameOver == true )
	{
		//The jump button restarts the game
		if ( Input.GetButtonDown(confirmButton) )
		{
			Restart();
		}
		
		//The pause button goes to the main menu
		if ( Input.GetButtonDown(pauseButton) )
		{
			MainMenu();
		}
	}
	else
	{
		//Toggle pause/unpause in the game
		if ( Input.GetButtonDown(pauseButton) )
		{
			if ( isPaused == true )    Unpause();
			else    Pause();
		}
		
		//Using swipe controls to move the player
		if ( swipeControls == true )
		{
			if ( swipeTimeoutCount > 0 )    swipeTimeoutCount -= Time.deltaTime;
			
			//Check touches on the screen
			for ( var touch:Touch in Input.touches )
	        {
	            //Check the touch position at the beginning
	            if ( touch.phase == TouchPhase.Began )
	            {
	                swipeStart = touch.position;
	                swipeEnd = touch.position;
	                
	                swipeTimeoutCount = swipeTimeout;
	            }
	           	
	           	//Check swiping motion
	            if ( touch.phase == TouchPhase.Moved )
	            {
	                swipeEnd = touch.position;
	            }
	           
	            //Check the touch position at the end, and move the player accordingly
	            if( touch.phase == TouchPhase.Ended && swipeTimeoutCount > 0 )
	            {
	                if( (swipeStart.x - swipeEnd.x) > swipeDistance && (swipeStart.y - swipeEnd.y) < -swipeDistance ) //Swipe left
	                {
	                    MovePlayer("left");
	                }
	                else if((swipeStart.x - swipeEnd.x) < -swipeDistance && (swipeStart.y - swipeEnd.y) > swipeDistance ) //Swipe right
	                {
	                    MovePlayer("right");
	                }
	                else if((swipeStart.y - swipeEnd.y) < -swipeDistance && (swipeStart.x - swipeEnd.x) < -swipeDistance ) //Swipe up
	                {
	                    MovePlayer("forward");
	                }
	                else if((swipeStart.y - swipeEnd.y) > swipeDistance && (swipeStart.x - swipeEnd.x) > swipeDistance ) //Swipe down
	                {
	                    MovePlayer("backward");
	              	}
	            }
			}
		}
	}
	
	//If the camera moved forward enough, create another lane
	if ( nextLanePosition - cameraObject.position.x < precreateLanes )
	{ 
		createLane(1);
	}
		
	if ( cameraObject )
	{
		//Make the camera chase the player in all directions
		if ( playerObjects[currentPlayer] )    
		{
			cameraObject.position.x = Mathf.Lerp( cameraObject.position.x, playerObjects[currentPlayer].position.x, Time.deltaTime * 3);
			cameraObject.position.z = Mathf.Lerp( cameraObject.position.z, playerObjects[currentPlayer].position.z, Time.deltaTime * 3);
		}
		
		//Make the death line chase the player forward only
		if ( deathLineObject )    
		{
			if ( isGameOver == false )    deathLineObject.position.x += deathLineSpeed * Time.deltaTime;
			
			if ( cameraObject.position.x > deathLineObject.position.x )    deathLineObject.position.x = cameraObject.position.x;
		}
	}
}

//This function creates a lane, sometimes reversing the paths of the moving objects in it
function createLane( laneCount:int )
{
	//Create a few lanes at the start of the game
	while ( laneCount > 0 )
	{
		laneCount--;
		
		//Choose a random lane from the list
		var randomLane:int = Mathf.Floor(Random.Range(0, lanesList.Length));
		
		//Create a random lane from the list of available lanes
		var newLane = Instantiate( lanesList[randomLane].laneObject, Vector3(nextLanePosition,0,0), Quaternion.identity);

		if ( Random.value < lanesList[randomLane].itemChance )
		{ 
			var newObject = Instantiate( objectDropList[Mathf.Floor(Random.Range(0, objectDropList.Length))] );
			
			newObject.position = newLane.position;
			
			newObject.position.z += Mathf.Round(Random.Range(-objectDropOffset,objectDropOffset));
		}
		
		//Go to the next lane position, based on the width of the current lane
		nextLanePosition += lanesList[randomLane].laneWidth;
	}
}

//This function changes the score of the player
function ChangeScore( changeValue:int )
{
	//Change the score
	score += changeValue;
	
	//Update the score text
	if ( scoreText )    scoreText.GetComponent(Text).text = score.ToString();
	
	//Increase the counter to the next level
	increaseCount += changeValue;
	
	//If we reached the required number of points, level up!
	if ( increaseCount >= levelUpEveryCoins )
	{
		increaseCount -= levelUpEveryCoins;
		
		LevelUp();
	}
}

//This function levels up, and increases the difficulty of the game
function LevelUp()
{
	//Increase the speed of the death line ( the moving fog ), but never above the maximum allowed value
	if ( deathLineSpeed + deathLineSpeedIncrease < deathLineSpeedMax )    deathLineSpeed += deathLineSpeedIncrease;	
	
	//If there is a source and a sound, play it from the source
	if ( soundSourceTag != String.Empty && soundLevelUp )    GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent.<AudioSource>().PlayOneShot(soundLevelUp);
}

//This function pauses the game
function Pause()
{
	isPaused = true;
	
	//Set timescale to 0, preventing anything from moving
	Time.timeScale = 0;
	
	//Show the pause screen and hide the game screen
	if ( pauseCanvas )    pauseCanvas.gameObject.SetActive(true);
	if ( gameCanvas )    gameCanvas.gameObject.SetActive(false);
}

function Unpause()
{
	isPaused = false;
	
	//Set timescale back to the current game speed
	Time.timeScale = gameSpeed;
	
	//Hide the pause screen and show the game screen
	if ( pauseCanvas )    pauseCanvas.gameObject.SetActive(false);
	if ( gameCanvas )    gameCanvas.gameObject.SetActive(true);
}

//This function handles the game over event
function GameOver( delay:float )
{
	yield WaitForSeconds(delay);
	
	isGameOver = true;
	
	//Remove the pause and game screens
	if ( pauseCanvas )    Destroy(pauseCanvas.gameObject);
	if ( gameCanvas )    Destroy(gameCanvas.gameObject);
	
	//Get the number of coins we have
	var totalCoins:int = PlayerPrefs.GetInt( coinsPlayerPrefs, 0);
	
	//Add to the number of coins we collected in this game
	totalCoins += score;
	
	//Record the number of coins we have
	PlayerPrefs.SetInt( coinsPlayerPrefs, totalCoins);
	
	//Show the game over screen
	if ( gameOverCanvas )    
	{
		//Show the game over screen
		gameOverCanvas.gameObject.SetActive(true);
		
		//Write the score text
		gameOverCanvas.Find("TextScore").GetComponent(Text).text = "SCORE " + score.ToString();
		
		//Check if we got a high score
		if ( score > highScore )    
		{
			highScore = score;
			
			//Register the new high score
			PlayerPrefs.SetInt(Application.loadedLevelName + "_HighScore", score);
		}
		
		//Write the high sscore text
		gameOverCanvas.Find("TextHighScore").GetComponent(Text).text = "HIGH SCORE " + highScore.ToString();
	}
}

//This function restarts the current level
function Restart()
{
	Application.LoadLevel(Application.loadedLevelName);
}

//This function returns to the Main Menu
function MainMenu()
{
	Application.LoadLevel(mainMenuLevelName);
}

//This function activates the selected player, while deactivating all the others
function SetPlayer( playerNumber:int )
{
	//Go through all the players, and hide each one except the current player
	for(index = 0; index < playerObjects.Length; index++)
	{
		if ( index != playerNumber )    playerObjects[index].gameObject.SetActive(false);
		else    playerObjects[index].gameObject.SetActive(true);
	}
}

//This function sends a move command to the current player
function MovePlayer( moveDirection:String )
{
	if ( playerObjects[currentPlayer] )    playerObjects[currentPlayer].SendMessage("Move", moveDirection);
}
