#ifndef __USERINT_H__
#define __USERINT_H__

#include <esolutions.h>
//Macros
#define	SkipSpace	while ( (c = getchar()) == ' ' || c == '\t' )

//Extern Variables
extern char Delimiter;

//Prototypes
void CheckValue (Attribute Att, Tree T, FEATURES features[MAX_FEATURES]);
void PrintRange (Attribute Att);
void ReadRange (Attribute Att, Tree T, FEATURES features[MAX_FEATURES]);
void ReadDiscr (Attribute Att,Tree T);
void ReadContin (Attribute Att, Tree T);
void Retry (Attribute Att, Tree T);
void SkipLine(char c);
void Clear();

#endif //__USERINT_H__