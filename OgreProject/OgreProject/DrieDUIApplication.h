/*
-----------------------------------------------------------------------------
Filename:    DrieDUIApplication.h
-----------------------------------------------------------------------------
*/
#ifndef __DrieDUIApplication_h_
#define __DrieDUIApplication_h_

#include "BaseApplication.h"

class DrieDUIApplication : public BaseApplication
{
public:
	DrieDUIApplication(void);
	virtual ~DrieDUIApplication(void);

protected:
	virtual void createScene(void);
	virtual bool keyPressed( const OIS::KeyEvent &arg );
	void createLight(void);
	void createGroundAndSky(void);
	void createObjects(void);
	void manipulateNode(Ogre::Node* node);

	Ogre::Node* selectedNode;
	int it;
};

#endif // #ifndef __DrieDUIApplication_h_
