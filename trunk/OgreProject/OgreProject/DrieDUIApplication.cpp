/*
-----------------------------------------------------------------------------
Filename:    DrieDUIApplication.cpp
-----------------------------------------------------------------------------
*/
#include "DrieDUIApplication.h"

//-------------------------------------------------------------------------------------
DrieDUIApplication::DrieDUIApplication(void)
{
}
//-------------------------------------------------------------------------------------
DrieDUIApplication::~DrieDUIApplication(void)
{
}

//-------------------------------------------------------------------------------------
void DrieDUIApplication::createScene(void)
{
	// ALS JE SHIFT INDRUKT DAN BEWEEG JE SNELLER
	// ALS JE SHIFT INDRUKT DAN BEWEEG JE SNELLER
	// ALS JE SHIFT INDRUKT DAN BEWEEG JE SNELLER
	// ALS JE SHIFT INDRUKT DAN BEWEEG JE SNELLER
	// ALS JE SHIFT INDRUKT DAN BEWEEG JE SNELLER
	// ALS JE SHIFT INDRUKT DAN BEWEEG JE SNELLER
	// SPAM
	// ALS JE SHIFT INDRUKT DAN BEWEEG JE SNELLER
// ALS JE SHIFT INDRUKT DAN BEWEEG JE SNELLER
	// ALS JE SHIFT INDRUKT DAN BEWEEG JE SNELLER
// ALS JE SHIFT INDRUKT DAN BEWEEG JE SNELLER
// ALS JE SHIFT INDRUKT DAN BEWEEG JE SNELLER
	// ALS JE SHIFT INDRUKT DAN BEWEEG JE SNELLER
// ALS JE SHIFT INDRUKT DAN BEWEEG JE SNELLER
	// ALS JE SHIFT INDRUKT DAN BEWEEG JE SNELLER

	mCamera->setPosition(Ogre::Vector3(0, 500, -2500));
	mCamera->lookAt(Ogre::Vector3(0,0,0));

	//Lichten (en schaduws) aanmaken
	createLight();

	//De grond en skybox instellen
	createGroundAndSky();

	//(Selecteerbare) objecten aanmaken
	createObjects();
}

//-------------------------------------------------------------------------------------

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

//-------------------------------------------------------------------------------------
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

//-------------------------------------------------------------------------------------
void DrieDUIApplication::createObjects(void){
	//+++++++++++++++++++++++++
	Ogre::Entity* ninja = mSceneMgr->createEntity("Ninja", "ninja.mesh");
    Ogre::SceneNode* ninjaNode = mSceneMgr->getRootSceneNode()->createChildSceneNode();
    ninjaNode->attachObject(ninja);
	//+++++++++++++++++++++++++
	Ogre::Entity* athena = mSceneMgr->createEntity( "Athena", "athene.mesh" );
	Ogre::SceneNode* athenaNode = mSceneMgr->getRootSceneNode()->createChildSceneNode( "athenaNode", Ogre::Vector3( -300, 90, -300) );
	athenaNode->attachObject( athena );
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
