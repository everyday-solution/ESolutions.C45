/*************************************************************************/
/*									 */
/*	Get case descriptions from data file				 */
/*	--------------------------------------				 */
/*									 */
/*************************************************************************/

#ifndef __GETDATA_CPP__
 #define __GETDATA_CPP__

#include "header.h"
#include "getdata.h"

/*************************************************************************/
/*									 */
/*  Read raw case descriptions from file with given extension.		 */
/*									 */
/*  On completion, cases are stored in array Item in the form		 */
/*  of Descriptions (i.e. arrays of attribute values), and		 */
/*  MaxItem is set to the number of data items.				 */
/*									 */
/*************************************************************************/

void GetData (C45_String Extension)
{
	FILE *Df = Nil;
	char Fn[MAX_CHAR] = {0};
	ItemNo i=0, j = 0, ItemSpace = 0;

	/*  Open data file  */
	//strcpy(Fn, FileName);
	StringCbCopy(Fn, MAX_CHAR, FileName);
	//strcat(Fn, Extension);
	StringCbCatA(Fn, MAX_CHAR, Extension);

	if (!(Df = fopen(Fn, "r"))) 
	{
		Error(0, Fn, "");
	}

	do
	{
		MaxItem = i;

		/*  Make sure there is room for another item  */

		if ( i >= ItemSpace )
		{
			if ( ItemSpace )
			{
				ItemSpace += Inc;
				Item = (Description *) realloc(Item, ItemSpace * sizeof(Description));
			}
			else
			{
				Item = (Description *)
				malloc((ItemSpace=Inc)*sizeof(Description));
			}
		}

		Item[i] = GetDescription(Df);

	} while ( Item[i] != Nil && ++i );

	fclose (Df);
	MaxItem = i - 1;
}



/*************************************************************************/
/*									 */
/*  Read a raw case description from file Df.				 */
/*									 */
/*  For each attribute, read the attribute value from the file.		 */
/*  If it is a discrete valued attribute, find the associated no.	 */
/*  of this attribute value (if the value is unknown this is 0).	 */
/*									 */
/*  Returns the Description of the case (i.e. the array of		 */
/*  attribute values).							 */
/*									 */
/*************************************************************************/

Description GetDescription (FILE* Df)
{
	Attribute Att = 0;
	char name[500] = {0}, 
		 *endname = Nil; 
	int Dv = 0;
	float Cv = 0.0;
	Description Dvec = Nil;

	if (ReadName (Df, name))
	{
		Dvec = (Description) calloc(MaxAtt+2, sizeof(AttValue));

		ForEach(Att, 0, MaxAtt)
		{
			if ( SpecialStatus[Att] == IGNORE )
			{
			/*  Skip this value  */

			DVal(Dvec, Att) = 0;
			}
			else
			{
				if ( MaxAttVal[Att] || SpecialStatus[Att] == DISCRETE )
				{
					/*  Discrete value  */ 
					if ( ! ( strcmp(name, "?") ) )
					{
						Dv = 0;
					}
					else
					{
						Dv = Which(name, AttValName[Att], 1, MaxAttVal[Att]);		    

						if ( ! Dv )
						{
							if ( SpecialStatus[Att] == DISCRETE )
							{
								/*  Add value to list  */
								Dv = ++MaxAttVal[Att];

								if ( Dv > (int) AttValName[Att][0] )
								{
									StringCbPrintf(szBuffer, cbDest, "\nToo many values for %s (max %d)\n",
									AttName[Att], (int) AttValName[Att][0]);
									SendOutput (szBuffer);
									exit(1);
								}
								AttValName[Att][Dv] = CopyString(name);
							}
							else
							{
								Error(4, AttName[Att], name);
							}
						}
					}
					DVal(Dvec, Att) = Dv;
				}
				else
				{
					/*  Continuous value  */
					if ( ! ( strcmp(name, "?") ) )
					{
						Cv = UNKNOWN;
					}
					else
					{
						Cv = strtod(name, &endname);
						if ( endname == name || *endname != '\0' )
						{
							Error(4, AttName[Att], name);
						}
					}
					CVal(Dvec, Att) = Cv;
				}
				ReadName(Df, name);
			}
		}

		if ((Dv = Which(name, ClassName, 0, MaxClass)) < 0)
		{
			Error(5, "", name);
			Dv = 0;
		}
		Class(Dvec) = Dv;

		return Dvec;
	}
	else
	{
		return Nil;
	}
} 
#endif // __GETDATA_CPP__