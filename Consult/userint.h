#ifndef __USERINT_H__
#define __USERINT_H__

//Macros
#define	SkipSpace	while ( (c = getchar()) == ' ' || c == '\t' )

//Extern Variables
extern char Delimiter;

//Prototypes
void CheckValue (Attribute Att, Tree T);
void PrintRange (Attribute Att);
void ReadRange (Attribute Att, Tree T);
void ReadDiscr (Attribute Att,Tree T);
void ReadContin (Attribute Att, Tree T);
void Retry (Attribute Att, Tree T);
void SkipLine(char c);
void Clear();

#endif //__USERINT_H__