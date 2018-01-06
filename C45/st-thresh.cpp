/*************************************************************************/
/*									 */
/*	Soften thresholds for continuous attributes			 */
/*	-------------------------------------------			 */
/*									 */
/*************************************************************************/

#ifndef __ST_THRESH_CPP__
#define __ST_THRESH_CPP__

#include "header.h"
#include "st-thresh.h"
#include "build.h"
#include "classify.h"
#include "sort.h"

/*************************************************************************/
/*									 */
/*  Soften all thresholds for continuous attributes in tree T		 */
/*									 */
/*************************************************************************/


void SoftenThresh(Tree T)
{
    CVals = (float *) calloc(MaxItem+1, sizeof(float));
    LHSErr = (Boolean *) calloc(MaxItem+1, sizeof(Boolean));
    RHSErr = (Boolean *) calloc(MaxItem+1, sizeof(Boolean));
    ThreshErrs = (ItemNo *) calloc(MaxItem+1, sizeof(ItemNo));

    InitialiseWeights();

    ScanTree(T, 0, MaxItem);

    cfree(ThreshErrs);
    cfree(RHSErr);
    cfree(LHSErr);
    cfree(CVals);
}



/*************************************************************************/
/*								  	 */
/*  Calculate upper and lower bounds for each test on a continuous	 */
/*  attribute in tree T, using data items from Fp to Lp			 */
/*								  	 */
/*************************************************************************/


void ScanTree(Tree T, ItemNo Fp, ItemNo Lp)
{
    short v = 0;
    float Val = 0.0, 
		 Se = 0.0, 
		 Limit = 0.0, 
		 Lower = 0.0, 
		 Upper = 0.0;
    ItemNo i = 0, 
		 Kp = 0, 
		 Ep = 0, 
		 LastI = 0, 
		 Errors = 0, 
		 BaseErrors = 0;
    ClassNo CaseClass = 0, 
		 Class1 = 0, 
		 Class2 = 0;
    Boolean LeftThresh = false;
    Description CaseDesc = Nil;
    Attribute Att = 0;

    /*  Stop when get to a leaf  */

    if ( ! T->NodeType ) return;

    /*  Group the unknowns together  */

    Kp = Group(0, Fp, Lp, T);

    /*  Soften a threshold for a continuous attribute  */

    Att = T->Tested;

    if ( T->NodeType == ThreshContin )
    {
	StringCbPrintf(szBuffer, cbDest, "\nTest %s <> %g\n", AttName[Att], T->Cut);
	SendOutput (szBuffer);

	Quicksort(Kp+1, Lp, Att, Swap);

	ForEach(i, Kp+1, Lp)
	{
	    /*  See how this item would be classified if its
		value were on each side of the threshold  */

	    CaseDesc = Item[i];
	    CaseClass = Class(CaseDesc);
	    Val = CVal(CaseDesc, Att);
		
	    Class1 = Category(CaseDesc, T->Branch[1]);
	    Class2 = Category(CaseDesc, T->Branch[2]);

	    CVals[i] = Val;
	    LHSErr[i] = (Class1 != CaseClass ? 1 : 0);
	    RHSErr[i] = (Class2 != CaseClass ? 1 : 0);
	}

	/*  Set Errors to total errors if take above thresh branch,
	    and BaseErrors to errors if threshold has original value  */

	Errors = BaseErrors = 0;
	ForEach(i, Kp+1, Lp)
	{
	    Errors += RHSErr[i];

	    if ( Below(CVals[i], T->Cut) )
	    {
		BaseErrors += LHSErr[i];
	    }
	    else
	    {
		BaseErrors += RHSErr[i];
	    }
	}

	/*  Calculate standard deviation of the number of errors  */

	Se = sqrt( (BaseErrors+0.5) * (Lp-Kp-BaseErrors+0.5) / (Lp-Kp+1) );
	Limit = BaseErrors + Se;

	Verbosity(1)
	{
	    StringCbPrintf(szBuffer, cbDest, "\t\t\tBase errors %d, items %d, se=%.1f\n",
		   BaseErrors, Lp-Kp, Se);
		 SendOutput (szBuffer);
	    StringCbPrintf(szBuffer, cbDest, "\n\tVal <=   Errors\t\t+Errors\n");
		 SendOutput (szBuffer);
	    StringCbPrintf(szBuffer, cbDest, "\t         %6d\n", Errors);
		 SendOutput (szBuffer);
	}

	/*  Set ThreshErrs[i] to the no. of errors if the threshold were i  */

	ForEach(i, Kp+1, Lp)
	{
	    ThreshErrs[i] = Errors = Errors + LHSErr[i] - RHSErr[i];

	    if ( i == Lp || CVals[i] != CVals[i+1] )
	    {
		Verbosity(1)
		    StringCbPrintf(szBuffer, cbDest, "\t%6g   %6d\t\t%7d\n",
			CVals[i], Errors, Errors - BaseErrors);
		SendOutput (szBuffer);
	    }
	}

	/*  Choose Lower and Upper so that if threshold were set to
	    either, the number of items misclassified would be one
	    standard deviation above BaseErrors  */

	LastI = Kp+1;
	Lower = Min(T->Cut, CVals[LastI]);
	Upper = Max(T->Cut, CVals[Lp]);
	while ( CVals[LastI+1] == CVals[LastI] ) LastI++;

	while ( LastI < Lp )
	{
	    i = LastI + 1;
	    while ( i < Lp && CVals[i+1] == CVals[i] ) i++;

	    if ( ! LeftThresh &&
		 ThreshErrs[LastI] > Limit &&
		 ThreshErrs[i] <= Limit &&
		 Below(CVals[i], T->Cut) )
	    {
		Lower = CVals[i] -
			(CVals[i] - CVals[LastI]) * (Limit - ThreshErrs[i]) /
			(ThreshErrs[LastI] - ThreshErrs[i]);
		LeftThresh = true;
	    }
	    else
	    if ( ThreshErrs[LastI] <= Limit &&
		 ThreshErrs[i] > Limit &&
		 ! Below(CVals[i], T->Cut) )
	    {
		Upper = CVals[LastI] +
			(CVals[i] - CVals[LastI]) * (Limit - ThreshErrs[LastI]) /
			(ThreshErrs[i] - ThreshErrs[LastI]);
		if ( Upper < T->Cut ) Upper = T->Cut;
	    }

	    LastI = i;
	}

	T->Lower = Lower;
	T->Upper = Upper;

	Verbosity(1) StringCbPrintf(szBuffer, cbDest, "\n");
	SendOutput (szBuffer);

	StringCbPrintf(szBuffer, cbDest, "\tLower = %g, Upper = %g\n", T->Lower, T->Upper);
	SendOutput (szBuffer);
    }

    /*  Recursively scan each branch  */

    ForEach(v, 1, T->Forks)
    {
	Ep = Group(v, Kp+1, Lp, T);

	if ( Kp < Ep )
	{
	    ScanTree(T->Branch[v], Kp+1, Ep);
	    Kp = Ep;
	}
    }
}

#endif //__ST_THRESH_CPP__