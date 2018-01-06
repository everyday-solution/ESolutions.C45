#ifndef __ST_THRESH_H__
 #define __ST_THRESH_H__

//Constants
#define	Below(v,t)	(v <= t + 1E-6)


//Variables
static Boolean *LHSErr,	/*  Does a misclassification occur with this value of an att  */
	*RHSErr;	/*  if the below or above threshold branches are taken  */

static ItemNo	*ThreshErrs;	/*  ThreshErrs[i] is the no. of misclassifications if thresh is i  */

static float	*CVals;		/*  All values of a continuous attribute  */

//Prototypes
void SoftenThresh(Tree T);
void ScanTree(Tree T, ItemNo Fp, ItemNo Lp);

#endif //__ST_THRESH_H__