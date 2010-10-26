#include <stdio.h>
#include <stdlib.h>
#include "wiiuse.h"
#include <iostream>

using namespace std;

class wiiController{
public:
	wiiController();
	int initController();


private:
	wiimote **wiimotes;
	int found, connected;

};