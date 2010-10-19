#ifndef __InputManager_H__
#define __InputManager_H__

#include <OISMouse.h>
#include <OISKeyboard.h>
#include <OISInputManager.h>
#include <OgreRenderWindow.h>
#include <iostream>

using namespace std;

class InputManager : public OIS::KeyListener, public OIS::MouseListener {
public:
	InputManager();
	~InputManager( void );

	void initialise( Ogre::RenderWindow *renderWindow, bool keyboard = true, bool mouse = true );
	void capture( void ); // in de main-loop aanroepen

	void addKeyListener( OIS::KeyListener *keyListener, const std::string& instanceName );
	void addMouseListener( OIS::MouseListener *mouseListener, const std::string& instanceName );

	void removeKeyListener( const std::string& instanceName );
	void removeMouseListener( const std::string& instanceName );

	// om te checken op de gegeven input
	bool keyDown(OIS::KeyCode k) { return m_Keyboard->isKeyDown(k); }
	bool noKeyPressed() { return (!m_Keyboard->isKeyDown(OIS::KC_W) && !m_Keyboard->isKeyDown(OIS::KC_A) && !m_Keyboard->isKeyDown(OIS::KC_S) && !m_Keyboard->isKeyDown(OIS::KC_D)); }

	void removeKeyListener( OIS::KeyListener *keyListener );
	void removeMouseListener( OIS::MouseListener *mouseListener );

	void removeAllListeners( void );
	void removeAllKeyListeners( void );
	void removeAllMouseListeners( void );

	void setWindowExtents( int width, int height );

	OIS::Mouse*    getMouse( void );
	OIS::Keyboard* getKeyboard( void );

	static InputManager* getSingletonPtr( void );
private:
	InputManager( const InputManager& ) { }
	InputManager & operator = ( const InputManager& );

	bool keyPressed( const OIS::KeyEvent &e );
	bool keyReleased( const OIS::KeyEvent &e );

	bool mouseMoved( const OIS::MouseEvent &e );
	bool mousePressed( const OIS::MouseEvent &e, OIS::MouseButtonID id );
	bool mouseReleased( const OIS::MouseEvent &e, OIS::MouseButtonID id );

	OIS::InputManager *m_InputSystem;
	OIS::Mouse        *m_Mouse;
	OIS::Keyboard     *m_Keyboard;

	std::map<std::string, OIS::KeyListener*> m_KeyListeners;
	std::map<std::string, OIS::MouseListener*> m_MouseListeners;

	std::map<std::string, OIS::KeyListener*>::iterator itKeyListener;
	std::map<std::string, OIS::MouseListener*>::iterator itMouseListener;

	std::map<std::string, OIS::KeyListener*>::iterator itKeyListenerEnd;
	std::map<std::string, OIS::MouseListener*>::iterator itMouseListenerEnd;

	static InputManager *m_InputManager;
};

#endif