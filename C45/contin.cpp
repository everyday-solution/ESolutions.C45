/*************************************************************************/
/*                                                                	 */
/*	Evaluation of a test on a continuous valued attribute	  	 */
/*	-----------------------------------------------------	  	 */
/*								  	 */
/*************************************************************************/

#ifndef __CONTIN_CPP__
#define __CONTIN_CPP__

#include "buildex.i"
#include "header.h"
#include "info.h"
#include "contin.h"
#include "build.h"
#include "sort.h"
#include "trees.h"

/*************************************************************************/
/*								  	 */
/*  Continuous attributes are treated as if they have possible values	 */
/*	0 (unknown), 1 (less than cut), 2(greater than cut)	  	 */
/*  This routine finds the best cut for items Fp through Lp and sets	 */
/*  Info[], Gain[] and Bar[]						 */
/*								  	 */
/*************************************************************************/


void EvalContinuousAtt(Attribute Att, ItemNo Fp, ItemNo Lp)
{ 
    ItemNo i = 0, BestI = 0, Xp = 0, Tries = 0;
    ItemCount Items = 0, KnownItems = 0, LowItems = 0, MinSplit = 0;
    ClassNo c = 0;
    float AvGain = 0.0, Val = 0.0, BestVal = 0.0, BaseInfo = 0.0, ThreshCost = 0.0;  

    Verbosity(2) 
	 {
		 StringCbPrintf(szBuffer, cbDest, "\tAtt %s", AttName[Att]);
		 SendOutput (szBuffer);
	 }

    Verbosity(3) 
	 {
		 StringCbPrintf(szBuffer, cbDest, "\n");
		 SendOutput (szBuffer);
	 }

    ResetFreq(2);

    /*  Omit and count unknown values */

    Items = CountItems(Fp, Lp);
    Xp = Fp;
    ForEach(i, Fp, Lp)
    {
	if ( CVal(Item[i],Att) == UNKNOWN )
	{
	    Freq[ 0 ][ Class(Item[i]) ] += Weight[i];
	    Swap(Xp, i);
	    Xp++;
	}
    }

    ValFreq[0] = 0;
    ForEach(c, 0, MaxClass)
    {
	ValFreq[0] += Freq[0][c];
    }

    KnownItems = Items - ValFreq[0];
    UnknownRate[Att] = 1.0 - KnownItems / Items;

    /*  Special case when very few known values  */

    if ( KnownItems < 2 * MINOBJS )
    {
	Verbosity(2) 
	{
		StringCbPrintf(szBuffer, cbDest, "\tinsufficient cases with known values\n");
		SendOutput (szBuffer);
	}

	Gain[Att] = -Epsilon;
	Info[Att] = 0.0;
	return;
    }

    Quicksort(Xp, Lp, Att, Swap);

    /*  Count base values and determine base information  */

    ForEach(i, Xp, Lp)
    {
	Freq[ 2 ][ Class(Item[i]) ] += Weight[i];
	SplitGain[i] = -Epsilon;
	SplitInfo[i] = 0;
    }

    BaseInfo = TotalInfo(Freq[2], 0, MaxClass) / KnownItems;

    /*  Try possible cuts between items i and i+1, and determine the
	information and gain of the split in each case.  We have to be wary
	of splitting a small number of items off one end, as we can always
	split off a single item, but this has little predictive power.  */

    MinSplit = 0.10 * KnownItems / (MaxClass + 1);
    if ( MinSplit <= MINOBJS ) MinSplit = MINOBJS;
    else
    if ( MinSplit > 25 ) MinSplit = 25;

    LowItems = 0;
    ForEach(i, Xp, Lp - 1)
    {
	c = Class(Item[i]);
	LowItems   += Weight[i];
	Freq[1][c] += Weight[i];
	Freq[2][c] -= Weight[i];

	if ( LowItems < MinSplit ) continue;
	else
	if ( LowItems > KnownItems - MinSplit ) break;

	if ( CVal(Item[i],Att) < CVal(Item[i+1],Att) - 1E-5 )
	{
	    ValFreq[1] = LowItems;
	    ValFreq[2] = KnownItems - LowItems;
	    SplitGain[i] = ComputeGain(BaseInfo, UnknownRate[Att], 2, KnownItems);
	    SplitInfo[i] = TotalInfo(ValFreq, 0, 2) / Items;
	    AvGain += SplitGain[i];
	    Tries++;

	    Verbosity(3)
	    {	StringCbPrintf(szBuffer, cbDest, "\t\tCut at %.3f  (gain %.3f, val %.3f):",
	               ( CVal(Item[i],Att) + CVal(Item[i+1],Att) ) / 2,
	    	       SplitGain[i],
	    	       Worth(SplitInfo[i], SplitGain[i], Epsilon));
	    	       PrintDistribution(Att, 2, true);
			SendOutput (szBuffer);
	    }
	}
    }

    /*  Find the best attribute according to the given criterion  */

    ThreshCost = Log(Tries) / Items;

    BestVal = 0;
    BestI   = None;
    ForEach(i, Xp, Lp - 1)
    {
	if ( (Val = SplitGain[i] - ThreshCost) > BestVal )
	{
	    BestI   = i;
	    BestVal = Val;
	}
    }

    /*  If a test on the attribute is able to make a gain,
	set the best break point, gain and information  */ 

    if ( BestI == None )
    {
	Gain[Att] = -Epsilon;
	Info[Att] = 0.0;

	Verbosity(2) 
	{
		StringCbPrintf(szBuffer, cbDest, "\tno gain\n");
		SendOutput (szBuffer);
	}
    }
    else
    {
	Bar[Att]  = (CVal(Item[BestI],Att) + CVal(Item[BestI+1],Att)) / 2;
	Gain[Att] = BestVal;
	Info[Att] = SplitInfo[BestI];

	Verbosity(2)
	{
	    StringCbPrintf(szBuffer, cbDest, "\tcut=%.3f, inf %.3f, gain %.3f\n",
		   Bar[Att], Info[Att], Gain[Att]);
		 SendOutput (szBuffer);
	}
    }
} 



/*************************************************************************/
/*                                                                	 */
/*  Change a leaf into a test on a continuous attribute           	 */
/*                                                                	 */
/*************************************************************************/


void ContinTest(Tree Node, Attribute Att)
{
    float Thresh = 0.0;

    Sprout(Node, 2);

    Thresh = GreatestValueBelow(Att, Bar[Att]);

    Node->NodeType	= ThreshContin;
    Node->Tested	= Att;
    Node->Cut		=
    Node->Lower		=
    Node->Upper		= Thresh;
    Node->Errors        = 0;
}



/*************************************************************************/
/*                                                                	 */
/*  Return the greatest value of attribute Att below threshold t  	 */
/*                                                                	 */
/*************************************************************************/


float GreatestValueBelow(Attribute Att, float t)
{
    ItemNo i = 0.0;
    float v = 0.0, Best = 0.0;
    Boolean NotYet = true;

    ForEach(i, 0, MaxItem)
    {
	v = CVal(Item[i], Att);
	if ( v != UNKNOWN && v <= t && ( NotYet || v > Best ) )
	{
	    Best = v;
	    NotYet = false;
	}
    }

    return Best;
}

#endif //__CONTIN_CPP__