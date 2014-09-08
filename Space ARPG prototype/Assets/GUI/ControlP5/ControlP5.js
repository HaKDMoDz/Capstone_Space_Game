#pragma strict

public var mySkin : GUISkin;

static var toolbarInt : int = 0;
static var toolbarStrings : String[] = ["Button 1","Button 2","Button 3","button 4"];

static var SliderValue : float = 0.0;

static var ScrollbarValue : float = 0.0;

static var selectionGridInt : int = 0;
static var selectionGridStrings : String[] = ["Button 1","Button 2","Button 3","Button 4"];

static var toggleBool : boolean = false;
static var toggleBool2 : boolean = false;
static var toggleBool3 : boolean = true;
static var toggleBool4 : boolean = false;

static var stringToEditOnTextField = "Enter Text";
static var passwordToEdit = "Password";

static var scrollVeiwPosition = Vector2.zero;
static var insideTheScrollView = "This is inside the ScrollView...";

static var windowRect = Rect (320, 28, 260, 104);





function OnGUI () {
	
	GUI.skin = mySkin;
	
	//--------------------------------------------------------
	// GUI.SelectionGrid + Label
	//--------------------------------------------------------
	GUI.Label(Rect(10, 10, 200, 15), "Toolbar");
	toolbarInt = GUI.Toolbar (Rect(10, 28, 260, 20), toolbarInt, toolbarStrings);

	//--------------------------------------------------------
	// GUI.SelectionGrid + Label
	//--------------------------------------------------------
	GUI.Label(Rect(10, 70, 200, 15), "Selection Grid");
	selectionGridInt = GUI.SelectionGrid(Rect(10, 88, 260, 44), selectionGridInt, selectionGridStrings,2);
	
	//--------------------------------------------------------
	// GUI - ScrollView with GUI.TextArea + Label
	//--------------------------------------------------------
	GUI.Label(Rect(10, 154, 200, 15), "Scroll View");	
	scrollVeiwPosition = GUI.BeginScrollView(Rect(10, 172, 260, 64), scrollVeiwPosition, Rect(0, 0, 400, 400));
	insideTheScrollView = GUI.TextArea (Rect (0, 0, 400, 400), insideTheScrollView, 200);
	GUI.EndScrollView ();

	//--------------------------------------------------------
	// GUI.TextField + Label
	//--------------------------------------------------------
	GUI.Label(Rect(10, 294-36, 200, 15), "Text Field");
	stringToEditOnTextField = GUI.TextField (Rect (10, 312-36, 126, 20), stringToEditOnTextField, 150);

	//--------------------------------------------------------
	// GUI.PasswordField + Label
	//--------------------------------------------------------
	GUI.Label(Rect(140, 294-36, 200, 15), "Password Field");
	passwordToEdit = GUI.PasswordField (Rect (140, 312-36, 130, 20), passwordToEdit, "*"[0], 25);
	
	//--------------------------------------------------------
	// GUI.Toggle + Label
	//--------------------------------------------------------
	GUI.Label(Rect(10, 354-36, 200, 15), "Toggles");
	toggleBool = GUI.Toggle(Rect(10, 372-36, 20, 20), toggleBool, "50");
	toggleBool2 = GUI.Toggle(Rect(75, 372-36, 20, 20), toggleBool2, "140");
	toggleBool3 = GUI.Toggle(Rect(140, 372-36, 20, 20), toggleBool3, "50");
	toggleBool4 = GUI.Toggle(Rect(205, 372-36, 20, 20), toggleBool4, "77");
	
	//--------------------------------------------------------
	// GUI.Window + Label - Check out the DoMyWindow
	//--------------------------------------------------------
	GUI.Label(Rect(320, 10, 200, 15), "Window");
	windowRect = GUI.Window (0, windowRect, DoMyWindow, "Dragable Window");

	//--------------------------------------------------------
	// GIU.HorizontalSlider + Label
	//--------------------------------------------------------
	GUI.Label(Rect(320, 154, 200, 15), "Horizontal Slider");
	SliderValue = GUI.HorizontalSlider(Rect(320, 172, 260, 12), SliderValue, 0.0, 127.0);
		
	//--------------------------------------------------------
	// GIU.HorizontalScrollbar + Label
	//--------------------------------------------------------
	GUI.Label(Rect(320, 206, 200, 15),"Horizontal Scrollbar");
	ScrollbarValue = GUI.HorizontalScrollbar(Rect(320, 224, 260, 12), ScrollbarValue, 0.0, 1.0, 10.0);
	
	//--------------------------------------------------------
	// GIU.VerticalSlider + Label
	//--------------------------------------------------------
	GUI.Label(Rect(320, 294-36, 200, 15), "Vertical Slider");
	SliderValue = GUI.VerticalSlider(Rect(320, 312-36, 12, 72), SliderValue, 0.0, 127.0);
	
	//--------------------------------------------------------
	// GIU.VerticalSlider + Label
	//--------------------------------------------------------
	GUI.Label(Rect(320+130, 294-36, 200, 15), "Vertical Scrollbar");
	ScrollbarValue = GUI.VerticalScrollbar(Rect(320+130, 312-36, 12, 72), ScrollbarValue, 0.0, 1.0, 10.0);
	
}


//------------------------------------------------------------
// This function is called from the GUI.Window
//------------------------------------------------------------
function DoMyWindow (windowID : int) {
	
	GUI.DragWindow (Rect (0,0, 10000, 20));	
}



