#ifndef RENDEROBJ_H
#define RENDEROBJ_H

#include <string>
#include <vector>
#include "Ogre.h"
using namespace std;

#define OBJECT 1 // removable, able to change rot/pos/color/size
#define WALL 2	 // able to change color

class RenderObject
{
public:
	RenderObject(Ogre::SceneManager *sceneMgr, Ogre::String entName, Ogre::String meshName, 
		Ogre::Vector3 position,	Ogre::Radian rotation=Ogre::Radian(0), 
		Ogre::Real scaleFactor = 1, int type = WALL, int ID = -1);
	
	~RenderObject(void);

	void setPosition(Ogre::Vector3 newpos);
	void scaleModel(Ogre::Real m);						   	
	void addRotation(const Ogre::Radian &angle);
								 	
	
	Ogre::String getName() { return m_entName; }
	Ogre::Entity* getEntity(){return m_entity;};
	Ogre::SceneNode * getBaseNode() { return m_baseNode; }
	Ogre::SceneNode * getModelNode() { return m_modelNode; }
	Ogre::Vector3 getPosition() { return m_position; }
	Ogre::Radian getRotation() { return m_rotation; }
	int getType(){ return m_type; }
	unsigned long getID(){ return m_ID; }

	static unsigned long getNextID(){ return ++maxID;}  
	static unsigned long maxID;

private:
	Ogre::SceneManager *m_sceneMgr;
	Ogre::Entity *m_entity;
	Ogre::SceneNode *m_baseNode, *m_modelNode;
	Ogre::RaySceneQuery *m_rayQuery;
	Ogre::String m_entName;
	Ogre::Vector3 m_position;
	Ogre::Radian m_rotation;
	int m_type; 
	unsigned long m_ID;
};


#endif

