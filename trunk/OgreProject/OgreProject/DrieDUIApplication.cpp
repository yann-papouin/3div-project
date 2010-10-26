/*
-----------------------------------------------------------------------------
Filename:    DrieDUIApplication.cpp
-----------------------------------------------------------------------------
*/

#include "DrieDUIApplication.h"

//-------------------------------------------------------------------------------------
DrieDUIApplication::DrieDUIApplication(void)
{
	it=0;
}
//-------------------------------------------------------------------------------------
DrieDUIApplication::~DrieDUIApplication(void)
{

}

//-------------------------------------------------------------------------------------
void DrieDUIApplication::createScene(void)
{
	//mCamera->setPosition(Ogre::Vector3(0, 500, -2500));
	//mCamera->lookAt(Ogre::Vector3(0,0,0));

	//Lichten (en schaduws) aanmaken
	createLight();

	//De grond en skybox instellen
	createGroundAndSky();

	//(Selecteerbare) objecten aanmaken
	createObjects();
}

void DrieDUIApplication::createLight(void){
	//Ambient light
	mSceneMgr->setAmbientLight(Ogre::ColourValue(0.5, 0.5, 0.5));
	//Schaduwtype
	mSceneMgr->setShadowTechnique(Ogre::SHADOWTYPE_STENCIL_ADDITIVE);

	// Een directional light
	Ogre::Light* directionalLight = mSceneMgr->createLight("directionalLight");
	directionalLight->setType(Ogre::Light::LT_DIRECTIONAL);
	directionalLight->setDiffuseColour(Ogre::ColourValue(.25, .25, 0));
	directionalLight->setSpecularColour(Ogre::ColourValue(.25, .25, 0));

	directionalLight->setDirection(Ogre::Vector3( 0, -1, 1 )); 


}
void DrieDUIApplication::createGroundAndSky(void){
	// De grond 
	Ogre::Plane plane(Ogre::Vector3::UNIT_Y, 0);

	Ogre::MeshManager::getSingleton().createPlane("ground", Ogre::ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME,
		plane, 10000, 10000, 20, 20, true, 1, 5, 5, Ogre::Vector3::UNIT_Z);

	Ogre::Entity* entGround = mSceneMgr->createEntity("GroundEntity", "ground");
	mSceneMgr->getRootSceneNode()->createChildSceneNode()->attachObject(entGround);

	entGround->setMaterialName("Examples/Rockwall");
	entGround->setCastShadows(false);

	//lucht
	mSceneMgr->setSkyBox(true, "Examples/SpaceSkyBox");

}
void DrieDUIApplication::createObjects(void){
	m_pRenderObjectManager = new RenderObjectManager(mSceneMgr);
	//m_pRenderObjectManager->addCrate(Ogre::Vector3( -300, 90, -300) , Ogre::Radian(0), 10);


	//+++++++++++++++++++++++++
	Ogre::Entity* ninja = mSceneMgr->createEntity("Ninja", "ninja.mesh");
	Ogre::SceneNode* ninjaNode = mSceneMgr->getRootSceneNode()->createChildSceneNode("ninjaNode1");
	ninjaNode->attachObject(ninja);
	//+++++++++++++++++++++++++
	//Ogre::Entity* athena = mSceneMgr->createEntity( "Athena", "athene.mesh" );
	//Ogre::SceneNode* athenaNode = mSceneMgr->getRootSceneNode()->createChildSceneNode( "athenaNode", Ogre::Vector3( -300, 90, -300) );
	//athenaNode->attachObject( athena );
	//+++++++++++++++++++++++++
	Ogre::Entity* tudorHouse = mSceneMgr->createEntity( "tudorHouse", "tudorhouse.mesh" );
	Ogre::SceneNode* tudorHouseNode = mSceneMgr->getRootSceneNode()->createChildSceneNode( "tudorHouseNode", Ogre::Vector3( 1500, 550, 1500) );
	tudorHouseNode->attachObject( tudorHouse );
	//+++++++++++++++++++++++++
	Ogre::Entity* ninja2 = mSceneMgr->createEntity( "Ninja2", "ninja.mesh" );
	Ogre::SceneNode* ninjaNode2 = mSceneMgr->getRootSceneNode()->createChildSceneNode( "ninjaNode2", Ogre::Vector3( 300, 0, -300) );
	ninjaNode2->attachObject( ninja2 );
	//+++++++++++++++++++++++++
	Ogre::Entity* ninja3 = mSceneMgr->createEntity( "Ninja3", "ninja.mesh" );
	Ogre::SceneNode* ninjaNode3 = mSceneMgr->getRootSceneNode()->createChildSceneNode( "ninjaNode3", Ogre::Vector3( 500, 0, -500) );
	ninjaNode3->attachObject( ninja3 );
	//+++++++++++++++++++++++++
	Ogre::Entity* jaiqua = mSceneMgr->createEntity( "Jaiqua", "jaiqua.mesh" );
	Ogre::SceneNode* jaiquaNode = mSceneMgr->getRootSceneNode()->createChildSceneNode( "jaiquaNode", Ogre::Vector3( -500, 0, -500) );
	jaiquaNode->attachObject( jaiqua );
	//+++++++++++++++++++++++++
	Ogre::Entity* jaiqua2 = mSceneMgr->createEntity( "Jaiqua2", "jaiqua.mesh" );
	Ogre::SceneNode* jaiquaNode2 = mSceneMgr->getRootSceneNode()->createChildSceneNode( "jaiquaNode2", Ogre::Vector3( 0, 0, -100) );
	jaiquaNode2->attachObject( jaiqua2 );
	//+++++++++++++++++++++++++
	Ogre::Entity* jaiqua3 = mSceneMgr->createEntity( "Jaiqua3", "jaiqua.mesh" );
	Ogre::SceneNode* jaiquaNode3 = mSceneMgr->getRootSceneNode()->createChildSceneNode( "jaiquaNode3", Ogre::Vector3( 750, 0, -500) );
	jaiquaNode3->attachObject( jaiqua3 );
	//+++++++++++++++++++++++++
	Ogre::Entity* sibenik = mSceneMgr->createEntity( "sibenik", "sibenik.mesh" );
	Ogre::SceneNode* sibenikNode = mSceneMgr->getRootSceneNode()->createChildSceneNode( "sibenikNode", Ogre::Vector3( 500, 0, 0) );
	sibenikNode->attachObject( sibenik );
	//+++++++++++++++++++++++++
	Ogre::Entity* ShaderSystem = mSceneMgr->createEntity( "ShaderSystem", "ShaderSystem.mesh" );
	Ogre::SceneNode* ShaderSystemNode = mSceneMgr->getRootSceneNode()->createChildSceneNode( "ShaderSystemNode", Ogre::Vector3( 1500, 0, 400) );
	ShaderSystemNode->attachObject( ShaderSystem );
	//+++++++++++++++++++++++++

	selectedNode = mSceneMgr->getRootSceneNode()->getChild("ninjaNode1");
	it++;
}
void DrieDUIApplication::manipulateNode(Ogre::Node* node)
{
	if(node != NULL)
	{
		Ogre::Quaternion q(0.9,0.3,0.2,0.4);
		node->rotate(q);
	}
}
bool DrieDUIApplication::keyPressed( const OIS::KeyEvent &arg )
{
	if (mTrayMgr->isDialogVisible()) return true;   // don't process any more keys if dialog is up

	if(arg.key == OIS::KC_F1)
	{
		switch(it%3)
		{
		case 0:
			selectedNode = mSceneMgr->getRootSceneNode()->getChild("ninjaNode1");
			break;
		case 1:
			selectedNode = mSceneMgr->getRootSceneNode()->getChild("ninjaNode2");
			break;
		case 2:
			selectedNode = mSceneMgr->getRootSceneNode()->getChild("ninjaNode3");
			break;
		}
		it++;

		return false;
	}
	else if(arg.key == OIS::KC_R)
	{
		manipulateNode(selectedNode);
		return false;
	}
	else if (arg.key == OIS::KC_F)   // toggle visibility of advanced frame stats
	{
		mTrayMgr->toggleAdvancedFrameStats();
		return false;
	}
	else if (arg.key == OIS::KC_G)   // toggle visibility of even rarer debugging details
	{
		if (mDetailsPanel->getTrayLocation() == OgreBites::TL_NONE)
		{
			mTrayMgr->moveWidgetToTray(mDetailsPanel, OgreBites::TL_TOPRIGHT, 0);
			mDetailsPanel->show();
		}
		else
		{
			mTrayMgr->removeWidgetFromTray(mDetailsPanel);
			mDetailsPanel->hide();
		}
		return false;
	}
	else if (arg.key == OIS::KC_T)   // cycle polygon rendering mode
	{
		Ogre::String newVal;
		Ogre::TextureFilterOptions tfo;
		unsigned int aniso;

		switch (mDetailsPanel->getParamValue(9).asUTF8()[0])
		{
		case 'B':
			newVal = "Trilinear";
			tfo = Ogre::TFO_TRILINEAR;
			aniso = 1;
			break;
		case 'T':
			newVal = "Anisotropic";
			tfo = Ogre::TFO_ANISOTROPIC;
			aniso = 8;
			break;
		case 'A':
			newVal = "None";
			tfo = Ogre::TFO_NONE;
			aniso = 1;
			break;
		default:
			newVal = "Bilinear";
			tfo = Ogre::TFO_BILINEAR;
			aniso = 1;
		}

		Ogre::MaterialManager::getSingleton().setDefaultTextureFiltering(tfo);
		Ogre::MaterialManager::getSingleton().setDefaultAnisotropy(aniso);
		mDetailsPanel->setParamValue(9, newVal);
		return false;
	}
	else if (arg.key == OIS::KC_R)   // cycle polygon rendering mode
	{
		Ogre::String newVal;
		Ogre::PolygonMode pm;

		switch (mCamera->getPolygonMode())
		{
		case Ogre::PM_SOLID:
			newVal = "Wireframe";
			pm = Ogre::PM_WIREFRAME;
			break;
		case Ogre::PM_WIREFRAME:
			newVal = "Points";
			pm = Ogre::PM_POINTS;
			break;
		default:
			newVal = "Solid";
			pm = Ogre::PM_SOLID;
		}

		mCamera->setPolygonMode(pm);
		mDetailsPanel->setParamValue(10, newVal);
		return false;
	}
	else if(arg.key == OIS::KC_F5)   // refresh all textures
	{
		Ogre::TextureManager::getSingleton().reloadAll();
		return false;
	}
	else if (arg.key == OIS::KC_SYSRQ)   // take a screenshot
	{
		mWindow->writeContentsToTimestampedFile("screenshot", ".jpg");
		return false;
	}
	else if (arg.key == OIS::KC_ESCAPE)
	{
		mShutDown = true;
		return false;
	}

	return true;
}
#if OGRE_PLATFORM == OGRE_PLATFORM_WIN32
#define WIN32_LEAN_AND_MEAN
#include "windows.h"
#endif

#ifdef __cplusplus
extern "C" {
#endif

#if OGRE_PLATFORM == OGRE_PLATFORM_WIN32
	INT WINAPI WinMain( HINSTANCE hInst, HINSTANCE, LPSTR strCmdLine, INT )
#else
	int main(int argc, char *argv[])
#endif
	{
		// Create application object
		DrieDUIApplication app;

		try {
			app.go();
		} catch( Ogre::Exception& e ) {
#if OGRE_PLATFORM == OGRE_PLATFORM_WIN32
			MessageBox( NULL, e.getFullDescription().c_str(), "An exception has occured!", MB_OK | MB_ICONERROR | MB_TASKMODAL);
#else
			std::cerr << "An exception has occured: " <<
				e.getFullDescription().c_str() << std::endl;
#endif
		}

		return 0;
	}

#ifdef __cplusplus
}
#endif
