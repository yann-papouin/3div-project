#ifndef RENDEROBJMAN_H
#define RENDEROBJMAN_H

#include "RenderObject.h"
#include <OgreSceneManager.h>
#include <vector>
using namespace std;

class RenderObjectManager
{
public:
	RenderObjectManager(Ogre::SceneManager* scnMgr);
	~RenderObjectManager(void);

	void addCrate(Ogre::Vector3 position,	Ogre::Radian rotation, Ogre::Real scaleFactor);
	RenderObject* getRenderObject(int ID);

private:
	vector<RenderObject*> m_renderObjects;		
	Ogre::SceneManager *m_psceneMgr;
};

#endif
