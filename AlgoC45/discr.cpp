/*************************************************************************/
/*									 */
/*	Evaluation of a test on a discrete valued attribute		 */
/*      ---------------------------------------------------		 */
/*									 */
/*************************************************************************/
#ifndef __DISC_CPP__
 #define __DISC_CPP__

#include "buildex.i"
#include "header.h"
#include "info.h"
#include "trees.h"
#include "discr.h"


/*************************************************************************/
/*									 */
/*  Set Info[] and Gain[] for discrete partition of items Fp to Lp	 */
/*									 */
/*************************************************************************/


void EvalDiscreteAtt(Attribute Att, ItemNo Fp, ItemNo Lp, ItemCount Items)
{ 
    ItemCount KnownItems;    

    ComputeFrequencies(Att, Fp, Lp);

    KnownItems = Items - ValFreq[0];

    /*  Special case when no known values of the attribute  */

    if ( Items <= ValFreq[0] )
    {
	Verbosity(2) printf("\tAtt %s: no known values\n", AttName[Att]);

	Gain[Att] = -Epsilon;
	Info[Att] = 0.0;
	return;
    }

    Gain[Att] = ComputeGain(DiscrKnownBaseInfo(KnownItems, MaxAttVal[Att]),
			    UnknownRate[Att], MaxAttVal[Att], KnownItems);
    Info[Att] = TotalInfo(ValFreq, 0, MaxAttVal[Att]) / Items;

    Verbosity(2)
    {
    	printf("\tAtt %s", AttName[Att]);
    	Verbosity(3) PrintDistribution(Att, MaxAttVal[Att], true);
    	printf("\tinf %.3f, gain %.3f\n", Info[Att], Gain[Att]);
    }

} 



/*************************************************************************/
/*									 */
/*  Compute frequency tables Freq[][] and ValFreq[] for attribute	 */
/*  Att from items Fp to Lp, and set the UnknownRate for Att		 */
/*									 */
/*************************************************************************/


void ComputeFrequencies(Attribute Att, ItemNo Fp, ItemNo Lp)
{
    Description Case; 
    ClassNo c;
    DiscrValue v;    
    ItemNo p;

    ResetFreq(MaxAttVal[Att]);

    /*  Determine the frequency of each class amongst cases
	with each possible value for the given attribute  */

    ForEach(p, Fp, Lp)
    { 
	Case = Item[p];
	Freq[ DVal(Case,Att) ][ Class(Case) ] += Weight[p];
    } 

    /*  Determine the frequency of each possible value for the
	given attribute  */

    ForEach(v, 0, MaxAttVal[Att]) 
    { 
	ForEach(c, 0, MaxClass)
	{
	    ValFreq[v] += Freq[v][c];
	}
    }

    /*  Set the rate of unknown values of the attribute  */
 
    UnknownRate[Att] = ValFreq[0] / CountItems(Fp, Lp);
}



/*************************************************************************/
/*									 */
/*  Return the base info for items with known values of a discrete	 */
/*  attribute, using the frequency table Freq[][]			 */
/*	 								 */
/*************************************************************************/


float DiscrKnownBaseInfo(DiscrValue KnownItems, ItemCount MaxVal)
{
    ClassNo c;
    ItemCount ClassCount;
    double Sum=0;
    DiscrValue v;

    ForEach(c, 0, MaxClass)
    {
	ClassCount = 0;
	ForEach(v, 1, MaxVal)
	{
	    ClassCount += Freq[v][c];
	}
	Sum += ClassCount * Log(ClassCount);
    }

    return (KnownItems * Log(KnownItems) - Sum) / KnownItems;
}



/*************************************************************************/
/*									 */
/*  Construct and return a node for a test on a discrete attribute	 */
/*									 */
/*************************************************************************/


void DiscreteTest(Tree Node, Attribute Att)
{    
    Sprout(Node, MaxAttVal[Att]);

    Node->NodeType	= BrDiscr;
    Node->Tested	= Att;
    Node->Errors	= 0;
}

#endif //__DISC_CPP__