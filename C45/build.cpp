/*************************************************************************/
/*								 	 */
/*    Central tree-forming algorithm incorporating all criteria  	 */
/*    ---------------------------------------------------------	 	 */
/*								 	 */
/*************************************************************************/

#ifndef __BUILD_CPP__
 #define __BUILD_CPP__

#include "header.h"
#include "subset.h"
#include "build.h"
#include "trees.h"
#include "subset.h"
#include "discr.h"
#include "contin.h"
#include "info.h"

/*************************************************************************/
/*								 	 */
/*		Allocate space for tree tables			 	 */
/*								 	 */
/*************************************************************************/


void InitialiseTreeData()
{ 
    DiscrValue v = 0;
    Attribute a = 0;

    Tested	= (char *) calloc(MaxAtt+1, sizeof(char));

    Gain	= (float *) calloc(MaxAtt+1, sizeof(float));
    Info	= (float *) calloc(MaxAtt+1, sizeof(float));
    Bar		= (float *) calloc(MaxAtt+1, sizeof(float));

    Subset = (Set **) calloc(MaxAtt+1, sizeof(Set *));	 

    ForEach(a, 0, MaxAtt)
    {
		if ( MaxAttVal[a] )
		{
			Subset[a]  = (Set *) calloc(MaxDiscrVal+1, sizeof(Set));
			ForEach(v, 0, MaxAttVal[a])
			{
				Subset[a][v] = (Set) malloc((MaxAttVal[a]>>3) + 1);
			}
		}
	}
	Subsets = (short *) calloc(MaxAtt+1, sizeof(short));

	SplitGain = (float *) calloc(MaxItem+1, sizeof(float));
	SplitInfo = (float *) calloc(MaxItem+1, sizeof(float));

	Weight = (ItemCount *) calloc(MaxItem+1, sizeof(ItemCount));

	Freq  = (ItemCount **) calloc (MaxDiscrVal+1, sizeof(ItemCount *));

	ForEach(v, 0, MaxDiscrVal)
	{
		Freq[v]  = (ItemCount *) calloc (MaxClass+1, sizeof(ItemCount));
	}

	ValFreq = (ItemCount *) calloc (MaxDiscrVal+1, sizeof(ItemCount));
	ClassFreq = (ItemCount *) calloc (MaxClass+1, sizeof(ItemCount));

	Slice1 = (ItemCount *) calloc (MaxClass+2, sizeof(ItemCount));
	Slice2 = (ItemCount *) calloc (MaxClass+2, sizeof(ItemCount));

	UnknownRate = (float *) calloc (MaxAtt+1, sizeof(float));

	/*  Check whether all attributes have many discrete values  */

	MultiVal = true;

	if (!SUBSET)
   {
		for ( a = 0 ; MultiVal && a <= MaxAtt ; a++ )
		{
			if ( SpecialStatus[a] != IGNORE )
			{
				MultiVal = MaxAttVal[a] >= 0.3 * (MaxItem + 1);
			}
		}
	}
}



/*************************************************************************/
/*								 	 */
/*		Initialise the weight of each item		 	 */
/*								 	 */
/*************************************************************************/


void InitialiseWeights ()
{
    ItemNo i = 0;

    ForEach(i, 0, MaxItem)
    {
        Weight[i] = 1.0;
    }
}



/*************************************************************************/
/*								 	 */
/*  Build a decision tree for the cases Fp through Lp:		 	 */
/*								 	 */
/*  - if all cases are of the same class, the tree is a leaf and so	 */
/*      the leaf is returned labelled with this class		 	 */
/*								 	 */
/*  - for each attribute, calculate the potential information provided 	 */
/*	by a test on the attribute (based on the probabilities of each	 */
/*	case having a particular value for the attribute), and the gain	 */
/*	in information that would result from a test on the attribute	 */
/*	(based on the probabilities of each case with a particular	 */
/*	value for the attribute being of a particular class)		 */
/*								 	 */
/*  - on the basis of these figures, and depending on the current	 */
/*	selection criterion, find the best attribute to branch on. 	 */
/*	Note:  this version will not allow a split on an attribute	 */
/*	unless two or more subsets have at least MINOBJS items. 	 */
/*								 	 */
/*  - try branching and test whether better than forming a leaf	 	 */
/*								 	 */
/*************************************************************************/


Tree FormTree(ItemNo Fp, ItemNo Lp)
{ 
	ItemNo i = 0, Kp = 0, Ep = 0;
	ItemCount Cases = 0, NoBestClass = 0, KnownCases = 0;
	float Factor = 0.0, BestVal = 0.0, Val = 0.0, AvGain=0.0;
	Attribute Att = 0, BestAtt = 0, Possible=0;
	ClassNo c = 0, BestClass = 0;
	Tree Node = Nil;
	DiscrValue v = 0;
	Boolean PrevAllKnown = false;

	Cases = CountItems(Fp, Lp);

	/*  Generate the class frequency distribution  */

	ForEach(c, 0, MaxClass)
	{
		ClassFreq[c] = 0;
	}

	ForEach(i, Fp, Lp)
	{ 
		ClassFreq[ Class(Item[i]) ] += Weight[i];
	} 

	/*  Find the most frequent class  */

	BestClass = 0;
	ForEach(c, 0, MaxClass)
	{
		if ( ClassFreq[c] > ClassFreq[BestClass] )
		{
			BestClass = c;
		}
	}
	NoBestClass = ClassFreq[BestClass];

	Node = Leaf(ClassFreq, BestClass, Cases, Cases - NoBestClass);

	/*  If all cases are of the same class or there are not enough
		cases to divide, the tree is a leaf  */

	if ( NoBestClass == Cases  || Cases < 2 * MINOBJS )
	{ 
		return Node;
	} 

	Verbosity(1)
	{
		StringCbPrintf(szBuffer, cbDest, "\n%d items, total weight %.1f\n", Lp - Fp + 1, Cases);
		SendOutput (szBuffer);
	}

	/*  For each available attribute, find the information and gain  */

	ForEach(Att, 0, MaxAtt) 
	{ 
		Gain[Att] = -Epsilon;

		if ( SpecialStatus[Att] == IGNORE ) 
		{
			continue;
		}

		if (MaxAttVal[Att])
		{
			/*  discrete valued attribute  */
			if ( SUBSET && MaxAttVal[Att] > 2 )
			{
				EvalSubset(Att, Fp, Lp, Cases);
			}
			else
			{
				if ( ! Tested[Att] )
				{
					EvalDiscreteAtt(Att, Fp, Lp, Cases);
				}
			}
		}
		else
		{ 
			/*  continuous attribute  */
			EvalContinuousAtt(Att, Fp, Lp);
		} 

		/*  Update average gain, excluding attributes with very many values  */

		if ( Gain[Att] > -Epsilon &&
			( MultiVal || MaxAttVal[Att] < 0.3 * (MaxItem + 1) ) )
		{
			Possible++;
			AvGain += Gain[Att];
		}
	} 

	/*  Find the best attribute according to the given criterion  */

	BestVal = -Epsilon;
	BestAtt = None;
	AvGain  = ( Possible ? AvGain / Possible : 1E6 );

	Verbosity(2)
	{
		if ( AvGain < 1E6 ) 
		{
			StringCbPrintf(szBuffer, cbDest, "\taverage gain %.3f\n", AvGain);
			SendOutput (szBuffer);
		}
	}

	ForEach(Att, 0, MaxAtt) 
   { 
		if ( Gain[Att] > -Epsilon )
		{ 
			Val = Worth(Info[Att], Gain[Att], AvGain);
			if ( Val > BestVal ) 
			{
				BestAtt  = Att; 
				BestVal = Val;
			} 
		} 
	} 

	/*  Decide whether to branch or not  */ 
	if ( BestAtt != None )
	{ 
		Verbosity(1)
		{
			StringCbPrintf(szBuffer, cbDest, "\tbest attribute %s", AttName[BestAtt]);
			SendOutput (szBuffer);

			if ( ! MaxAttVal[BestAtt] )
			{
				StringCbPrintf(szBuffer, cbDest, " cut %.3f", Bar[BestAtt]);
				SendOutput (szBuffer);
			}
			StringCbPrintf(szBuffer, cbDest, " inf %.3f gain %.3f val %.3f\n", Info[BestAtt], Gain[BestAtt], BestVal);
			SendOutput (szBuffer);
		}	

	/*  Build a node of the selected test  */
		if ( MaxAttVal[BestAtt] )
		{
			/*  Discrete valued attribute  */
			if ( SUBSET && MaxAttVal[BestAtt] > 2 )
			{
				SubsetTest(Node, BestAtt);
			}
			else
			{
				DiscreteTest(Node, BestAtt);
			}
		}
		else
		{ 
			/*  Continuous attribute  */
			ContinTest(Node, BestAtt);
		} 

	/*  Remove unknown attribute values  */

	PrevAllKnown = AllKnown;

	Kp = Group(0, Fp, Lp, Node) + 1;
	if ( Kp != Fp ) AllKnown = false;
	KnownCases = Cases - CountItems(Fp, Kp-1);
	UnknownRate[BestAtt] = (Cases - KnownCases) / (Cases + 0.001);

	Verbosity(1)
	{
	    if ( UnknownRate[BestAtt] > 0 )
	    {
		StringCbPrintf(szBuffer, cbDest, "\tunknown rate for %s = %.3f\n",
		       AttName[BestAtt], UnknownRate[BestAtt]);
		SendOutput (szBuffer);
	    }
	}

	/*  Recursive divide and conquer  */

	++Tested[BestAtt];

	Ep = Kp - 1;
	Node->Errors = 0;

	ForEach(v, 1, Node->Forks)
	{
	    Ep = Group(v, Kp, Lp, Node);

	    if ( Kp <= Ep )
	    {
		Factor = CountItems(Kp, Ep) / KnownCases;

		ForEach(i, Fp, Kp-1)
		{
		    Weight[i] *= Factor;
		}

		Node->Branch[v] = FormTree(Fp, Ep);
		Node->Errors += Node->Branch[v]->Errors;

		Group(0, Fp, Ep, Node);
		ForEach(i, Fp, Kp-1)
		{
		    Weight[i] /= Factor;
		}
	    }
	    else
	    {
		Node->Branch[v] = Leaf(Node->ClassDist, BestClass, 0.0, 0.0);
	    }
	}

	--Tested[BestAtt];
	AllKnown = PrevAllKnown;

	/*  See whether we would have been no worse off with a leaf  */

	if ( Node->Errors >= Cases - NoBestClass - Epsilon )
	{ 
	    Verbosity(1)
		StringCbPrintf(szBuffer, cbDest, "Collapse tree for %d items to leaf %s\n",
	                Lp - Fp + 1, ClassName[BestClass]);

	    Node->NodeType = 0;
	} 
    }
    else
    { 
	Verbosity(1)
	{
	    StringCbPrintf(szBuffer, cbDest, "\tno sensible splits  %.1f/%.1f\n",  Cases, Cases - NoBestClass);
		 SendOutput (szBuffer);
	}
    } 

    return Node; 
} 



/*************************************************************************/
/*								 	 */
/*  Group together the items corresponding to branch V of a test 	 */
/*  and return the index of the last such			 	 */
/*								 	 */
/*  Note: if V equals zero, group the unknown values		 	 */
/*								 	 */
/*************************************************************************/


ItemNo Group(DiscrValue V, ItemNo Fp, ItemNo Lp, Tree TestNode)
{
    ItemNo i;
    Attribute Att;
    float Thresh;
    Set SS;    

    Att = TestNode->Tested;

    if ( V )
    {
	/*  Group items on the value of attribute Att, and depending
	    on the type of branch  */

	switch ( TestNode->NodeType )
	{
	    case BrDiscr:

		ForEach(i, Fp, Lp)
		{
		    if ( DVal(Item[i], Att) == V ) Swap(Fp++, i);
		}
		break;

	    case ThreshContin:

		Thresh = TestNode->Cut;
		ForEach(i, Fp, Lp)
		{
		    if ( (CVal(Item[i], Att) <= Thresh) == (V == 1) ) Swap(Fp++, i);
		}
		break;

	    case BrSubset:

		SS = TestNode->Subset[V];
		ForEach(i, Fp, Lp)
		{
		    if ( In(DVal(Item[i], Att), SS) ) Swap(Fp++, i);
		}
		break;
	}
    }
    else
    {
	/*  Group together unknown values  */

	switch ( TestNode->NodeType )
	{
	    case BrDiscr:
	    case BrSubset:

		ForEach(i, Fp, Lp)
		{
		    if ( ! DVal(Item[i], Att) ) Swap(Fp++, i);
		}
		break;

	    case ThreshContin:

		ForEach(i, Fp, Lp)
		{
		    if ( CVal(Item[i], Att) == UNKNOWN ) Swap(Fp++, i);
		}
		break;
	}
    }

    return Fp - 1;
}


/*************************************************************************/
/*                                                               	 */
/*		Exchange items at a and b			 	 */
/*									 */
/*************************************************************************/


void Swap(ItemNo a, ItemNo b)
{
    register Description Hold;
    register ItemCount HoldW;

    Hold = Item[a];
    Item[a] = Item[b];
    Item[b] = Hold;

    HoldW = Weight[a];
    Weight[a] = Weight[b];
    Weight[b] = HoldW;
}

#endif