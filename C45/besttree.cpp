/*************************************************************************/
/*									 */
/*	Routines to manage tree growth, pruning and evaluation		 */
/*	------------------------------------------------------		 */
/*									 */
/*************************************************************************/

#ifndef __BESTREE_CPP
#define __BESTREE_CPP

#include "header.h"
#include "besttree.h"
#include "trees.h"
#include "build.h"
#include "prune.h"
#include "classify.h"
#include "confmat.h"

/*************************************************************************/
/*									 */
/*	Grow and prune a single tree from all data			 */
/*									 */
/*************************************************************************/

//Specimem
#define random() rand()
#define	 Random			((random()&2147483647) / 2147483648.0)

void OneTree (void)
/*  ---------  */
{
    Raw = (Tree *) calloc(1, sizeof(Tree));
    Pruned = (Tree *) calloc(1, sizeof(Tree));

    InitialiseTreeData();
    InitialiseWeights(); 

    AllKnown = true;
    Raw[0] = FormTree(0, MaxItem);
    StringCbPrintf(szBuffer, cbDest, "\n");
	 SendOutput (szBuffer);
    PrintTree(Raw[0]);

    SaveTree(Raw[0], ".unpruned");

    Pruned[0] = CopyTree(Raw[0]);
    if ( Prune(Pruned[0]) )
    {
	StringCbPrintf(szBuffer, cbDest, "\nSimplified ");
	SendOutput (szBuffer);
	PrintTree(Pruned[0]);
    }
}



/*************************************************************************/
/*									 */
/*	Grow and prune TRIALS trees and select the best of them		 */
/*									 */
/*************************************************************************/


short BestTree()
{
    short t = 0, Best=0;

    InitialiseTreeData();

    TargetClassFreq = (ItemNo *) calloc(MaxClass+1, sizeof(ItemNo));

    Raw    = (Tree *) calloc(TRIALS, sizeof(Tree));
    Pruned = (Tree *) calloc(TRIALS, sizeof(Tree));

    /*  If necessary, set initial size of window to 20% (or twice
	the sqrt, if this is larger) of the number of data items,
	and the maximum number of items that can be added to the
	window at each iteration to 20% of the initial window size  */

    if ( ! WINDOW )
    {
	WINDOW = Max(2 * sqrt(MaxItem+1.0), (MaxItem+1) / 5);
    }

    if ( ! INCREMENT )
    {
	INCREMENT = Max(WINDOW / 5, 1);
    }

    FormTarget(WINDOW);

    /*  Form set of trees by iteration and prune  */

    ForEach(t, 0, TRIALS-1 )
    {
        FormInitialWindow();

	StringCbPrintf(szBuffer, cbDest, "\n--------\nTrial %d\n--------\n\n", t);
	SendOutput (szBuffer);

	Raw[t] = Iterate(WINDOW, INCREMENT);
	StringCbPrintf(szBuffer, cbDest, "\n");
	SendOutput (szBuffer);
	PrintTree(Raw[t]);

	SaveTree(Raw[t], ".unpruned");

	Pruned[t] = CopyTree(Raw[t]);
	if ( Prune(Pruned[t]) )
	{
	    StringCbPrintf(szBuffer, cbDest, "\nSimplified ");
		 SendOutput (szBuffer);
	    PrintTree(Pruned[t]);
	}

	if ( Pruned[t]->Errors < Pruned[Best]->Errors )
	{
	    Best = t;
	}
    }
    StringCbPrintf(szBuffer, cbDest, "\n--------\n");
	 SendOutput (szBuffer);

    return Best;
}



/*************************************************************************/
/*									 */
/*  The windowing approach seems to work best when the class		 */
/*  distribution of the initial window is as close to uniform as	 */
/*  possible.  FormTarget generates this initial target distribution,	 */
/*  setting up a TargetClassFreq value for each class.			 */
/*									 */
/*************************************************************************/


void FormTarget(ItemNo Size)    
{
    ItemNo i = 0, *ClassFreq = Nil;
    ClassNo c = 0, Smallest = 0, ClassesLeft = 0;

    ClassFreq = (ItemNo *) calloc(MaxClass+1, sizeof(ItemNo));

    /*  Generate the class frequency distribution  */

    ForEach(i, 0, MaxItem)
    {
	ClassFreq[ Class(Item[i]) ]++;
    }

    /*  Calculate the no. of classes of which there are items  */

    ForEach(c, 0, MaxClass)
    {
	if ( ClassFreq[c] )
	{
	    ClassesLeft++;
	}
	else
	{
	    TargetClassFreq[c] = 0;
	}
    }

    while ( ClassesLeft )
    {
	/*  Find least common class of which there are some items  */

	Smallest = -1;
	ForEach(c, 0, MaxClass)
	{
	    if ( ClassFreq[c] &&
		 ( Smallest < 0 || ClassFreq[c] < ClassFreq[Smallest] ) )
	    {
		Smallest = c;
	    }
	}

	/*  Allocate the no. of items of this class to use in the window  */

	TargetClassFreq[Smallest] = Min(ClassFreq[Smallest], Round(Size/ClassesLeft));

	ClassFreq[Smallest] = 0;

	Size -= TargetClassFreq[Smallest];
	ClassesLeft--;
    }

    cfree(ClassFreq);
}



/*************************************************************************/
/*									 */
/*  Form initial window, attempting to obtain the target class profile	 */
/*  in TargetClassFreq.  This is done by placing the targeted number     */
/*  of items of each class at the beginning of the set of data items.	 */
/*									 */
/*************************************************************************/


void FormInitialWindow()
{
    ItemNo i = 0, Start = 0, More = 0;
    ClassNo c = 0;

    ForEach(c, 0, MaxClass)
    {
	More = TargetClassFreq[c];

	for ( i = Start ; More ; i++ )
	{
	    if ( Class(Item[i]) == c )
	    {
		Swap(Start, i);
		Start++;
		More--;
	    }
	}
    }
}



/*************************************************************************/
/*									 */
/*		Shuffle the data items randomly				 */
/*									 */
/*************************************************************************/


void     Shuffle()
{
    ItemNo This = 0, Alt = 0, Left = 0;
    Description Hold = Nil;

    This = 0;
    for( Left = MaxItem+1 ; Left ; )
    {		
        Alt = This + (Left--) * Random;		
		Hold = Item[This];
        Item[This++] = Item[Alt];
        Item[Alt] = Hold;
    }
}



/*************************************************************************/
/*									 */
/*  Grow a tree iteratively with initial window size Window and		 */
/*  initial window increment IncExceptions.				 */
/*									 */
/*  Construct a classifier tree using the data items in the		 */
/*  window, then test for the successful classification of other	 */
/*  data items by this tree.  If there are misclassified items,		 */
/*  put them immediately after the items in the window, increase	 */
/*  the size of the window and build another classifier tree, and	 */
/*  so on until we have a tree which successfully classifies all	 */
/*  of the test items or no improvement is apparent.			 */
/*									 */
/*  On completion, return the tree which produced the least errors.	 */
/*									 */
/*************************************************************************/


Tree Iterate(ItemNo Window, ItemNo IncExceptions)
{
    Tree Classifier = Nil, 
			BestClassifier = Nil;
    ItemNo	i = 0, 
				Errors = 0, 
				TotalErrors = 0, 
				BestTotalErrors = MaxItem+1,
				Exceptions = 0, 
				Additions = 0;
    ClassNo Assigned = 0;
    short Cycle=0;

    StringCbPrintf(szBuffer, cbDest, "Cycle   Tree    -----Cases----");
	 SendOutput (szBuffer);
    StringCbPrintf(szBuffer, cbDest, "    -----------------Errors-----------------\n");
	 SendOutput (szBuffer);
    StringCbPrintf(szBuffer, cbDest, "        size    window   other");
	 SendOutput (szBuffer);
    StringCbPrintf(szBuffer, cbDest, "    window  rate   other  rate   total  rate\n");
	 SendOutput (szBuffer);
    StringCbPrintf(szBuffer, cbDest, "-----   ----    ------  ------");
	 SendOutput (szBuffer);
    StringCbPrintf(szBuffer, cbDest, "    ------  ----  ------  ----  ------  ----\n");
	 SendOutput (szBuffer);

    do
    {
	/*  Build a classifier tree with the first Window items  */

	InitialiseWeights();
	AllKnown = true;
	Classifier = FormTree(0, Window-1);

	/*  Error analysis  */

	Errors = Round(Classifier->Errors);

	/*  Move all items that are incorrectly classified by the
	    classifier tree to immediately after the items in the
	    current window.  */

	Exceptions = Window;
	ForEach(i, Window, MaxItem)
	{
	    Assigned = Category(Item[i], Classifier);
	    if ( Assigned != Class(Item[i]) )
	    {
		Swap(Exceptions, i);
		Exceptions++;
	    }
	}
        Exceptions -= Window;
	TotalErrors = Errors + Exceptions;

	/*  Print error analysis  */

	StringCbPrintf(szBuffer, cbDest, "%3d  %7d  %8d  %6d  %8d%5.1f%%  %6d%5.1f%%  %6d%5.1f%%\n",
	       ++Cycle, TreeSize(Classifier), Window, MaxItem-Window+1,
	       Errors, 100*(float)Errors/Window,
	       Exceptions, 100*Exceptions/(MaxItem-Window+1.001),
	       TotalErrors, 100*TotalErrors/(MaxItem+1.0));
	SendOutput (szBuffer);

	/*  Keep track of the most successful classifier tree so far  */

	if ( ! BestClassifier || TotalErrors < BestTotalErrors )
	{
	    if ( BestClassifier ) ReleaseTree(BestClassifier);
	    BestClassifier = Classifier;
	    BestTotalErrors = TotalErrors;
        }
	else
	{
	    ReleaseTree(Classifier);
	}

	/*  Increment window size  */

	Additions = Min(Exceptions, IncExceptions);
	Window = Min(Window + Max(Additions, Exceptions / 2), MaxItem + 1);
    }
    while ( Exceptions );

    return BestClassifier;
}



/*************************************************************************/
/*									 */
/*	Print report of errors for each of the trials			 */
/*									 */
/*************************************************************************/


void Evaluate (Boolean CMInfo, short Saved)
{
    ClassNo RealClass = 0, 
				PrunedClass = 0;
    short t = 0;
    ItemNo	*ConfusionMat = Nil, 
				i = 0, 
				RawErrors = 0, 
				PrunedErrors = 0;

    if (CMInfo)
    {
		ConfusionMat = (ItemNo *) calloc((MaxClass+1)*(MaxClass+1), sizeof(ItemNo));
    }

    StringCbPrintf(szBuffer, cbDest, "\n");
	 SendOutput (szBuffer);

    if (TRIALS > 1)
    {
		StringCbPrintf(szBuffer, cbDest, "Trial\t Before Pruning           After Pruning\n");
		SendOutput (szBuffer);
		StringCbPrintf(szBuffer, cbDest, "-----\t----------------   ---------------------------\n");
		SendOutput (szBuffer);
    }
    else
    {
		StringCbPrintf(szBuffer, cbDest, "\t Before Pruning           After Pruning\n");
		SendOutput (szBuffer);
		StringCbPrintf(szBuffer, cbDest, "\t----------------   ---------------------------\n");
		SendOutput (szBuffer);
    }
    StringCbPrintf(szBuffer, cbDest, "\tSize      Errors   Size      Errors   Estimate\n\n");
	 SendOutput (szBuffer);

    ForEach(t, 0, TRIALS-1)
    {
		RawErrors = PrunedErrors = 0;

		ForEach(i, 0, MaxItem)
		{
			RealClass = Class(Item[i]);

			if (Category(Item[i], Raw[t]) != RealClass) 
			{
				RawErrors++;
			}

			PrunedClass = Category(Item[i], Pruned[t]);

			if (PrunedClass != RealClass ) 
			{
				PrunedErrors++;
			}

			if (CMInfo && t == Saved)
			{
				ConfusionMat[RealClass*(MaxClass+1)+PrunedClass]++;
			}
		}
    
		if ( TRIALS > 1 )
		{
			StringCbPrintf(szBuffer, cbDest, "%4d", t);
			SendOutput (szBuffer);
		}

		StringCbPrintf(szBuffer, cbDest, "\t%4d  %3d(%4.1f%%)   %4d  %3d(%4.1f%%)    (%4.1f%%)%s\n",
			    TreeSize(Raw[t]), RawErrors, 100.0*RawErrors / (MaxItem+1.0),
				 TreeSize(Pruned[t]), PrunedErrors, 100.0*PrunedErrors / (MaxItem+1.0),
				 100 * Pruned[t]->Errors / Pruned[t]->Items,
				 ( t == Saved ? "   <<" : "" ));
		SendOutput (szBuffer);
	}

	if (CMInfo)
	{
		PrintConfusionMatrix(ConfusionMat);
		free(ConfusionMat);
	}
}

#endif //__BESTREE_CPP