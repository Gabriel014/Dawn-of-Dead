//This script handles a shop, in which there are items that can be bought and unlocked with coins.
#pragma strict

import UnityEngine.UI;

//How many coins we have left in the shop
var coinsLeft:int = 0;

//The text that displays the coins we have
var coinsText:Transform;

//The player prefs record of the coins we have
var coinsPlayerPrefs:String = "Coins";

//This class defines items in a shop.
var shopItems:ShopItem[];

public class ShopItem
{
	//The button of the item.
	var itemButton:Transform;
	
	//Is the item locked or not. 0 = locked, 1 = unlocked
	var lockState:int = 0;
	
	//How many coins we need to unlock this item
	var costToUnlock:int = 100;
	
	//The player prefs record for this item
	var playerPrefsName:String = "FroggyUnlock";
}

//The number of the currently selected item
var currentItem:int = 0;

//This is the player prefs name that will be updated with the number of the currently selected item
var playerPrefsName:String = "CurrentPlayer";

//The color of the item when we have at least one of it
var unselectedColor:Color = Color(0.6,0.6,0.6,1);

//The color of the item when it is selected
var selectedColor:Color = Color(1,1,1,1);

//The color of the item when we can't afford it
var errorColor:Color = Color(1,0,0,1);

function Start()
{
	//Get the number of coins we have
	coinsLeft = PlayerPrefs.GetInt(coinsPlayerPrefs, coinsLeft);
	
	//Update the text of the coins we have
	coinsText.GetComponent(Text).text = coinsLeft.ToString();
	
	//Get the number of the current player
	currentItem = PlayerPrefs.GetInt(playerPrefsName, currentItem);
	
	//Update all the items
	UpdateItems();
}

function UpdateItems()
{
	for ( var index:int = 0 ; index < shopItems.Length ; index++ )
	{
		//Get the lock state of this item from player prefs
		shopItems[index].lockState = PlayerPrefs.GetInt(shopItems[index].playerPrefsName, shopItems[index].lockState);
		
		//Deselect the item
		shopItems[index].itemButton.GetComponent(Image).color = unselectedColor;
		
		//If we already unlocked this item, don't display its price
		if ( shopItems[index].lockState > 0 )
		{
			//Deactivate the price and coin icon
			shopItems[index].itemButton.Find("TextPrice").gameObject.SetActive(false);
			
			//Highlight the currently selected item
			if ( index == currentItem )    shopItems[index].itemButton.GetComponent(Image).color = selectedColor;
		}
		else
		{
			//Update the text of the cost
			shopItems[index].itemButton.Find("TextPrice").GetComponent(Text).text = shopItems[index].costToUnlock.ToString();
		}
	}
}

function BuyItem( itemNumber:int )
{
	//If we already unlocked this item, just select it
	if ( shopItems[itemNumber].lockState > 0 )
	{
		//Select the item
		SelectItem(itemNumber);
	}
	else if ( shopItems[itemNumber].costToUnlock <= coinsLeft ) //If we have enough coins, buy this item
	{
		//Increase the item count
		shopItems[itemNumber].lockState = 1;
		
		//Register the item count in the player prefs
		PlayerPrefs.SetInt(shopItems[itemNumber].playerPrefsName, shopItems[itemNumber].lockState);
		
		//Deduct the price from the coins we have
		coinsLeft -= shopItems[itemNumber].costToUnlock;
		
		//Update the text of the coins we have
		coinsText.GetComponent(Text).text = coinsLeft.ToString();
		
		//Register the item lock state in the player prefs
		PlayerPrefs.SetInt(coinsPlayerPrefs, coinsLeft);
		
		//Select the item
		SelectItem(itemNumber);
	}
	
	//Update all the items
	UpdateItems();
}

//This function selects an item
function SelectItem( itemNumber:int )
{
	currentItem = itemNumber;
	
	PlayerPrefs.SetInt( playerPrefsName, itemNumber);
}
