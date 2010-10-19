#include "CameraController.h"
#include <iostream>
using namespace std;


CameraController::CameraController(Ogre::RenderWindow* w, Ogre::SceneManager* sm){
	m_pWindow = w;
	m_pSceneMan = sm;
}

CameraController::~CameraController(){
}

void CameraController::initialise(){
	m_pCamera = m_pSceneMan->createCamera("PlayerCam");
	m_pCamera->setPosition(Ogre::Vector3(0, 500, -2500));
	m_pCamera->lookAt(Ogre::Vector3(0,0,0));
	m_pCamera->setNearClipDistance(5);

	createViewports();

	InputManager::getSingletonPtr()->addKeyListener(this, "ccKeyListener");
	InputManager::getSingletonPtr()->addMouseListener(this, "ccMouseListener");
}

Ogre::Camera* CameraController::getCamera(){
	return m_pCamera;
}
void CameraController::createViewports(){// Create one viewport, entire window
	Ogre::Viewport* vp = m_pWindow->addViewport(m_pCamera);
	vp->setBackgroundColour(Ogre::ColourValue(0,0,0));

	// Alter the camera aspect ratio to match the viewport
	m_pCamera->setAspectRatio(
		Ogre::Real(vp->getActualWidth()) / Ogre::Real(vp->getActualHeight()));
}

// OIS::KeyListener
bool CameraController::keyPressed( const OIS::KeyEvent &arg ){
	if (arg.key == OIS::KC_UP)
	{
		m_pCamera->moveRelative(Ogre::Vector3(0,0,50));
		return false;
	}
	else if (arg.key == OIS::KC_DOWN)
	{
		m_pCamera->moveRelative(Ogre::Vector3(0,0,-50));
		return false;
	}
	else if (arg.key == OIS::KC_LEFT)
	{
		m_pCamera->moveRelative(Ogre::Vector3(-50,0,0));
		return false;
	}
	else if (arg.key == OIS::KC_RIGHT)
	{
		m_pCamera->moveRelative(Ogre::Vector3(50,0,0));
		return false;
	}

	return true;
}
bool CameraController::keyReleased( const OIS::KeyEvent &arg ){
	return true;
}
// OIS::MouseListener
bool CameraController::mouseMoved( const OIS::MouseEvent &arg ){
	return true;
}
bool CameraController::mousePressed( const OIS::MouseEvent &arg, OIS::MouseButtonID id ){
	return true;
}
bool CameraController::mouseReleased( const OIS::MouseEvent &arg, OIS::MouseButtonID id ){
	return true;
}