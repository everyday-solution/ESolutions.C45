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

/*************************************************************************/
/*									 */
/*	Ask for the value of attribute Att if necessary			 */
/*									 */
/*************************************************************************/
void CheckValue (Attribute Att, Tree T)
{
    if ( RangeDesc[Att].Asked ) return;

    printf("%s", AttName[Att]);
    if ( RangeDesc[Att].Known )
    {
	printf(" [ ");
	PrintRange(Att);
	printf(" ]");
    }
    printf(": ");

    ReadRange(Att, T);
}

 
/*************************************************************************/
/*									 */
/*	Print the range of values for attribute Att			 */
/*									 */
/*************************************************************************/
void PrintRange (Attribute Att)
{
    DiscrValue dv;
    Boolean First=true;
    float p;

    if ( MaxAttVal[Att] )  /*  discrete attribute  */
    {
	ForEach(dv, 1, MaxAttVal[Att] )
	{
	    if ( (p = RangeDesc[Att].Probability[dv]) > Fuzz )
	    {
		if ( ! First )
		{
		    printf(", ");
		}
		First = false;

		printf("%s", AttValName[Att][dv]);
		if ( p < 1-Fuzz )
		{
		    printf(": %.2f", p);
		}
	    }
	}
    }
    else  /*  continuous attribute  */
    {
	printf("%g", RangeDesc[Att].LowerBound);
	if ( RangeDesc[Att].UpperBound > RangeDesc[Att].LowerBound + Fuzz )
	{
	    printf(" - %g", RangeDesc[Att].UpperBound);
	}
    }
}

/*************************************************************************/
/*									 */
/*	Read a range of values for attribute Att or <cr>		 */
/*									 */
/*************************************************************************/
void ReadRange (Attribute Att, Tree T)
{
    char c;

    RangeDesc[Att].Asked=true;

    SkipSpace;

    if ( c == '\n' )
    {
	return;
    }
    if ( c == '?' )
    {
	if ( (c = getchar()) == 't' )
	{
	    if ( T ) PrintTree(T);
	    SkipLine(c);
	    RangeDesc[Att].Asked = false;
	    CheckValue(Att, T);
	}
	else
	{
	    RangeDesc[Att].Known = false;
	    SkipLine(c);
	}
	return;
    }

    ungetc(c, stdin);
    RangeDesc[Att].Known = true;

    if ( MaxAttVal[Att] )
    {
	ReadDiscr(Att, T);
    }
    else
    {
	ReadContin(Att, T);
    }
}



/*************************************************************************/
/*									 */
/*	Read a discrete attribute value or range			 */
/*									 */
/*************************************************************************/
void ReadDiscr (Attribute Att,Tree T)
{
    char Name[500];
    DiscrValue dv, PNo;
    float P, PSum;

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
			printf("\tPermissible values are %s", AttValName[Att][1]);
			ForEach(dv, 2, MaxAttVal[Att])
			{
			printf(", %s", AttValName[Att][dv]);
			}
			printf("\n");

			SkipLine(Delimiter);
			Retry(Att, T);
			return;
		}

		if ( Delimiter == ':' )
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
	printf("Probability values must sum to 1\n");
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
    char c;

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
	printf("Must be a continuous value or range\n");
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
    CheckValue(Att, T);
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
    Attribute Att;

    ForEach(Att, 0, MaxAtt)
    {
	RangeDesc[Att].Known = false;
    }
}
