#include "RenderObject.h"

unsigned long RenderObject::maxID = 0;

RenderObject::RenderObject(Ogre::SceneManager *sceneMgr, Ogre::String entName, Ogre::String meshName, 
	Ogre::Vector3 position,	Ogre::Radian rotation, 
	Ogre::Real scaleFactor, int type, int ID): m_sceneMgr(sceneMgr), 
	m_entName(entName), m_position(position), m_rotation(Ogre::Radian(rotation)), m_ID(ID)
{
	m_type = type;
	m_entity = m_sceneMgr->createEntity(entName, meshName);
	m_entity->setCastShadows(true);
	m_entity->setQueryFlags(type); // nodig voor picking

	// toevoegen aan scene
	m_baseNode = m_sceneMgr->getRootSceneNode()->createChildSceneNode(position);
	m_modelNode = m_baseNode->createChildSceneNode();
	m_modelNode->attachObject(m_entity);
	m_modelNode->rotate(Ogre::Vector3(0,1,0), Ogre::Radian(rotation));
	scaleModel(scaleFactor);

	m_rayQuery = m_sceneMgr->createRayQuery(Ogre::Ray(m_baseNode->getPosition(), Ogre::Vector3::NEGATIVE_UNIT_Y));
	m_entity->setVisible(true);
	m_baseNode->setVisible(true);
	m_modelNode->setVisible(true);
}


RenderObject::~RenderObject(void)
{
}

void RenderObject::setPosition(Ogre::Vector3 newpos){
	m_position = newpos;
}

void RenderObject::scaleModel(Ogre::Real m){
	m_modelNode->scale(m, m, m);
}

void RenderObject::addRotation(const Ogre::Radian &angle){
	m_modelNode->rotate(Ogre::Vector3(0,1,0), angle);
	m_rotation += angle;
}
