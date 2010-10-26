#include "RenderObjectManager.h"


RenderObjectManager::RenderObjectManager(Ogre::SceneManager* scnMgr):m_psceneMgr(scnMgr)
{
}


RenderObjectManager::~RenderObjectManager(void)
{
}

void RenderObjectManager::addCrate(Ogre::Vector3 position,	Ogre::Radian rotation, Ogre::Real scaleFactor){
	unsigned long ID = RenderObject::getNextID();
	RenderObject* object = new RenderObject(m_psceneMgr, "Entity_crate"+Ogre::StringConverter::toString(ID), 
		"crate.mesh", position, rotation, scaleFactor, OBJECT, ID);
	m_renderObjects.push_back(object);
}

RenderObject* RenderObjectManager::getRenderObject(int ID){
	for(unsigned int i = 0; i < m_renderObjects.size(); ++i)
		if(m_renderObjects[i]->getID() == ID)
			return 	m_renderObjects[i];

	return 0;
}
