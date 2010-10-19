#pragma once

#ifndef __CAMERACONTROLLER_H__
#define __CAMERACONTROLLER_H__

#include "InputManager.h"
#include <OgreCamera.h>
#include <OgreRenderWindow.h>
#include <OgreSceneManager.h>
#include <OISEvents.h>
#include <OISKeyboard.h>
#include <OISMouse.h>
#include <OgreRoot.h>
#include <OgreViewport.h>

class CameraController : public OIS::KeyListener, public OIS::MouseListener
{
public:
	CameraController(Ogre::RenderWindow* w, Ogre::SceneManager* sm);
	~CameraController(void);	
	void initialise();
	Ogre::Camera* getCamera();
	void createViewports();

	// OIS::KeyListener
	virtual bool keyPressed( const OIS::KeyEvent &arg );
	virtual bool keyReleased( const OIS::KeyEvent &arg );
	// OIS::MouseListener
	virtual bool mouseMoved( const OIS::MouseEvent &arg );
	virtual bool mousePressed( const OIS::MouseEvent &arg, OIS::MouseButtonID id );
	virtual bool mouseReleased( const OIS::MouseEvent &arg, OIS::MouseButtonID id );

private:
	Ogre::Camera* m_pCamera;
	Ogre::RenderWindow* m_pWindow;
	Ogre::SceneManager* m_pSceneMan;
};

#endif

