/*************************************************************************/
/*									 */
/*	Prune a decision tree and predict its error rate		 */
/*	------------------------------------------------		 */
/*									 */
/*************************************************************************/

#ifndef __PRUNE_CPP__
#define __PRUNE_CPP__

#include "header.h"
#include "prune.h"
#include "build.h"
#include "stats.h"
#include "trees.h"


//Variables
Set	*PossibleValues = Nil;
Boolean	Changed = false;


/*************************************************************************/
/*									 */
/*  Prune tree T, returning true if tree has been modified		 */
/*									 */
/*************************************************************************/

Boolean Prune(Tree T)
{
	ItemNo i = 0;
	Attribute a = 0;

	AllKnown = true;

	Verbosity(1) StringCbPrintf(szBuffer, cbDest, "\n");
	SendOutput (szBuffer);

	Changed = false;

	EstimateErrors(T, 0, MaxItem, 0, true);

	if ( SUBSET )
	{
		if ( ! PossibleValues )
		{
			PossibleValues = (Set *) calloc(MaxAtt+1, sizeof(Set));
		}

		ForEach(a, 0, MaxAtt)
		{
			if ( MaxAttVal[a] )
			{
				PossibleValues[a] = (Set) malloc((MaxAttVal[a]>>3) + 1);
				ClearBits((MaxAttVal[a]>>3) + 1, PossibleValues[a]);
				ForEach(i, 1, MaxAttVal[a])
				{
					SetBit(i, PossibleValues[a]);
				}
			}
		}

		CheckPossibleValues(T);
	}

	return Changed;
}




/*************************************************************************/
/*									 */
/*	Estimate the errors in a given subtree				 */
/*									 */
/*************************************************************************/
float EstimateErrors(Tree T, ItemNo Fp, ItemNo Lp, short Sh, Boolean UpdateTree)
{ 
	ItemNo i = 0, 
		Kp = 0, 
		Ep = 0;
	ItemCount Cases = 0, 
		KnownCases = 0, 
		*LocalClassDist = Nil, 
		TreeErrors = 0, 
		LeafErrors = 0,
		ExtraLeafErrors = 0, 
		BranchErrors = 0, 
		Factor = 0, 
		MaxFactor = 0;
	DiscrValue v = 0, MaxBr = 0;
	ClassNo c = 0, BestClass = 0;
	Boolean PrevAllKnown = false;

    /*  Generate the class frequency distribution  */

    Cases = CountItems(Fp, Lp);
    LocalClassDist = (ItemCount *) calloc(MaxClass+1, sizeof(ItemCount));

    ForEach(i, Fp, Lp)
    { 
	LocalClassDist[ Class(Item[i]) ] += Weight[i];
    } 

    /*  Find the most frequent class and update the tree  */

    BestClass = T->Leaf;
    ForEach(c, 0, MaxClass)
    {
	if ( LocalClassDist[c] > LocalClassDist[BestClass] )
	{
	    BestClass = c;
	}
    }
    LeafErrors = Cases - LocalClassDist[BestClass];
    ExtraLeafErrors = AddErrs(Cases, LeafErrors);

    if ( UpdateTree )
    {
	T->Items = Cases;
	T->Leaf  = BestClass;
	memcpy(T->ClassDist, LocalClassDist, (MaxClass + 1) * sizeof(ItemCount));
    }

    if ( ! T->NodeType )	/*  leaf  */
    {
	TreeErrors = LeafErrors + ExtraLeafErrors;

	if ( UpdateTree )
	{
	    T->Errors = TreeErrors;

	    LocalVerbosity(1)
	    {
		Intab(Sh);
	    	StringCbPrintf(szBuffer, cbDest, "%s (%.2f:%.2f/%.2f)\n", ClassName[T->Leaf],
	    		T->Items, LeafErrors, T->Errors);
			SendOutput (szBuffer);
	    }
	}

	free(LocalClassDist);

	return TreeErrors;
    }

    /*  Estimate errors for each branch  */

    Kp = Group(0, Fp, Lp, T) + 1;
    KnownCases = CountItems(Kp, Lp);

    PrevAllKnown = AllKnown;
    if ( Kp != Fp ) AllKnown = false;

    TreeErrors = MaxFactor = 0;

    ForEach(v, 1, T->Forks)
    {
	Ep = Group(v, Kp, Lp, T);

	if ( Kp <= Ep )
	{
	    Factor = CountItems(Kp, Ep) / KnownCases;

	    if ( Factor >= MaxFactor )
	    {
		MaxBr = v;
		MaxFactor = Factor;
	    }

	    ForEach(i, Fp, Kp-1)
	    {
		Weight[i] *= Factor;
	    }

	    TreeErrors += EstimateErrors(T->Branch[v], Fp, Ep, Sh+1, UpdateTree);

	    Group(0, Fp, Ep, T);
	    ForEach(i, Fp, Kp-1)
	    {
		Weight[i] /= Factor;
	    }
	}
    }
 
    AllKnown = PrevAllKnown;

    if ( ! UpdateTree )
    {
	free(LocalClassDist);

	return TreeErrors;
    }

    /*  See how the largest branch would fare  */

    BranchErrors = EstimateErrors(T->Branch[MaxBr], Fp, Lp, -1000, false);

    LocalVerbosity(1)
    {
        Intab(Sh);
        StringCbPrintf(szBuffer, cbDest, "%s:  [%d%%  N=%.2f  tree=%.2f  leaf=%.2f+%.2f  br[%d]=%.2f]\n",
		AttName[T->Tested],
		(int) ((TreeErrors * 100) / (T->Items + 0.001)),
		T->Items, TreeErrors, LeafErrors, ExtraLeafErrors,
		MaxBr, BranchErrors);
		  SendOutput (szBuffer);
    }

    /*  See whether tree should be replaced with leaf or largest branch  */

    if ( LeafErrors + ExtraLeafErrors <= BranchErrors + 0.1 &&
	 LeafErrors + ExtraLeafErrors <= TreeErrors + 0.1 )
    {
	LocalVerbosity(1)
	{
	    Intab(Sh);
	    StringCbPrintf(szBuffer, cbDest, "Replaced with leaf %s\n", ClassName[T->Leaf]);
		 SendOutput (szBuffer);
	}

	T->NodeType = 0;
	T->Errors = LeafErrors + ExtraLeafErrors;
	Changed = true;
    }
    else
    if ( BranchErrors <= TreeErrors + 0.1 )
    {
	LocalVerbosity(1)
	{
	    Intab(Sh);
	    StringCbPrintf(szBuffer, cbDest, "Replaced with branch %d\n", MaxBr);
		 SendOutput (szBuffer);
	}

	AllKnown = PrevAllKnown;
	EstimateErrors(T->Branch[MaxBr], Fp, Lp, Sh, true);
	memcpy((char *) T, (char *) T->Branch[MaxBr], sizeof(TreeRec));
	Changed = true;
    }
    else
    {
	T->Errors = TreeErrors;
    }

    AllKnown = PrevAllKnown;
    free(LocalClassDist);

    return T->Errors;
}



/*************************************************************************/
/*									 */
/*	Remove unnecessary subset tests on missing values		 */
/*									 */
/*************************************************************************/
void CheckPossibleValues(Tree T)
{
    Set HoldValues = Nil;
    int v = 0, 
		 Bytes = 0, 
		 b = 0;
    Attribute A = 0;
    char Any = 0;

    if ( T->NodeType == BrSubset )
    {
	A = T->Tested;

	Bytes = (MaxAttVal[A]>>3) + 1;
	HoldValues = (Set) malloc(Bytes);

	/*  See if last (default) branch can be simplified or omitted  */

	ForEach(b, 0, Bytes-1)
	{
	    T->Subset[T->Forks][b] &= PossibleValues[A][b];
	    Any |= T->Subset[T->Forks][b];
	}

	if ( ! Any )
	{
	    T->Forks--;
	}

	/*  Process each subtree, leaving only values in branch subset  */

	CopyBits(Bytes, PossibleValues[A], HoldValues);

	ForEach(v, 1, T->Forks)
	{
	    CopyBits(Bytes, T->Subset[v], PossibleValues[A]);

	    CheckPossibleValues(T->Branch[v]);
	}

	CopyBits(Bytes, HoldValues, PossibleValues[A]);

	free(HoldValues);
    }
    else
    if ( T->NodeType )
    {
	ForEach(v, 1, T->Forks)
	{
	    CheckPossibleValues(T->Branch[v]);
	}
    }
}

#endif