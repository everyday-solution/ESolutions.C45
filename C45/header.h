/*************************************************************************/
/*									 */
/*	Print header for all C4.5 programs				 */
/*	----------------------------------				 */
/*									 */
/*************************************************************************/

#ifndef __HEADER_H__
 #define __HEADER_H__

#define  RELEASE "8"

#define  Space(s)	(s == ' ' || s == '\n' || s == '\t')
#define  SkipComment	while ( ( c = getc(f) ) != '\n' )
#define	cfree( x )		(free(x),x=NULL)

#define WINDOWS_LEAN_AND_MEAN
#include <windows.h>
#include <stdio.h>
#include <time.h>
#include <string.h>
#include <stdlib.h>
#include <strsafe.h>

#include <esolutions.h>
#include "defns.h"
#include "types.h"
#include "extern.h"

#include "besttree.h"
#include "build.h"
#include "classify.h"
#include "confmat.h"
#include "contin.h"
#include "discr.h"
#include "getdata.h"
#include "getnames.h"
#include "getopt.h"
#include "info.h"
#include "prune.h"
#include "sort.h"
#include "st-thresh.h"
#include "stats.h"
#include "subset.h"
#include "trees.h"
#include "consult.h"
#include "userint.h"

extern Set
	**Subset;	/* Subset[a][s] = subset s for att a */

extern short
	*Subsets;	/* Subsets[a] = no. subsets for att a */

extern ItemCount
	*Slice1,	/* Slice1[c]    = saved values of Freq[x][c] in subset.c */
	*Slice2;	/* Slice2[c]    = saved values of Freq[y][c] */

extern char Delimiter;

extern ItemCount
	*Weight,	/* Weight[i]  = current fraction of item i */
	**Freq,		/* Freq[x][c] = no. items of class c with outcome x */
	*ValFreq,	/* ValFreq[x]   = no. items with outcome x */
	*ClassFreq;	/* ClassFreq[c] = no. items of class c */

//Variables
extern ItemNo	*TargetClassFreq;
extern Tree		*Raw;
extern Tree		*Pruned;

//Variables 
extern float
	*Gain,		/* Gain[a] = info gain by split on att a */
	*Info,		/* Info[a] = potential info of split on att a */
	*Bar,		/* Bar[a]  = best threshold for contin att a */
	*UnknownRate;	/* UnknownRate[a] = current unknown rate for att a */

extern Boolean
	*Tested,	/* Tested[a] set if att a has already been tested */
	MultiVal;	/* true when all atts have many values */


	/*  External variables initialised here  */

extern float
	*SplitGain,	/* SplitGain[i] = gain with att value of item i as threshold */
	*SplitInfo,	/* SplitInfo[i] = potential info ditto */
	*ClassSum;

extern	size_t cbDest;
extern	char szBuffer[MAX_OUTPUT_SIZE];
extern	SOCKET connectSocket;
extern	STATISTICS statistics;

extern void PrintHeader (char *Title);
extern void Error (short n, C45_String s1, C45_String s2);
extern C45_String CopyString (C45_String x);
extern Boolean ReadName (FILE *f, C45_String s);
extern int Which (C45_String Val, C45_String List[], short First, short Last);
extern ItemCount CountItems (ItemNo Fp, ItemNo Lp);
extern void SendOutput (char* szBuffer);
#endif //_HEADER_H__

