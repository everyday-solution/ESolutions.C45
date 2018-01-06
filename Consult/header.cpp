/*************************************************************************/
/*									 */
/*	Print header for all C4.5 programs				 */
/*	----------------------------------				 */
/*									 */
/*************************************************************************/

#ifndef __HEADER_CPP__
 #define __HEADER_CPP__

#include "header.h"

short		MaxAtt;		/* max att number */
short		MaxClass;	/* max class number */
short		MaxDiscrVal = 2;	/* max discrete values for any att */

ItemNo		MaxItem = 0;	/* max data item number */

Description	*Item;		/* data items */

DiscrValue	*MaxAttVal;	/* number of values for each att */

char		*SpecialStatus;	/* special att treatment */

C45_String *ClassName;
C45_String *AttName;
C45_String **AttValName;
C45_String FileName = "DF";

Boolean		AllKnown = true;	/* true if there have been no splits
								on atts with missing values above
								the current position in the tree */


short VERBOSITY = 0;	/* verbosity level (0 = none) */
short TRIALS = 10;		/* number of trees to be grown */

Boolean	GAINRATIO = true;	/* true=gain ratio, false=gain */
Boolean	SUBSET = false;		/* true if subset tests allowed */
Boolean	BATCH = true;		/* true if windowing turned off */
Boolean	UNSEENS = true;	/* true if to evaluate on test data */
Boolean	PROBTHRESH = false;	/* true if to use soft thresholds */

ItemNo	MINOBJS = 2;	/* minimum items each side of a cut */
ItemNo	WINDOW = 0;		/* initial window size */
ItemNo	INCREMENT = 0;	/* max window increment each iteration */

float	CF = 0.25;		/* confidence limit for tree pruning */

Set	**Subset;	/* Subset[a][s] = subset s for att a */

short *Subsets;	/* Subsets[a] = no. subsets for att a */

ItemCount	*Slice1,	/* Slice1[c]    = saved values of Freq[x][c] in subset.c */
				*Slice2;	/* Slice2[c]    = saved values of Freq[y][c] */

char Delimiter;

ItemCount	*Weight,	/* Weight[i]  = current fraction of item i */
				**Freq,		/* Freq[x][c] = no. items of class c with outcome x */
				*ValFreq,	/* ValFreq[x]   = no. items with outcome x */
				*ClassFreq;	/* ClassFreq[c] = no. items of class c */

//Variables
ItemNo	*TargetClassFreq;
Tree		*Raw;
Tree		*Pruned;

//Variables 
float	*Gain,		/* Gain[a] = info gain by split on att a */
		*Info,		/* Info[a] = potential info of split on att a */
		*Bar,		/* Bar[a]  = best threshold for contin att a */
		*UnknownRate;	/* UnknownRate[a] = current unknown rate for att a */

Boolean	*Tested,	/* Tested[a] set if att a has already been tested */
			MultiVal;	/* true when all atts have many values */

float	*SplitGain,	/* SplitGain[i] = gain with att value of item i as threshold */
		*SplitInfo;	/* SplitInfo[i] = potential info ditto */

float *ClassSum = Nil;		/* accumulated central estimates */ 

void PrintHeader (char *Title)
{	
	//Specimem
    //char *ctime(), TitleLine[80];
	char TitleLine[80];
    long  clock;
    short Underline;
	time_t timevar;

	sprintf (TitleLine, "C4.5 [release %s] %s", RELEASE, Title);
	time(&timevar);
    printf ("\n%s\t%l", TitleLine, ctime(&timevar));

    Underline = strlen (TitleLine);
    while ( Underline-- ) putchar('-');
    putchar('\n');
}

/*************************************************************************/
/*									 */
/*			Error messages					 */
/*									 */
/*************************************************************************/

void Error(short n, C45_String s1, C45_String s2)
{
    static char Messages=0;

    printf("\nERROR:  ");
    switch(n)
    {
	case 0: printf("cannot open file %s%s\n", s1, s2);
		exit(1);

	case 1:	printf("colon expected after attribute name %s\n", s1);
		break;

	case 2:	printf("unexpected eof while reading attribute %s\n", s1);
		break;

	case 3: printf("attribute %s has only one value\n", s1);
		break;

	case 4: printf("case %d's value of '%s' for attribute %s is illegal\n",
		    MaxItem+1, s2, s1);
		break;

	case 5: printf("case %d's class of '%s' is illegal\n", MaxItem+1, s2);
    }

    if ( ++Messages > 10 )
    {
	printf("Error limit exceeded\n");
	exit(1);
    }
}

/*************************************************************************/
/*									 */
/*	Allocate space then copy string into it				 */
/*									 */
/*************************************************************************/

C45_String CopyString (C45_String x)
{
    char *s;

    s = (char *) calloc(strlen(x)+1, sizeof(char));
    strcpy(s, x);
    return s;
}



Boolean ReadName (FILE *f, C45_String s)
{
    register char *Sp=s;
    register int c;

    /*  Skip to first non-space character  */

    while ( ( c = getc(f) ) == '|' || Space(c) )
    {
	if ( c == '|' ) SkipComment;
    }

    /*  Return false if no names to read  */

    if ( c == EOF )
    {
	Delimiter = EOF;
	return false;
    }

    /*  Read in characters up to the next delimiter  */

    while ( c != ':' && c != ',' && c != '\n' && c != '|' && c != EOF )
    {
	if ( c == '.' )
	{
	    if ( ( c = getc(f) ) == '|' || Space(c) ) break;
	    *Sp++ = '.';
	}

	if ( c == '\\' )
	{
	    c = getc(f);
	}

	*Sp++ = c;

	if ( c == ' ' )
	{
	    while ( ( c = getc(f) ) == ' ' )
		;
	}
	else
	{
	    c = getc(f);
	}
    }

    if ( c == '|' ) SkipComment;
    Delimiter = c;

    /*  Strip trailing spaces  */

    while ( Space(*(Sp-1)) ) Sp--;

    *Sp++ = '\0';
    return true;
}



/*************************************************************************/
/*									 */
/*	Locate value Val in List[First] to List[Last]			 */
/*									 */
/*************************************************************************/


int Which(C45_String Val, C45_String List[], short First, short Last)
{
    short n=First;

    while ( n <= Last && strcmp(Val, (const char*) List[n]) ) n++;

    return ( n <= Last ? n : First-1 );
}


/*************************************************************************/
/*								 	 */
/*	Return the total weight of items from Fp to Lp		 	 */
/*								 	 */
/*************************************************************************/

ItemCount CountItems (ItemNo Fp, ItemNo Lp)
{
    register ItemCount Sum=0.0, *Wt, *LWt;

    if ( AllKnown ) return Lp - Fp + 1;

    for ( Wt = Weight + Fp, LWt = Weight + Lp ; Wt <= LWt ; )
    {
	Sum += *Wt++;
    }

    return Sum;
}

#endif //__HEADER_CPP__