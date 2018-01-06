/*************************************************************************/
/*									 */
/*	Get names of classes, attributes and attribute values		 */
/*	-----------------------------------------------------		 */
/*									 */
/*************************************************************************/
#ifndef __GETNAMES_CPP__
#define __GETNAMES_CPP__

#include <sys/types.h>
#include <sys/stat.h>

#include "header.h"
#include "getnames.h"

/*************************************************************************/
/*									 */
/*  Read a name from file f into string s, setting Delimiter.		 */
/*									 */
/*  - Embedded periods are permitted, but periods followed by space	 */
/*    characters act as delimiters.					 */
/*  - Embedded spaces are permitted, but multiple spaces are replaced	 */
/*    by a single space.						 */
/*  - Any character can be escaped by '\'.				 */
/*  - The remainder of a line following '|' is ignored.			 */
/*									 */
/*************************************************************************/


/*************************************************************************/
/*									 */
/*  Read the names of classes, attributes and legal attribute values.	 */
/*  On completion, these names are stored in:				 */
/*	ClassName	-  class names					 */
/*	AttName		-  attribute names				 */
/*	AttValName	-  attribute value names			 */
/*  with:								 */
/*	MaxAttVal	-  number of values for each attribute		 */
/*									 */
/*  Other global variables set are:					 */
/*	MaxAtt		-  maximum attribute number			 */
/*	MaxClass	-  maximum class number				 */
/*	MaxDiscrVal	-  maximum discrete values for any attribute	 */
/*									 */
/*  Note:  until the number of attributes is known, the name		 */
/*	   information is assembled in local arrays			 */
/*									 */
/*************************************************************************/


void GetNames (void)
{
    FILE *Nf = Nil;
	 char Fn[MAX_CHAR] = {0}, Buffer[1000] = {0};
    DiscrValue v = 0.0;
    int AttCeiling=100, ClassCeiling=100, ValCeiling = 0;

    /*  Open names file  */

    //strcpy(Fn, FileName);
	 StringCbCopy (Fn, MAX_CHAR, FileName);	 
    //strcat(Fn, ".names");
	 StringCbCatA (Fn, MAX_CHAR, ".names");
    if ( ! ( Nf = fopen(Fn, "r") ) ) Error(0, Fn, "");

    /*  Get class names from names file  */

    ClassName = (C45_String *) calloc(ClassCeiling, sizeof(C45_String));
    MaxClass = -1;
    do
    {
	ReadName(Nf, Buffer);

	if ( ++MaxClass >= ClassCeiling)
	{
	    ClassCeiling += 100;
	    ClassName = (C45_String *) realloc(ClassName, ClassCeiling*sizeof(C45_String));
	}
	ClassName[MaxClass] = CopyString(Buffer);
    }
    while ( Delimiter == ',' );

    /*  Get attribute and attribute value names from names file  */

    AttName = (C45_String *) calloc(AttCeiling, sizeof(C45_String));
    MaxAttVal = (DiscrValue *) calloc(AttCeiling, sizeof(DiscrValue));
    AttValName = (C45_String **) calloc(AttCeiling, sizeof(C45_String *));
    SpecialStatus = (char *) malloc(AttCeiling);

    MaxAtt = -1;
    while ( ReadName(Nf, Buffer) )
    {
	if ( Delimiter != ':' ) Error(1, Buffer, "");

	if ( ++MaxAtt >= AttCeiling )
	{
	    AttCeiling += 100;
	    AttName = (C45_String *) realloc(AttName, AttCeiling*sizeof(C45_String));
	    MaxAttVal = (DiscrValue *) realloc(MaxAttVal, AttCeiling*sizeof(DiscrValue));
	    AttValName = (C45_String **) realloc(AttValName, AttCeiling*sizeof(C45_String *));
	    SpecialStatus = (char *) realloc(SpecialStatus, AttCeiling);
	}

	AttName[MaxAtt] = CopyString(Buffer);
	SpecialStatus[MaxAtt] = Nil;
	MaxAttVal[MaxAtt] = 0;
	ValCeiling = 100;
	AttValName[MaxAtt] = (C45_String *) calloc(ValCeiling, sizeof(C45_String));

	do
	{
	    if ( ! ( ReadName(Nf, Buffer) ) ) Error(2, AttName[MaxAtt], "");

	    if ( ++MaxAttVal[MaxAtt] >= ValCeiling )
	    {
		ValCeiling += 100;
		AttValName[MaxAtt] =
		    (C45_String *) realloc(AttValName[MaxAtt], ValCeiling*sizeof(C45_String));
	    }

	    AttValName[MaxAtt][MaxAttVal[MaxAtt]] = CopyString(Buffer);
	}
	while ( Delimiter == ',' );

	if ( MaxAttVal[MaxAtt] == 1 )
	{
	    /*  Check for special treatment  */

	    if ( ! strcmp(Buffer, "continuous") )
	    {}
	    else
	    if ( ! memcmp(Buffer, "discrete", 8) )
	    {
		SpecialStatus[MaxAtt] = DISCRETE;

		/*  Read max values, reserve space and check MaxDiscrVal  */

		v = atoi(&Buffer[8]);
		if ( v < 2 )
		{
		    StringCbPrintf(szBuffer, cbDest, "** %s: illegal number of discrete values\n",
			   AttName[MaxAtt]);
			 SendOutput (szBuffer);
		    exit(1);
		}

		AttValName[MaxAtt] =
		    (C45_String *) realloc(AttValName[MaxAtt], (v+2)*sizeof(C45_String));
		AttValName[MaxAtt][0] = (char *) v;
		if ( v > MaxDiscrVal ) MaxDiscrVal = v;
	    }
	    else
	    if ( ! strcmp(Buffer, "ignore") )
	    {
		SpecialStatus[MaxAtt] = IGNORE;
	    }
	    else
	    {
		/*  Cannot have only one discrete value for an attribute  */

		Error(3, AttName[MaxAtt], "");
	    }

	    MaxAttVal[MaxAtt] = 0;
	}
	else
	if ( MaxAttVal[MaxAtt] > MaxDiscrVal ) MaxDiscrVal = MaxAttVal[MaxAtt];
    }

    fclose(Nf);
}
#endif //__GETNAMES_CPP__