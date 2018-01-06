/*************************************************************************/
/*								 	 */
/*	User interface for consulting trees and rulesets	 	 */
/*	------------------------------------------------	 	 */
/*								 	 */
/*************************************************************************/


#include "header.h"
#include "userint.h"
#include "consult.h"
#include "trees.h"
#include "esolutions.h"

/*************************************************************************/
/*									 */
/*	Ask for the value of attribute Att if necessary			 */
/*									 */
/*************************************************************************/
void CheckValue (Attribute Att, Tree T, FEATURES features[MAX_FEATURES])
{
	if (RangeDesc[Att].Asked)
	{
		return;
	}

	StringCbPrintf (szBuffer, cbDest, "%s", AttName[Att]);
	SendOutput (szBuffer);
	
	if (RangeDesc[Att].Known )
   {
		StringCbPrintf(szBuffer, cbDest, " [ ");
		SendOutput (szBuffer);
		PrintRange(Att);
		StringCbPrintf(szBuffer, cbDest, " ]");
		SendOutput (szBuffer);
	}
	StringCbPrintf (szBuffer, cbDest, ": ");
	SendOutput (szBuffer);

	ReadRange (Att, T, features);
}

 
/*************************************************************************/
/*									 */
/*	Print the range of values for attribute Att			 */
/*									 */
/*************************************************************************/
void PrintRange (Attribute Att)
{
    DiscrValue dv = 0;
    Boolean First=true;
    float p = 0.0;

    if ( MaxAttVal[Att] )  /*  discrete attribute  */
    {
	ForEach(dv, 1, MaxAttVal[Att] )
	{
	    if ( (p = RangeDesc[Att].Probability[dv]) > Fuzz )
	    {
		if ( ! First )
		{
		    StringCbPrintf(szBuffer, cbDest, ", ");
			 SendOutput (szBuffer);
		}
		First = false;

		StringCbPrintf(szBuffer, cbDest, "%s", AttValName[Att][dv]);
		SendOutput (szBuffer);

		if ( p < 1-Fuzz )
		{
		    StringCbPrintf(szBuffer, cbDest, ": %.2f", p);
			 SendOutput (szBuffer);
		}
	    }
	}
    }
    else  /*  continuous attribute  */
    {
	StringCbPrintf(szBuffer, cbDest, "%g", RangeDesc[Att].LowerBound);
	SendOutput (szBuffer);
	if ( RangeDesc[Att].UpperBound > RangeDesc[Att].LowerBound + Fuzz )
	{
	    StringCbPrintf(szBuffer, cbDest, " - %g", RangeDesc[Att].UpperBound);
		 SendOutput (szBuffer);
	}
    }
}

/*************************************************************************/
/*									 */
/*	Read a range of values for attribute Att or <cr>		 */
/*									 */
/*************************************************************************/
void ReadRange (Attribute Att, Tree T, FEATURES features[MAX_FEATURES])
{
	if (!features)
	{		 
		char c;

		SkipSpace;	

		if ( c == '\n' )
		{
			return;
		}

		if ( c == '?' )
		{
			if ( (c = getchar()) == 't' )
			{
				if (T) 
				{
					PrintTree(T);
				}
				SkipLine(c);
				RangeDesc[Att].Asked = false;
				CheckValue(Att, T, features);
			}
			else
			{
				RangeDesc[Att].Known = false;
				SkipLine(c);
			}
			return;
		}

		ungetc(c, stdin);
		RangeDesc[Att].Asked = true;
		RangeDesc[Att].Known = true;

		if (MaxAttVal[Att])
		{
			ReadDiscr(Att, T);
		}
		else
		{
			ReadContin(Att, T);
		}
	}
	else
	{	//features set
		RangeDesc[Att].Asked = true;
		if (MaxAttVal[Att])
		{
			ReadDiscr (Att, T, features);
		}
		else
		{
			ReadContin (Att, T, features);
		}
	}
}



/*************************************************************************/
/*									 */
/*	Read a discrete attribute value or range			 */
/*									 */
/*************************************************************************/
void ReadDiscr (Attribute Att,Tree T)
{
	char Name[500] = {0};
	DiscrValue dv = 0, PNo = 0;
	float P = 0.0, PSum = 0.0;

	ForEach(dv, 1, MaxAttVal[Att])
	{
		RangeDesc[Att].Probability[dv] = 0.0;
	}

	do
	{
		ReadName(stdin, Name);

		dv = Which(Name, AttValName[Att], 1, MaxAttVal[Att]);

		if ( ! dv )
		{
			StringCbPrintf(szBuffer, cbDest, "\tPermissible values are %s", AttValName[Att][1]);
			SendOutput (szBuffer);

			ForEach(dv, 2, MaxAttVal[Att])
			{
				StringCbPrintf(szBuffer, cbDest, ", %s", AttValName[Att][dv]);
				SendOutput (szBuffer);
			}
			StringCbPrintf(szBuffer, cbDest, "\n");
			SendOutput (szBuffer);

			SkipLine(Delimiter);
			Retry(Att, T);
			return;
		}

		if (Delimiter == ':' )
		{
			ReadName(stdin, Name);
			sscanf(Name, "%f", &P);	/* get probability */
		}
		else
		{
			P = 1.0;		/*  only one attribute value  */
		}

		RangeDesc[Att].Probability[dv] = P;
	}
	while ( Delimiter == ',' );

	/*  Check that sum of probabilities is not > 1  */

	PNo = MaxAttVal[Att];
	PSum = 1.0;
	ForEach(dv, 1, MaxAttVal[Att])
	{
		if ( RangeDesc[Att].Probability[dv] > Fuzz )
		{
			PSum -= RangeDesc[Att].Probability[dv];
			PNo--;
		}
	}

	if ( PSum < 0 || ! PNo && PSum > Fuzz )
	{
		StringCbPrintf(szBuffer, cbDest, "Probability values must sum to 1\n");
		SendOutput (szBuffer);
		SkipLine(Delimiter);
		Retry(Att, T);
		return;
	}

	 /*  Distribute the remaining probability equally among
	the unspecified attribute values  */

	PSum /= PNo;
	ForEach(dv, 1, MaxAttVal[Att])
	{
		if ( RangeDesc[Att].Probability[dv] < Fuzz )
		{
			RangeDesc[Att].Probability[dv] = PSum;
		}
	}
}


/*************************************************************************/
/*									 */
/*	Read a continuous attribute value or range			 */
/*									 */
/*************************************************************************/
void ReadContin (Attribute Att, Tree T)
{
    char c = 0;

    scanf("%f", &RangeDesc[Att].LowerBound);
    SkipSpace;

    if ( c == '-'  )
    {
	scanf("%f", &RangeDesc[Att].UpperBound);
	SkipSpace;
    }
    else
    {
	RangeDesc[Att].UpperBound = RangeDesc[Att].LowerBound;
    }

    if ( c != '\n' )
    {
	StringCbPrintf(szBuffer, cbDest, "Must be a continuous value or range\n");
	SendOutput (szBuffer);
	SkipLine(c);
	Retry(Att, T);
    }
}



/*************************************************************************/
/*									 */
/*	Try again to obtain a value for attribute Att			 */
/*									 */
/*************************************************************************/
void Retry (Attribute Att, Tree T)
{
    RangeDesc[Att].Asked = false;
    RangeDesc[Att].Known = false;
    CheckValue(Att, T, NULL);
}


/*************************************************************************/
/*									 */
/*	Skip to the end of the line of input				 */
/*									 */
/*************************************************************************/
void SkipLine(char c)
{
    while ( c != '\n' ) c = getchar();
}


/*************************************************************************/
/*									 */
/*		Clear the range description				 */
/*									 */
/*************************************************************************/
void Clear()
{
    Attribute Att = 0;

    ForEach(Att, 0, MaxAtt)
    {
	RangeDesc[Att].Known = false;
    }
}
