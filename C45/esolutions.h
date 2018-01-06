#ifndef __ESOLUTIONS_H__
#define __ESOLUTIONS_H__

#include <types.h>

typedef signed int C45_RETCODE;

//C4.5 error/status codes
const C45_RETCODE C45_UNDEFINED_ERROR	= -1;
const C45_RETCODE C45_SUCCESS				=  0;

//Error >= 101 are consult errors
const C45_RETCODE C45_WRONG_ATTRIBUTE_VALUE	= 101;	//Unknown attribute value
const C45_RETCODE	C45_WRONG_PROBYBILITY		= 102;	//Probability values of an attribute does not sum to 1

//eSolutions edit
#define MAX_CHAR						256
#define MAX_DISCRETE_ATTRIBUTES	25
#define MAX_FEATURES					100
#define MAX_OUTCLASSES				25
#define MAX_OUTPUT_SIZE				2048
#define MAX_XML_PACKAGE				100000

//Debug Modes
#define DEBUG_OFF			(int)	0
#define DEBUG_CONSOLE	(int)	1
#define DEBUG_TCP_IP		(int)	2

#pragma pack (push, 4)
typedef struct _discreteFeature
{
	float fProbability;
	char	szAttribute[MAX_CHAR];
} DISCRETEFEATURE, *PDISCRETEFEATURE;

typedef struct _continousFeature
{
	float	fLowerBound;
	float	fUpperBound;
} CONTINOUSFEATURE, *PCONTINOUSFEATURE;

typedef struct _discreteFeatureList
{
	//Horrible! Dummy placeholder to fill out 8 "secret" byte from interop. REFACTOR!!
	int dummy1;
	int dummy2;
	DISCRETEFEATURE	discreteFeature[MAX_DISCRETE_ATTRIBUTES];
} DISCRETEFEATURELIST, *PDISCRETEFEATURELIST;

typedef struct _features
{
	bool						isDiscreteFeature;
	CONTINOUSFEATURE		continousFeature;	
	PDISCRETEFEATURELIST	pDiscreteFeatureList;
	bool						bKnown;
} FEATURES, *PFEATURES;


typedef struct _classBuffer
{
	//Horrible! Dummy placeholder to fill out 8 "secret" byte from interop. REFACTOR!!
	int	dummy1;
	int	dummy2;
	char	szClassBuffer[MAX_CHAR];
} CLASSBUFFER, *PCLASSBUFFER;

typedef struct _outclasses
{	
	float				fBestGuess;
	float				fLowerBound;
	float				fUpperBound;
	PCLASSBUFFER	pClassBuffer;
} OUTCLASSES, *POUTCLASSES;


typedef struct _trace_package
{
	char szHost[MAX_CHAR];
	char szCustomer[MAX_CHAR];
	char szUser[MAX_CHAR];
	char szStation[MAX_CHAR];
	char szApplication[MAX_CHAR];
	char szApplication_version[MAX_CHAR];
	char szModule[MAX_CHAR];
	char szModule_version[MAX_CHAR];
	char szOS[MAX_CHAR];
} TRACE_PACKAGE, *PTRACE_PACKAGE;

typedef struct _statistics
{
	int	iTreeSize;
	int	iTreeGenerationTime;	//in ms
	int	iDecisionTime;			//in ms
} STATISTICS, *PSTATISTICS;

typedef struct _debugParameter
{
	int	iVerbosity;						//0-3
	int	iDebugMode;						//0 = off, 1 = Console, 2 = TCP/IP (Bitmask)
	char	szSnifferHost[MAX_CHAR];	//Debugserver hostname
	int	iSnifferPort;					//TCP-Port of debug server
}	DEBUGPARAMETER, *PDEBUGPARAMETER;

//Prototypes
C45_RETCODE ReadDiscr (Attribute Att, Tree T, FEATURES features[MAX_FEATURES]);
C45_RETCODE ReadContin (Attribute Att, Tree T, FEATURES features[MAX_FEATURES]);
void			TCPInitialize (void);
bool			TCPConnect ();
void			TCPCleanUp (void);
void			SendTCPMessage (char* szBuffer);
int			XMLize (char* szBuffer, char* szXMLStream);
bool			GetHeader (PTRACE_PACKAGE pPackage);
void			GetOSInfo (char* szBuffer);
bool			PrependSize (char* pPackage, char* pBuffer);
void			ConvertPeculiars (char* pXMLStream, char* pNewXMLStream);
char*			ReplaceWord (char* pOldString, char* pSearchWord, char* pReplaceWord, char* pNewString);

#endif //__ESOLUTIONS_H__
