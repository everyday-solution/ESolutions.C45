/*************************************************************************/
/*                                                              	 */
/*  Determine the class of a case description from a decision tree	 */
/*  --------------------------------------------------------------	 */
/*                                                              	 */
/*************************************************************************/
#ifndef __CLASSIFY_CPP__
 #define __CLASSIFY_CPP__

#include "header.h"
#include "classify.h"

/*************************************************************************/
/*                                                              	 */
/*  Categorize a case description using the given decision tree		 */
/*                                                              	 */
/*************************************************************************/
//Variables

ClassNo Category (Description CaseDesc, Tree DecisionTree)
{ 
    ClassNo c = 0, BestClass = 0;

    if ( ! ClassSum )
    {
	ClassSum = (float *) malloc((MaxClass+1) * sizeof(float));
    }

    ForEach(c, 0, MaxClass)
    {
	ClassSum[c] = 0;
    }

    Classify(CaseDesc, DecisionTree, 1.0);

    BestClass = 0;
    ForEach(c, 0, MaxClass)
    {
	Verbosity(5) 
	{
		StringCbPrintf(szBuffer, cbDest, "class %s weight %.2f\n", ClassName[c], ClassSum[c]);
		SendOutput (szBuffer);
	}

	if ( ClassSum[c] > ClassSum[BestClass] ) BestClass = c;
    }

    return BestClass;
}



/*************************************************************************/
/*                                                              	 */
/*  Classify a case description using the given subtree by adjusting	 */
/*  the value ClassSum for each class					 */
/*                                                              	 */
/*************************************************************************/


void Classify(Description CaseDesc, Tree T, float Weight)
{
    DiscrValue v = 0, dv = 0;
    float Cv = 0.0;
    Attribute a = 0.0;
    ClassNo c = 0.0;

    switch ( T->NodeType )
    {
        case 0:  /* leaf */

	    if ( T->Items > 0 )
	    {
		/*  Update from ALL classes  */

		ForEach(c, 0, MaxClass)
		{
		    if ( T->ClassDist[c] )
		    {
			ClassSum[c] += Weight * T->ClassDist[c] / T->Items;
		    }
		}
	    }
	    else
	    {
		ClassSum[T->Leaf] += Weight;
	    }

	    return;

	case BrDiscr:  /* test of discrete attribute */

	    a = T->Tested;
	    v = DVal(CaseDesc, a);

	    if ( v && v <= T->Forks )	/*  Make sure not new discrete value  */
	    {
		Classify(CaseDesc, T->Branch[v], Weight);
	    }
	    else
	    {
		ForEach(v, 1, T->Forks)
		{
		    Classify(CaseDesc, T->Branch[v], 
			     (Weight * T->Branch[v]->Items) / T->Items);
		}
	    }

	    return;

	case ThreshContin:  /* test of continuous attribute */

	    a = T->Tested;
	    Cv = CVal(CaseDesc, a);

	    if ( Cv == UNKNOWN )
	    {
		ForEach(v, 1, 2)
		{
		    Classify(CaseDesc, T->Branch[v], 
			     (Weight * T->Branch[v]->Items) / T->Items);
		}
	    }
	    else
	    {
		v = ( Cv <= T->Cut ? 1 : 2 );
		Classify(CaseDesc, T->Branch[v], Weight);
	    }

	    return;

	case BrSubset:  /* subset test on discrete attribute  */

	    a = T->Tested;
	    dv = DVal(CaseDesc, a);

	    if ( dv )
	    {
		ForEach(v, 1, T->Forks)
		{
		    if ( In(dv, T->Subset[v]) )
		    {
			Classify(CaseDesc, T->Branch[v], Weight);

			return;
		    }
		}
	    }

	    /*  Value unknown or not found in any of the subsets  */

	    ForEach(v, 1, T->Forks)
	    {
		Classify(CaseDesc, T->Branch[v], 
		         (Weight * T->Branch[v]->Items) / T->Items);
	    }

	    return;
    } 
}

#endif //__CLASSIFY_CPP__