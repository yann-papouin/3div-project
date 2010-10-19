#include "vrpnHeaders\vrpn_Analog.h"
#include "vrpnHeaders\vrpn_Button.h"
#include "vrpnHeaders\vrpn_Keyboard.h"
#include "vrpnHeaders\vrpn_Tracker_3DMouse.h"
#include <string>
#include <sstream>

class vrpnController
{
public:
	vrpnController(void);
	//vrpnController(Ogre::Camera *playercam);
	void loopAllRemotes(void);
	inline int getFeedback(){return feedback;}

private:
	vrpn_Analog_Remote *vrpnAnalog;
	vrpn_Button_Remote *vrpnButton;
	vrpn_Button_Remote *vrpnKeyboard;
	vrpn_Tracker_3DMouse *vrpn3DMouse;
	int feedback;
	//Ogre::Camera *camera;
	//void VRPN_CALLBACK handle_analog(void* userData, const vrpn_ANALOGCB a);
	//void VRPN_CALLBACK handle_button(void* userData, const vrpn_BUTTONCB b);
};