/*************************************************************************/
/*									 */
/*	Calculate information, information gain, and print dists	 */
/*	--------------------------------------------------------	 */
/*									 */
/*************************************************************************/

#ifndef __INFO_CPP__
#define __INFO_CPP__

#include "buildex.i"
#include "header.h"
#include "info.h"


/*************************************************************************/
/*									 */
/*  Determine the worth of a particular split according to the		 */
/*  operative criterion							 */
/*									 */
/*	    Parameters:							 */
/*		SplitInfo:	potential info of the split		 */
/*		SplitGain:	gain in info of the split		 */
/*		MinGain:	gain above which the Gain Ratio		 */
/*				may be used				 */
/*									 */
/*  If the Gain criterion is being used, the information gain of	 */
/*  the split is returned, but if the Gain Ratio criterion is		 */
/*  being used, the ratio of the information gain of the split to	 */
/*  its potential information is returned.				 */
/*									 */
/*************************************************************************/


float Worth(float ThisInfo, float ThisGain, float MinGain)
{
    if ( GAINRATIO )
    {
	if ( ThisGain >= MinGain - Epsilon && ThisInfo > Epsilon )
	{
	    return ThisGain / ThisInfo;
	}
	else
	{
	    return -Epsilon;
	}
    }
    else
    {
	return ( ThisInfo > 0 && ThisGain > -Epsilon ? ThisGain : -Epsilon );
    }
}



/*************************************************************************/
/*									 */
/*  Zero the frequency tables Freq[][] and ValFreq[] up to MaxVal	 */
/*									 */
/*************************************************************************/


void ResetFreq(DiscrValue MaxVal)
{
	DiscrValue v = 0;
	ClassNo c = 0;

	ForEach(v, 0, MaxVal)
	{ 
		ForEach(c, 0, MaxClass)
		{
			Freq[v][c] = 0;
		}
		ValFreq[v] = 0;
	} 
}


/*************************************************************************/
/*									 */
/*  Given tables Freq[][] and ValFreq[], compute the information gain.	 */
/*									 */
/*	    Parameters:							 */
/*		BaseInfo:	average information for all items with	 */
/*				known values of the test attribute	 */
/*		UnknownRate:	fraction of items with unknown ditto	 */
/*		MaxVal:		number of forks				 */
/*		TotalItems:	number of items with known values of	 */
/*				test att				 */
/*									 */
/*  where Freq[x][y] contains the no. of cases with value x for a	 */
/*  particular attribute that are members of class y,			 */
/*  and ValFreq[x] contains the no. of cases with value x for a		 */
/*  particular attribute						 */
/*									 */
/*************************************************************************/


float ComputeGain(float BaseInfo, float UnknFrac, DiscrValue MaxVal, ItemCount TotalItems)
{
    DiscrValue v = 0;
    float ThisInfo=0.0, ThisGain = 0.0;
    short ReasonableSubsets=0;

    /*  Check whether all values are unknown or the same  */

    if ( ! TotalItems ) return -Epsilon;

    /*  There must be at least two subsets with MINOBJS items  */

    ForEach(v, 1, MaxVal)
    {
	if ( ValFreq[v] >= MINOBJS ) ReasonableSubsets++;
    }
    if ( ReasonableSubsets < 2 ) return -Epsilon;

    /*  Compute total info after split, by summing the
	info of each of the subsets formed by the test  */

    ForEach(v, 1, MaxVal)
    {
	ThisInfo += TotalInfo(Freq[v], 0, MaxClass);
    }

    /*  Set the gain in information for all items, adjusted for unknowns  */

    ThisGain = (1 - UnknFrac) * (BaseInfo - ThisInfo / TotalItems);

    Verbosity(5)
	 {
		StringCbPrintf(szBuffer, cbDest, "ComputeThisGain: items %.1f info %.3f base %.3f unkn %.3f result %.3f\n", TotalItems + ValFreq[0], ThisInfo, BaseInfo, UnknFrac, ThisGain);
		SendOutput (szBuffer);
	 }

    return ThisGain;
}



/*************************************************************************/
/*									 */
/*  Compute the total information in V[ MinVal..MaxVal ]		 */
/*									 */
/*************************************************************************/


float TotalInfo(ItemCount V[], DiscrValue MinVal, DiscrValue MaxVal)
{
	DiscrValue v = 0;
	float Sum=0.0;
	ItemCount N = 0.0, TotalItems=0;

	ForEach(v, MinVal, MaxVal)
	{
		N = V[v];

		Sum += N * Log(N);
		TotalItems += N;
	}

	return TotalItems * Log(TotalItems) - Sum;
}


/*************************************************************************/
/*									 */
/*	Print distribution table for given attribute			 */
/*									 */
/*************************************************************************/
void PrintDistribution(Attribute Att, DiscrValue MaxVal, Boolean ShowNames)
{
	DiscrValue v = 0;
	ClassNo c = 0;
	C45_String Val = Nil;

	StringCbPrintf(szBuffer, cbDest, "\n\t\t\t ");	
	SendOutput (szBuffer);
	ForEach(c, 0, MaxClass)
	{
		StringCbPrintf(szBuffer, cbDest, "%7.6s", ClassName[c]);
		SendOutput (szBuffer);
	}
	StringCbPrintf(szBuffer, cbDest, "\n");
	SendOutput (szBuffer);

	ForEach(v, 0, MaxVal)
	{
		if ( ShowNames )
		{
			Val = ( !v ? "unknown" :
					MaxAttVal[Att] ? AttValName[Att][v] :
					v == 1 ? "below" : "above" );
			StringCbPrintf(szBuffer, cbDest, "\t\t[%-7.7s:", Val);
			SendOutput (szBuffer);
		}
		else
		{
			StringCbPrintf(szBuffer, cbDest, "\t\t[%-7d:", v);
			SendOutput (szBuffer);
		}

		ForEach(c, 0, MaxClass)
		{
			StringCbPrintf(szBuffer, cbDest, " %6.1f", Freq[v][c]);
			SendOutput (szBuffer);
		}

		StringCbPrintf(szBuffer, cbDest, "]\n");
		SendOutput (szBuffer);
	}
}

#endif //__INFO_CPP__