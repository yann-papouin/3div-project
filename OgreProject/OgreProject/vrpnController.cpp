
#include "vrpnController.h"

void vrpnController::loopAllRemotes(void)
{
	vrpnAnalog->mainloop();
	vrpnButton->mainloop();
	vrpnKeyboard->mainloop(); //Deze 3 heb ik getest, en blijken niet nodig?
}

/*
* Callback voor de analoge waarden van de gewone muis (3d muis = ook compatibel normaal)
*/
void VRPN_CALLBACK handle_analog(void* userData, const vrpn_ANALOGCB a)
{
	//std::string *feedback = static_cast<std::string*>(userData);
	int nbChannels = a.num_channel;
	//(*feedback) = ("A");
	for(int i=0; i < a.num_channel; i++)
	{
		//(*feedback) += a.channel[i];
	}
}

/*
* Callback voor de knoppen
*/
void VRPN_CALLBACK handle_button(void* userData, const vrpn_BUTTONCB b)
{	
	//std::string *feedback = static_cast<std::string*>(userData);
	//(*feedback) = "B" + b.button + ':' + b.state;
	if ( b.state == 1)
	{
		std::stringstream out;
		out << b.button;

		//(*feedback) = out.str();
	}
	else
	{
		//(*feedback) = "LB niet ingedrukt";
	}
}

/*
* Callback voor het toetsenbord
*/
void VRPN_CALLBACK handle_keyboard(void* userData, const vrpn_BUTTONCB k)
{
	//std::string *feedback = static_cast<std::string*>(userData);
	if (k.state == 1)
	{
		std::stringstream out;
		out << k.button;

		//(*feedback) = out.str();
	}
	else
	{
		//(*feedback) = "Geen knop ingedrukt";
	}
}

vrpnController::vrpnController(void)
{
	//vrpnAnalog = new vrpn_Analog_Remote("Mouse0@localhost");
	//vrpnButton = new vrpn_Button_Remote("Mouse0@localhost");
	vrpnAnalog = new vrpn_Analog_Remote("device0@localhost");
	vrpnButton = new vrpn_Button_Remote("device0@localhost");
	vrpnKeyboard = new vrpn_Button_Remote("Keyboard0@localhost");

	//Volgende functies kloppen wel, maar zijn commented om die feedback te kunnen tonen
	//vrpnAnalog->register_change_handler(&feedback, handle_analog);//(void*)feedback.c_str() -> userdata?
	vrpnButton->register_change_handler(&feedback, handle_button);//(void*)feedback.c_str()
	//vrpnKeyboard->register_change_handler(&feedback, handle_keyboard);
}

/*
* vrpncontroller met camera - object -> voor bewegen cam
*/
/*
vrpnController::vrpnController(Ogre::Camera *playercam)
{
	camera = playercam;
}
*/