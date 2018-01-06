/*************************************************************************/
/*								   	 */
/*	Classify items interactively using a decision tree	   	 */
/*	--------------------------------------------------   		 */
/*								   	 */
/*************************************************************************/


#include "header.h"
#include "consult.h"
#include "userint.h"
#include "esolutions.h"

//definition of extern variables
short TRACE = 0; 
RangeDescRec RangeDesc = {0}; 
Tree DecisionTree = Nil;			/* tree being used */
float	*LowClassSum = 0;			/* accumulated lower estimates */	

/*************************************************************************/
/*								   	 */
/*  Classify the extended case description in RangeDesc using the	 */
/*  given subtree, by adjusting the values ClassSum and LowClassSum	 */
/*  for each class, indicating the likelihood of the case being  	 */
/*  of that class.						   	 */
/*								   	 */
/*************************************************************************/

void ClassifyCase (Tree Subtree, float Weight, FEATURES features[100])
{
    DiscrValue v = 0;
    float BranchWeight = 0;
    Attribute a = 0;
    short s = 0;
    ClassNo c = 0;

    /*  A leaf  */

	if (!Subtree->NodeType)
	{
		Verbosity(1)
		{
			StringCbPrintf(szBuffer, cbDest, "\tClass %s weight %g cases %g\n", ClassName[Subtree->Leaf], Weight, Subtree->Items);
			SendOutput (szBuffer);
		}

		if ( Subtree->Items > 0 )
		{
			/*  Adjust class sum of ALL classes, but adjust low class sum
			of leaf class only  */

			ForEach(c, 0, MaxClass)
			{
				ClassSum[c] += Weight * Subtree->ClassDist[c] / Subtree->Items;
			}

			LowClassSum[Subtree->Leaf] +=
			Weight * (1 - Subtree->Errors / Subtree->Items);
		}
		else
		{
			ClassSum[Subtree->Leaf] += Weight;
		}

		return;
   }

    a = Subtree->Tested;

	CheckValue (a, Subtree, features);

	/*  Unknown value  */

	if ( ! RangeDesc[a].Known )
	{
		ForEach(v, 1, Subtree->Forks)
		{
			ClassifyCase(Subtree->Branch[v], (Weight * Subtree->Branch[v]->Items) / Subtree->Items, features);
		}
		return;
	}

	/*  Known value  */

	switch ( Subtree->NodeType )
	{
		case BrDiscr:  /* test of discrete attribute */
		{
			ForEach(v, 1, MaxAttVal[a])
			{
				BranchWeight = RangeDesc[a].Probability[v];

				if ( BranchWeight > 0 )
				{
					Verbosity(1)
					{
						StringCbPrintf(szBuffer, cbDest, "\tWeight %g: test att %s (val %s = %g)\n", Weight, AttName[a], AttValName[a][v], BranchWeight);
						SendOutput (szBuffer);
					}

					ClassifyCase (Subtree->Branch[v], Weight * BranchWeight, features);
				}
			}
			break;
		}

		case ThreshContin:  /* test of continuous attribute */
		{
			BranchWeight = RangeDesc[a].UpperBound <= Subtree->Lower ? 1.0 :
								RangeDesc[a].LowerBound > Subtree->Upper ? 0.0 :
								RangeDesc[a].LowerBound != RangeDesc[a].UpperBound ? (Area(Subtree, RangeDesc[a].LowerBound) - Area(Subtree, RangeDesc[a].UpperBound)) / (RangeDesc[a].UpperBound - RangeDesc[a].LowerBound) :	
								Interpolate(Subtree, RangeDesc[a].LowerBound) ;

			Verbosity(1)
			{
				StringCbPrintf(szBuffer, cbDest, "\tWeight %g: test att %s (branch weight=%g)\n", Weight, AttName[a], BranchWeight);
				SendOutput (szBuffer);
			}

			if ( BranchWeight > Fuzz )
			{
				ClassifyCase(Subtree->Branch[1], Weight * BranchWeight, features);
			}

			if ( BranchWeight < 1-Fuzz )
			{
				ClassifyCase(Subtree->Branch[2], Weight * (1 - BranchWeight), features);
			}
			break;
		}

		case BrSubset:  /* subset test on discrete attribute  */
		{
			ForEach(s, 1, Subtree->Forks)
			{
				BranchWeight = 0.0;

				ForEach(v, 1, MaxAttVal[a])
				{
					if (In(v, Subtree->Subset[s]))
					{
						BranchWeight += RangeDesc[a].Probability[v];
					}
				}

				if ( BranchWeight > 0 )
				{
					Verbosity(1)
					{
		    			StringCbPrintf(szBuffer, cbDest, "\tWeight %g: test att %s (val %s = %g)\n", Weight, AttName[a], AttValName[a][v], BranchWeight);
						SendOutput (szBuffer);
					}
					ClassifyCase(Subtree->Branch[s], Weight * BranchWeight, features);
				}
			}
			break;
		}
	}
}



/*************************************************************************/
/*								   	 */
/*  Interpolate a single value between Lower, Cut and Upper		 */
/*								   	 */
/*************************************************************************/


float Interpolate (Tree t, float v)
{
    float Sum=Epsilon;

    if ( v <= t->Lower )
    {
	return 1.0;
    }

    if ( v <= t->Cut )
    {
	return 1 - 0.5 * (v - t->Lower) / (t->Cut - t->Lower + Epsilon);
    }

    if ( v < t->Upper )
    {
	return 0.5 - 0.5 * (v - t->Cut) / (t->Upper - t->Cut + Epsilon);
    }

    return 0.0;
}



/*************************************************************************/
/*								   	 */
/*  Compute the area under a soft threshold curve to the right of a	 */
/*  given value.							 */
/*								   	 */
/*************************************************************************/


float Area (Tree t, float v)
{
    float Sum=Epsilon, F;

    if ( v < t->Lower )
    {
	Sum += t->Lower - v;
	v = t->Lower;
    }

    if ( v < t->Cut )
    {
	F = (t->Cut - v ) / (t->Cut - t->Lower + Epsilon);

	Sum += 0.5 * (t->Cut - v) + 0.25 * F * (t->Cut - v);
	v = t->Cut;
    }

    if ( v < t->Upper )
    {
	F = (t->Upper - v ) / (t->Upper - t->Cut + Epsilon);

	Sum += 0.25 * (t->Upper - v) * F;
    }

    Verbosity(1) 
	 {
		 StringCbPrintf(szBuffer, cbDest, "lower=%g  cut=%g  upper=%g  area=%g\n",t->Lower, t->Cut, t->Upper, Sum);
		 SendOutput (szBuffer);
	 }

    return Sum;
}



/*************************************************************************/
/*								  	 */
/*		Process a single case				  	 */
/*								  	 */
/*************************************************************************/


void InterpretTree ()
{ 
    ClassNo c, BestClass;
    float Uncertainty=1.0;
    char Reply;
    Attribute a;

    /*  Initialise  */

    ForEach(a, 0, MaxAtt)
    {
	RangeDesc[a].Asked = false;
    }

    if ( ! ClassSum )
    {
	/*  The first time through .. allocate class sums  */

	ClassSum = (float *) malloc((MaxClass+1) * sizeof(float));
	LowClassSum = (float *) malloc((MaxClass+1) * sizeof(float));

	StringCbPrintf(szBuffer, cbDest, "\n");
	SendOutput (szBuffer);
    }
    else
    {
	StringCbPrintf(szBuffer, cbDest, "\n-------------------------------------------\n\n");
	SendOutput (szBuffer);
    }

    ForEach(c, 0, MaxClass)
    {
	LowClassSum[c] = ClassSum[c] = 0;
    }

    /*  Find the likelihood of an item's being of each class  */

    ClassifyCase(DecisionTree, 1.0, NULL);

    /*  Find the best class and show decision made  */

    BestClass = 0;
    ForEach(c, 0, MaxClass)
    {
	Verbosity(1) StringCbPrintf(szBuffer, cbDest, "class %d weight %.2f\n", c, ClassSum[c]);
	SendOutput (szBuffer);

	Uncertainty -= LowClassSum[c];
	if ( ClassSum[c] > ClassSum[BestClass] ) BestClass = c;
    }

    StringCbPrintf(szBuffer, cbDest, "\nDecision:\n");
	 SendOutput (szBuffer);
    Decision(BestClass, ClassSum[BestClass],
	     LowClassSum[BestClass],
	     Uncertainty + LowClassSum[BestClass]);

    /*  Show the other significant classes, if more than two classes  */

    if ( MaxClass > 1 )
    {
	while ( true )
	{
	    ClassSum[BestClass] = 0;
	    BestClass = 0;
	    ForEach(c, 0, MaxClass)
	    {
		if ( ClassSum[c] > ClassSum[BestClass] ) BestClass = c;
	    }

	    if ( ClassSum[BestClass] < Fuzz ) break;

	    Decision(BestClass, ClassSum[BestClass],
		     LowClassSum[BestClass],
		     Uncertainty + LowClassSum[BestClass]);
	}
    }

    /*  Prompt for what to do next  */

    while ( true )
    {
	StringCbPrintf(szBuffer, cbDest, "\nRetry, new case or quit [r,n,q]: ");
	SendOutput (szBuffer);
	Reply = getchar();
	SkipLine(Reply);
	switch ( Reply )
	{
	  case 'r':  return;
	  case 'n':  Clear(); return;
	  case 'q':  exit(0);
	  default:   
		  {
				StringCbPrintf(szBuffer, cbDest, "Please enter 'r', 'n' or 'q'");
				SendOutput (szBuffer);
		  }
	}
    }
}



/*************************************************************************/
/*								  	 */
/*  Print the chosen class with certainty factor and range	  	 */
/*								  	 */
/*************************************************************************/
void Decision (ClassNo c, float p, float lb, float ub)
{
    StringCbPrintf(szBuffer, cbDest, "\t%s", ClassName[c]);
	 SendOutput (szBuffer);

    if ( p < 1-Fuzz || lb < ub - Fuzz )
    {
	StringCbPrintf(szBuffer, cbDest, "  CF = %.2f", p);
	SendOutput (szBuffer);
	if ( lb < ub - Fuzz )
	{
	    StringCbPrintf(szBuffer, cbDest, "  [ %.2f - %.2f ]", lb, ub);
		 SendOutput (szBuffer);
	}
    }

    StringCbPrintf(szBuffer, cbDest, "\n");
	 SendOutput (szBuffer);
}
//Interpretes a single case depending on a feature aray
void InterpretSingleCase (FEATURES Features[100], OUTCLASSES outClasses[MAX_OUTCLASSES])
{
	ClassNo c, BestClass;
	float Uncertainty=1.0;
	char Reply;
	Attribute a;

	/*  Initialise  */

	ForEach(a, 0, MaxAtt)
   {
		RangeDesc[a].Asked = false;
	}
	
	if (!LowClassSum)
	{		
		LowClassSum = (float *) malloc((MaxClass+1) * sizeof(float));
	}

	if (!ClassSum )
   {
		/*  The first time through .. allocate class sum  */
		ClassSum = (float *) malloc((MaxClass+1) * sizeof(float));		

		StringCbPrintf(szBuffer, cbDest, "\n");
		SendOutput (szBuffer);
	}
   else
   {
		StringCbPrintf(szBuffer, cbDest, "\n-------------------------------------------\n\n");
		SendOutput (szBuffer);
	}

   ForEach(c, 0, MaxClass)
   {
		LowClassSum[c] = ClassSum[c] = 0;
   }

   /*  Find the likelihood of an item's being of each class  */
   ClassifyCase(DecisionTree, 1.0, Features);

   /*  Find the best class and show decision made  */
	BestClass = 0;
	ForEach(c, 0, MaxClass)
   {
		Verbosity(1) 
		{
			StringCbPrintf(szBuffer, cbDest, "class %d weight %.2f\n", c, ClassSum[c]);
			SendOutput (szBuffer);
		}

		Uncertainty -= LowClassSum[c];

		if (ClassSum[c] > ClassSum[BestClass]) 
		{
			BestClass = c;
		}
	}

	StringCbPrintf(szBuffer, cbDest, "\nDecision:\n");
	SendOutput (szBuffer);
   Decision (BestClass, ClassSum[BestClass],	LowClassSum[BestClass],	Uncertainty + LowClassSum[BestClass]);

	// fill first (best) outclass
	//strcpy (outClasses[0].pClassBuffer->szClassBuffer, ClassName[BestClass]);
	StringCbCopy(outClasses[0].pClassBuffer->szClassBuffer, strlen (ClassName[BestClass]) + 1, ClassName[BestClass]);
	outClasses[0].fBestGuess = ClassSum[BestClass];
	outClasses[0].fLowerBound = LowClassSum[BestClass];
	outClasses[0].fUpperBound = Uncertainty + LowClassSum[BestClass];

    /*  Show the other significant classes, if more than two classes  */

	if ( MaxClass > 1 )
   {
		int iClassCounter = 1;

		while ( true )
		{
			ClassSum[BestClass] = 0;
			BestClass = 0;

			ForEach(c, 0, MaxClass)
			{
				if ( ClassSum[c] > ClassSum[BestClass] ) 
				{
					BestClass = c;
				}
			}

			if (ClassSum[BestClass] < Fuzz) 
			{
				//Just count classes which weights more than the fuzzy value
				MaxClass = iClassCounter;
				break;
			}

			Decision(BestClass, ClassSum[BestClass], LowClassSum[BestClass], Uncertainty + LowClassSum[BestClass]);

			//strcpy (outClasses[iClassCounter].pClassBuffer->szClassBuffer, ClassName[BestClass]);
			StringCbCopy(outClasses[0].pClassBuffer->szClassBuffer, strlen (ClassName[BestClass]) + 1, ClassName[BestClass]);
			outClasses[iClassCounter].fBestGuess = ClassSum[BestClass];
			outClasses[iClassCounter].fLowerBound = LowClassSum[BestClass];
			outClasses[iClassCounter].fUpperBound = Uncertainty + LowClassSum[BestClass];

			iClassCounter++;
		}
	}
}