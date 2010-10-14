/*
-----------------------------------------------------------------------------
Filename:    DrieDUIApplication.h
-----------------------------------------------------------------------------
*/
#ifndef __DrieDUIApplication_h_
#define __DrieDUIApplication_h_

#include "BaseApplication.h"

class DrieDUIApplication : public BaseApplication
{
public:
    DrieDUIApplication(void);
    virtual ~DrieDUIApplication(void);

protected:
    virtual void createScene(void);
	void createLight(void);
	void createGroundAndSky(void);
	void createObjects(void);
};

#endif // #ifndef __DrieDUIApplication_h_
