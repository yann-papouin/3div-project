#include "CameraController.h"
#include <iostream>
#include "math.h"
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
		return false;
	else if (arg.key == OIS::KC_DOWN)
		return false;
	else if (arg.key == OIS::KC_LEFT)
		return false;
	else if (arg.key == OIS::KC_RIGHT)
		return false;

	return true;
}

bool CameraController::keyReleased( const OIS::KeyEvent &arg ){
	if (arg.key == OIS::KC_UP)
		return false;
	else if (arg.key == OIS::KC_DOWN)
		return false;
	else if (arg.key == OIS::KC_LEFT)
		return false;
	else if (arg.key == OIS::KC_RIGHT)
		return false;	

	return true;
}

void CameraController::update(Ogre::Real secondsElapsed){
	InputManager* inputMan = InputManager::getSingletonPtr();
	Ogre::Vector3 result = Ogre::Vector3(0,0,0);
	if(inputMan->keyDown(OIS::KC_UP) || inputMan->wiiKeyUp)
		result += Ogre::Vector3(0,0,-500);
	if(inputMan->keyDown(OIS::KC_DOWN) || inputMan->wiiKeyDown)
		result += Ogre::Vector3(0,0,500);
	if(inputMan->keyDown(OIS::KC_LEFT) || inputMan->wiiKeyLeft)
		result += Ogre::Vector3(-500,0,0);
	if(inputMan->keyDown(OIS::KC_RIGHT) || inputMan->wiiKeyRight)
		result += Ogre::Vector3(500,0,0);

	m_pCamera->moveRelative(result * secondsElapsed);	
	Ogre::Vector3 pos = m_pCamera->getPosition();
	pos.y = std::max(50.0, (double) pos.y);
	m_pCamera->setPosition(pos);
}

// OIS::MouseListener
bool CameraController::mouseMoved( const OIS::MouseEvent &arg ){
	m_pCamera->yaw(Ogre::Degree(arg.state.X.rel * -0.1f));
	m_pCamera->pitch(Ogre::Degree(arg.state.Y.rel * -0.1f));
	return false;
}

bool CameraController::mousePressed( const OIS::MouseEvent &arg, OIS::MouseButtonID id ){
	return true;
}
bool CameraController::mouseReleased( const OIS::MouseEvent &arg, OIS::MouseButtonID id ){
	return true;
}