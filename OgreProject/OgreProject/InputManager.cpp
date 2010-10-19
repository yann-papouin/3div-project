#include "InputManager.h"

InputManager *InputManager::m_InputManager;

//Callbacks
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
		out << b.button << " ingedrukt";
		MessageBox(NULL, out.str().c_str(), "Knop ingedauwd",  MB_OK | MB_ICONERROR | MB_TASKMODAL);
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
		InputManager *inp = InputManager::getSingletonPtr();
		//(*feedback) = out.str();
	}
	else
	{
		//(*feedback) = "Geen knop ingedrukt";
	}
}

InputManager::InputManager( void ) :
m_Mouse( 0 ),
	m_Keyboard( 0 ),
	m_InputSystem( 0 ) {
}

InputManager::~InputManager( void ) {
	if( m_InputSystem ) {
		if( m_Mouse ) {
			m_InputSystem->destroyInputObject( m_Mouse );
			m_Mouse = 0;
		}

		if( m_Keyboard ) {
			m_InputSystem->destroyInputObject( m_Keyboard );
			m_Keyboard = 0;
		}

		m_InputSystem->destroyInputSystem( m_InputSystem );
		m_InputSystem = 0;

		m_KeyListeners.clear();
		m_MouseListeners.clear();
	}
}

void InputManager::initialise( Ogre::RenderWindow *renderWindow, bool keyboard, bool mouse ) {
	if( !m_InputSystem ) {
		OIS::ParamList paramList;    
		size_t windowHnd = 0;
		std::ostringstream windowHndStr;

		renderWindow->getCustomAttribute( "WINDOW", &windowHnd );

		windowHndStr << (unsigned int) windowHnd;
		paramList.insert( std::make_pair( std::string( "WINDOW" ), windowHndStr.str() ) );

		m_InputSystem = OIS::InputManager::createInputSystem( paramList );
		vrpnAnalog = new vrpn_Analog_Remote("device0@localhost");
		vrpnButton = new vrpn_Button_Remote("device0@localhost");
		vrpnKeyboard = new vrpn_Button_Remote("Keyboard0@localhost");

		//Volgende functies kloppen wel, maar zijn commented om die feedback te kunnen tonen
		//vrpnAnalog->register_change_handler(&feedback, handle_analog);//(void*)feedback.c_str() -> userdata?
		vrpnButton->register_change_handler(0, handle_button);//(void*)feedback.c_str()
		//vrpnKeyboard->register_change_handler(&feedback, handle_keyboard);

		if( keyboard ) {
			m_Keyboard = static_cast<OIS::Keyboard*>( m_InputSystem->createInputObject( OIS::OISKeyboard, true ) );
			m_Keyboard->setEventCallback( this );
		}

		if( mouse ) {
			m_Mouse = static_cast<OIS::Mouse*>( m_InputSystem->createInputObject( OIS::OISMouse, true ) );
			m_Mouse->setEventCallback( this );

			unsigned int width, height, depth;
			int left, top;
			renderWindow->getMetrics( width, height, depth, left, top );

			this->setWindowExtents( width, height );
		}
	}
}

void InputManager::capture( void ) {
	if( m_Mouse )
		m_Mouse->capture();

	if( m_Keyboard )
		m_Keyboard->capture();
	//vrpnc->loopAllRemotes();
	vrpnButton->mainloop();
	vrpnAnalog->mainloop();
}

void InputManager::addKeyListener( OIS::KeyListener *keyListener, const std::string& instanceName ) {
	if( m_Keyboard ) {
		itKeyListener = m_KeyListeners.find( instanceName );
		if( itKeyListener == m_KeyListeners.end() ) {
			m_KeyListeners[ instanceName ] = keyListener;
		}
	}
}

void InputManager::addMouseListener( OIS::MouseListener *mouseListener, const std::string& instanceName ) {
	if( m_Mouse ) {
		itMouseListener = m_MouseListeners.find( instanceName );
		if( itMouseListener == m_MouseListeners.end() ) {
			m_MouseListeners[ instanceName ] = mouseListener;
		}
	}
}

void InputManager::removeKeyListener( const std::string& instanceName ) {
	itKeyListener = m_KeyListeners.find( instanceName );
	if( itKeyListener != m_KeyListeners.end() ) {
		m_KeyListeners.erase( itKeyListener );
	}
}

void InputManager::removeMouseListener( const std::string& instanceName ) {
	itMouseListener = m_MouseListeners.find( instanceName );
	if( itMouseListener != m_MouseListeners.end() ) {
		m_MouseListeners.erase( itMouseListener );
	}
}

void InputManager::removeKeyListener( OIS::KeyListener *keyListener ) {
	for(itKeyListener=m_KeyListeners.begin();itKeyListener!=m_KeyListeners.end();++itKeyListener ) {
		if( itKeyListener->second == keyListener ) {
			m_KeyListeners.erase( itKeyListener );
			break;
		}
	}
}

void InputManager::removeMouseListener( OIS::MouseListener *mouseListener ) {
	for(itMouseListener=m_MouseListeners.begin();itMouseListener!=m_MouseListeners.end();++itMouseListener ) {
		if( itMouseListener->second == mouseListener ) {
			m_MouseListeners.erase( itMouseListener );
			break;
		}
	}
}

void InputManager::removeAllListeners( void ) {
	m_KeyListeners.clear();
	m_MouseListeners.clear();
}

void InputManager::removeAllKeyListeners( void ) {
	m_KeyListeners.clear();
}

void InputManager::removeAllMouseListeners( void ) {
	m_MouseListeners.clear();
}

void InputManager::setWindowExtents( int width, int height ) {
	const OIS::MouseState &mouseState = m_Mouse->getMouseState();
	mouseState.width  = width;
	mouseState.height = height;
}

OIS::Mouse* InputManager::getMouse( void ) {
	return m_Mouse;
}

OIS::Keyboard* InputManager::getKeyboard( void ) {
	return m_Keyboard;
}

bool InputManager::keyPressed( const OIS::KeyEvent &e ) {
	for(itKeyListener=m_KeyListeners.begin();itKeyListener!=m_KeyListeners.end();++itKeyListener ) {
		if(!itKeyListener->second->keyPressed( e ))
			break;
		if(itKeyListener == m_KeyListeners.end())
			break;
	}

	return true;
}

bool InputManager::keyReleased( const OIS::KeyEvent &e ) {
	for(itKeyListener=m_KeyListeners.begin();itKeyListener!=m_KeyListeners.end();++itKeyListener ) {
		if(!itKeyListener->second->keyReleased( e ))
			break;
		if(itKeyListener == m_KeyListeners.end())
			break;
	}

	return true;
}

bool InputManager::mouseMoved( const OIS::MouseEvent &e ) {
	for(itMouseListener=m_MouseListeners.begin();itMouseListener!=m_MouseListeners.end();++itMouseListener ) {
		if(!itMouseListener->second->mouseMoved( e ))
			break;
		if(itMouseListener==m_MouseListeners.end())
			break;
	}

	return true;
}

bool InputManager::mousePressed( const OIS::MouseEvent &e, OIS::MouseButtonID id ) {
	for(itMouseListener=m_MouseListeners.begin();itMouseListener!=m_MouseListeners.end();++itMouseListener ) {
		if(!itMouseListener->second->mousePressed( e, id ))
			break;
		if(itMouseListener==m_MouseListeners.end())
			break;
	}

	return true;
}

bool InputManager::mouseReleased( const OIS::MouseEvent &e, OIS::MouseButtonID id ) {
	for(itMouseListener=m_MouseListeners.begin();itMouseListener!=m_MouseListeners.end();++itMouseListener ) {
		if(!itMouseListener->second->mouseReleased( e, id ))
			break;
		if(itMouseListener==m_MouseListeners.end())
			break;

	}

	return true;
}

void InputManager::DmouseListener(void)
{
	//int ID = vrpnc->getFeedback();

}

InputManager* InputManager::getSingletonPtr( void ) {
	if( !m_InputManager )
		m_InputManager = new InputManager();

	return m_InputManager;
}