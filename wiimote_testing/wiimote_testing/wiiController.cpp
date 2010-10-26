#include "wiiController.h"

wiiController::wiiController()
{
	initController();
}

int wiiController::initController()
{
	//Init all connected wiimote objects
	wiimotes = wiiuse_init(2);
	//Find wiimotes
	found = wiiuse_find(wiimotes, 2, 5);
	if (!found){
		cout << "Could not find any wiimotes" << endl;
		return 0;
	}
	connected = wiiuse_connect(wiimotes, 2);
	if (connected) 
		cout << "Connected to " << connected << " wiimotes of " << found << "found" << endl;
	else{
		cout << "Could not connect to any wiimote" << endl;
		return 0;
	}
	//Set a led and rumble, just to check :p
	wiiuse_set_leds(wiimotes[0], WIIMOTE_LED_1);
	wiiuse_set_leds(wiimotes[1], WIIMOTE_LED_2);

	//Setup for the motion events
	wiiuse_motion_sensing(wiimotes[0], 1);
	wiiuse_motion_sensing(wiimotes[1], 1);
	wiiuse_set_orient_threshold(wiimotes[0], 5.0f); 	//Threshold in graden
	wiiuse_set_orient_threshold(wiimotes[1], 5.0f);
	
	bool opgehouden = false;
	//pollen
	while (1) {
		if (wiiuse_poll(wiimotes, 2)) {
			//int i = 0;
			for (int i = 0; i < 2; ++i) {
				switch (wiimotes[i]->event) {
					case (WIIUSE_EVENT):
					{
						if (IS_PRESSED(wiimotes[i], WIIMOTE_BUTTON_UP)){
							cout << "Pressed up on wiimote " << i+1 << endl;
						} else
						if (IS_PRESSED(wiimotes[i], WIIMOTE_BUTTON_DOWN)){
							cout << "Pressed down on wiimote " << i+1 << endl;
						} else
						if (IS_PRESSED(wiimotes[i], WIIMOTE_BUTTON_LEFT)){
							cout << "Pressed left on wiimote " << i+1 << endl;
						} else
						if (IS_PRESSED(wiimotes[i], WIIMOTE_BUTTON_RIGHT)){
							cout << "Pressed rechts on wiimote " << i+1 << endl;
						} else
							if (WIIUSE_USING_ACC(wiimotes[i])){
								
								cout << "Wiimote " << i + 1 << " roll = " << wiimotes[i]->orient.roll << '\t' <<  wiimotes[i]->orient.a_roll << endl;
								cout << "Wiimote " << i + 1 << " pitch = " << wiimotes[i]->orient.pitch << '\t' <<  wiimotes[i]->orient.a_pitch << endl;
								cout << "Wiimote " << i + 1 << " yaw = " << wiimotes[i]->orient.yaw << endl;
								
								if (wiimotes[i]->orient.pitch < -80.0f & !opgehouden)
								{
									//cout << "Wiimote " << i + 1 << " rechtop gehouden" << endl;
									opgehouden = true;
								}else
								{
									//cout << "Wiimote " << i + 1 << " neer gelegd" << endl;
									opgehouden = false;
								}
						}

					}
					break;
				}
			}
		}
	}
	//wiiuse_rumble(wiimotes[0], 1);
	return 1;
}