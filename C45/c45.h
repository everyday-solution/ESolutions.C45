#ifndef __C45_DLL__
#define __C45_DLL__

#include <header.h>>

#ifdef __DLL_IMPORT__
	#define DLL extern "C" __declspec (dllimport)
#else
	#define DLL extern "C" __declspec (dllexport)
#endif //__DLL_IMPORT__


DLL C45_RETCODE GenerateDTree (	char*	pszFilename, 
											bool	bGainRatio, 
											bool	bSubtreeRaising, 
											bool	bSoftTresh, 
											int	iConfidenceLevel,
											DEBUGPARAMETER	debugParameter);

DLL C45_RETCODE Consult	(	char*				pszFilename, 
									PFEATURES		pFeatures, 
									int*				pClassesCount, 
									POUTCLASSES*	pOutClasses,
									DEBUGPARAMETER	debugParameter);

DLL C45_RETCODE AddResult	(	char* pszFeatureStream, 
										char* pszClass);

DLL void GetStatistics (PSTATISTICS pStatistics);

//Global variables
#endif //__C45_DLL__