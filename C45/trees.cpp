/*************************************************************************/
/*									 */
/*	Routines for displaying, building, saving and restoring trees	 */
/*	-------------------------------------------------------------	 */
/*									 */
/*************************************************************************/

#ifndef __TREES_CPP__
 #define __TREES_CPP__

#include "header.h"
#include "trees.h"
#include "build.h"

/*************************************************************************/
/*									 */
/*	Display entire decision tree T					 */
/*									 */
/*************************************************************************/

//Variables
short	Subtree = 0;			/* highest subtree to be printed */
Tree	Subdef[100] = {0};	/* pointers to subtrees */
FILE	*TRf = Nil;				/* file pointer for tree i/o */
char	Fn[MAX_CHAR] = {0};			/* file name */


void PrintTree(Tree T)
{
    short s = 0;

    StringCbPrintf(szBuffer, cbDest, szBuffer, cbDest, "Decision Tree:\n");
	 SendOutput (szBuffer);
    Show(T, 0);
    StringCbPrintf(szBuffer, cbDest, "\n");
	 SendOutput (szBuffer);

    ForEach(s, 1, Subtree)
    {
	StringCbPrintf(szBuffer, cbDest, "\n\nSubtree [S%d]\n", s);
	SendOutput (szBuffer);
	Show(Subdef[s], 0);
	StringCbPrintf(szBuffer, cbDest, "\n");
	SendOutput (szBuffer);
    }
    StringCbPrintf(szBuffer, cbDest, "\n");
	 SendOutput (szBuffer);
}



/*************************************************************************/
/*									 */
/*	Display the tree T with offset Sh				 */
/*									 */
/*************************************************************************/


void Show(Tree T, short Sh)
{
    DiscrValue v = 0, 
		 MaxV = 0;

    if ( T->NodeType )
    {
	/*  See whether separate subtree needed  */

	if ( T != Nil && Sh && Sh * TabSize + MaxLine(T) > Width )
	{
	    if ( Subtree < 99 )
	    {
		Subdef[++Subtree] = T;
		StringCbPrintf(szBuffer, cbDest, "[S%d]", Subtree);
		SendOutput (szBuffer);
	    }
	    else
	    {
		StringCbPrintf(szBuffer, cbDest, "[S??]");
		SendOutput (szBuffer);
	    }
	}
	else
	{
	    MaxV = T->Forks;

	    /*  Print simple cases first */

	    ForEach(v, 1, MaxV)
	    {
		if ( ! T->Branch[v]->NodeType )
		{
		    ShowBranch(Sh, T, v);
		}
	    }

	    /*  Print subtrees  */

	    ForEach(v, 1, MaxV)
	    {
		if ( T->Branch[v]->NodeType )
		{
		    ShowBranch(Sh, T, v);
		}
	    }
	}
    }
    else
    {
	StringCbPrintf(szBuffer, cbDest, " %s (%.1f", ClassName[T->Leaf], T->Items);
	SendOutput (szBuffer);
	if ( T->Errors > 0 ) 
	{
		StringCbPrintf(szBuffer, cbDest, "/%.1f", T->Errors);
		SendOutput (szBuffer);
	}
	StringCbPrintf(szBuffer, cbDest, ")");
	SendOutput (szBuffer);

    }
}



/*************************************************************************/
/*									 */
/*	Print a node T with offset Sh, branch value v, and continue	 */
/*									 */
/*************************************************************************/


void ShowBranch (short Sh, Tree T, DiscrValue v)
{
    DiscrValue Pv = 0, Last =  0;
    Attribute Att = 0;
    Boolean FirstValue = false;
    short TextWidth = 0, Skip = 0, Values = 0, i =  0;
    
    Att = T->Tested;

    switch ( T->NodeType )
    {
	case BrDiscr:

	    Indent(Sh, Tab);

	    StringCbPrintf(szBuffer, cbDest, "%s = %s:", AttName[Att], AttValName[Att][v]);
		 SendOutput (szBuffer);
	    break;

	case ThreshContin:

	    Indent(Sh, Tab);

	    StringCbPrintf(szBuffer, cbDest, "%s %s %g ", AttName[Att], ( v == 1 ? "<=" : ">" ), T->Cut);
		 SendOutput (szBuffer);

	    if ( T->Lower != T->Upper )
	    {
		StringCbPrintf(szBuffer, cbDest, "[%g,%g]", T->Lower, T->Upper);
		SendOutput (szBuffer);
	    }

	    StringCbPrintf(szBuffer, cbDest, ":");
		 SendOutput (szBuffer);
	    break;

	case BrSubset:

	    /*  Count values at this branch  */

	    ForEach(Pv, 1, MaxAttVal[Att])
	    {
		if ( In(Pv, T->Subset[v]) )
		{
		    Last = Pv;
		    Values++;
		}
	    }
	    if ( ! Values ) return;

	    Indent(Sh, Tab);

	    if ( Values == 1 )
	    {
		StringCbPrintf(szBuffer, cbDest, "%s = %s:", AttName[Att], AttValName[Att][Last]);
		SendOutput (szBuffer);
		break;
	    }

	    StringCbPrintf(szBuffer, cbDest, "%s in {", AttName[Att]);
		 SendOutput (szBuffer);
	    FirstValue = true;
	    Skip = TextWidth = strlen(AttName[Att]) + 5;

	    ForEach(Pv, 1, MaxAttVal[Att])
	    {
		if ( In(Pv, T->Subset[v]) )
		{
		    if ( ! FirstValue &&
			 TextWidth + strlen(AttValName[Att][Pv]) + 11 > Width )
		    {
		  	Indent(Sh, Tab);
			ForEach(i, 1, Skip) putchar(' ');

			TextWidth = Skip;
			FirstValue = true;
		    }

		    StringCbPrintf(szBuffer, cbDest, "%s%c", AttValName[Att][Pv], Pv == Last ? '}' : ',');
			 SendOutput (szBuffer);
		    TextWidth += strlen(AttValName[Att][Pv]) + 1;
		    FirstValue = false;
		}
	    }
	    putchar(':');
    }

    Show(T->Branch[v], Sh+1);
}



/*************************************************************************/
/*									 */
/*	Find the maximum single line size for non-leaf subtree St.	 */
/*	The line format is						 */
/*			<attribute> <> X.xx:[ <class (<Items>)], or	 */
/*			<attribute> = <DVal>:[ <class> (<Items>)]	 */
/*									 */
/*************************************************************************/


short MaxLine(Tree St)
{
    Attribute a = 0;
    DiscrValue v = 0, MaxV = 0, Next = 0;
    short Ll = 0, MaxLl = 0;

    a = St->Tested;

    MaxV = St->Forks;
    ForEach(v, 1, MaxV)
    {
	Ll = ( St->NodeType == 2 ? 4 : strlen(AttValName[a][v]) ) + 1;

	/*  Find the appropriate branch  */

        Next = v;

	if ( ! St->Branch[Next]->NodeType )
	{
	    Ll += strlen(ClassName[St->Branch[Next]->Leaf]) + 6;
	}
	MaxLl = Max(MaxLl, Ll);
    }

    return strlen(AttName[a]) + 4 + MaxLl;
}



/*************************************************************************/
/*								   	 */
/*	Indent Sh columns					  	 */
/*								  	 */
/*************************************************************************/

void Indent(short Sh, char* Mark)
{
    StringCbPrintf(szBuffer, cbDest, "\n");
	 SendOutput (szBuffer);
    while ( Sh-- ) 
	 {
		 StringCbPrintf(szBuffer, cbDest, "%s", Mark);
		 SendOutput (szBuffer);
	 }
}



/*************************************************************************/
/*									 */
/*	Save entire decision tree T in file with extension Extension	 */
/*									 */
/*************************************************************************/

void SaveTree(Tree T, C45_String Extension)
{
	static char *LastExt="";

	if ( strcmp(LastExt, Extension) )
	{
		LastExt = Extension;
	}

	if (TRf)
	{
		fclose(TRf);
	}

	//strcpy(Fn, FileName);
	StringCbCopy (Fn, MAX_CHAR, FileName);
	//strcat(Fn, Extension);
	StringCbCat (Fn, MAX_CHAR, Extension);

	if (!( TRf = fopen(Fn, "w")))
	{
	   Error(0, Fn, " for writing");
   }

   putc('\n', TRf);
   OutTree (T);

   SaveDiscreteNames();
}



/*************************************************************************/
/*									 */
/*	Save tree T as characters					 */
/*									 */
/*************************************************************************/


void OutTree(Tree T)
{
	DiscrValue v = 0;
	int Bytes = 0;

	StreamOut((char *) &T->NodeType, sizeof(short));
	StreamOut((char *) &T->Leaf, sizeof(ClassNo));
	StreamOut((char *) &T->Items, sizeof(ItemCount));
	StreamOut((char *) &T->Errors, sizeof(ItemCount));
	StreamOut((char *) T->ClassDist, (MaxClass + 1) * sizeof(ItemCount));

	if ( T->NodeType )
	{
		StreamOut((char *) &T->Tested, sizeof(Attribute));
		StreamOut((char *) &T->Forks, sizeof(short));

		switch ( T->NodeType )
		{
			case BrDiscr:
			{
				break;
			}

			case ThreshContin:
			{
				StreamOut((char *) &T->Cut, sizeof(float));
				StreamOut((char *) &T->Lower, sizeof(float));
				StreamOut((char *) &T->Upper, sizeof(float));
				break;
			}

			case BrSubset:
			{
				Bytes = (MaxAttVal[T->Tested]>>3) + 1;

				ForEach(v, 1, T->Forks)
				{
					StreamOut((char *) T->Subset[v], Bytes);
				}
				break;
			}
		}

		ForEach(v, 1, T->Forks)
		{
			OutTree(T->Branch[v]);
		}
	}
}



/*************************************************************************/
/*									 */
/*	Retrieve entire decision tree with extension Extension		 */
/*									 */
/*************************************************************************/


Tree GetTree(C45_String Extension)
{
	Tree Hold = Nil;
	static char *LastExt="";

	if (TRf) 
	{
		fclose (TRf);
	}

	//strcpy(Fn, FileName);
	StringCbCopy (Fn, MAX_CHAR, FileName);
	//strcat(Fn, Extension);
	StringCbCat (Fn, MAX_CHAR, Extension);

	if (!(TRf = fopen (Fn, "rb"))) 
	{
		Error(0, Fn, "");
	}

	if (!TRf || getc(TRf) == EOF) 
	{
		return Nil;
	}

	Hold = InTree();

	RecoverDiscreteNames();

	return Hold;
}


/*************************************************************************/
/*									 */
/*	Retrieve tree from saved characters				 */
/*									 */
/*************************************************************************/
Tree InTree()
{
	Tree T = Nil;
	DiscrValue v = 0;
	int Bytes = 0;

	T = (Tree) malloc(sizeof(TreeRec));

	StreamIn ((char *) &T->NodeType, sizeof(short));
	StreamIn ((char *) &T->Leaf,		sizeof(ClassNo));
	StreamIn ((char *) &T->Items,		sizeof(ItemCount));
	StreamIn ((char *) &T->Errors,	sizeof(ItemCount));

	T->ClassDist = (ItemCount *) calloc(MaxClass+1, sizeof(ItemCount));
	StreamIn((char *) T->ClassDist, (MaxClass + 1) * sizeof(ItemCount));

	if (T->NodeType)
	{
		StreamIn((char *) &T->Tested, sizeof(Attribute));
		StreamIn((char *) &T->Forks, sizeof(short));

		switch (T->NodeType)
		{
			case BrDiscr:
			{
				break;
			}

			case ThreshContin:
			{
				StreamIn((char *) &T->Cut, sizeof(float));
				StreamIn((char *) &T->Lower, sizeof(float));
				StreamIn((char *) &T->Upper, sizeof(float));
				break;
			}

			case BrSubset:
			{
				T->Subset = (Set *) calloc(T->Forks + 1, sizeof(Set));
				Bytes = (MaxAttVal[T->Tested]>>3) + 1;

				ForEach(v, 1, T->Forks)
				{
					T->Subset[v] = (Set) malloc(Bytes);
					StreamIn((char *) T->Subset[v], Bytes);
				}
			}
		}

		T->Branch = (Tree *) calloc(T->Forks + 1, sizeof(Tree));
		ForEach(v, 1, T->Forks)
		{
			T->Branch[v] = InTree();
		}
	}

   return T;
}



/*************************************************************************/
/*									 */
/*	Stream characters to/from file TRf from/to an address		 */
/*									 */
/*************************************************************************/


void StreamOut (C45_String s, int n)
{
	static int iFilePosition = 0;

   while ( n-- ) 
	{
		putc (*s++, TRf);
		iFilePosition++;
	}

	if (fflush (TRf))
	{
		StringCbPrintf (szBuffer, cbDest, "Error flushing TRf");
		SendOutput (szBuffer);
	}
}



void StreamIn (C45_String s, int n)
{
	static int iFilePosition = 0;
	int err;

   while ( n-- ) 
	{
		*s++ = getc(TRf);
		iFilePosition++;
	}
}



/*************************************************************************/
/*									 */
/*	Free up space taken up by tree Node				 */
/*									 */
/*************************************************************************/


void ReleaseTree (Tree Node)
{
    DiscrValue v = 0;

    if ( Node->NodeType )
    {
	ForEach(v, 1, Node->Forks)
	{
	    ReleaseTree(Node->Branch[v]);
	}
	cfree(Node->Branch);

	if ( Node->NodeType == BrSubset )
	{
	    cfree(Node->Subset);
	}

    }

    cfree(Node->ClassDist);
    cfree(Node);
}



/*************************************************************************/
/*									 */
/*	Construct a leaf in a given node				 */
/*									 */
/*************************************************************************/

Tree Leaf (ItemCount* ClassFreq, ClassNo NodeClass, ItemCount Cases, ItemCount Errors)
{
    Tree Node = Nil;

    Node = (Tree) calloc(1, sizeof(TreeRec));

    Node->ClassDist = (ItemCount *) calloc(MaxClass+1, sizeof(ItemCount));	
	memcpy(Node->ClassDist, ClassFreq, (MaxClass+1) * sizeof(ItemCount)); 
    
    Node->NodeType	= 0; 
    Node->Leaf		= NodeClass;
    Node->Items		= Cases;
    Node->Errors	= Errors;

    return Node; 
}


/*************************************************************************/
/*									 */
/*	Insert branches in a node 	                 		 */
/*									 */
/*************************************************************************/


void Sprout(Tree Node, DiscrValue Branches)
{
    Node->Forks = Branches;
    
    Node->Branch = (Tree *) calloc(Branches+1, sizeof(Tree));
}



/*************************************************************************/
/*									 */
/*	Count the nodes in a tree					 */
/*									 */
/*************************************************************************/

	
int TreeSize(Tree Node)
{
    int Sum=0;
    DiscrValue v = Nil;

    if ( Node->NodeType )
    {
	ForEach(v, 1, Node->Forks)
	{
	    Sum += TreeSize(Node->Branch[v]);
	}
    }

    return Sum + 1;
}



/*************************************************************************/
/*									 */
/*	Return a copy of tree T						 */
/*									 */
/*************************************************************************/


Tree CopyTree(Tree T)
{
    DiscrValue v = 0;
    Tree New = Nil;

    New = (Tree) malloc(sizeof(TreeRec));
    memcpy(New, T, sizeof(TreeRec));

    New->ClassDist = (ItemCount *) calloc(MaxClass+1, sizeof(ItemCount));
    memcpy(New->ClassDist, T->ClassDist, (MaxClass + 1) * sizeof(ItemCount));

    if ( T->NodeType )
    {
	New->Branch = (Tree *) calloc(T->Forks + 1, sizeof(Tree));
	ForEach(v, 1, T->Forks)
	{
	    New->Branch[v] = CopyTree(T->Branch[v]);
	}
    }

    return New;
}



/*************************************************************************/
/*									 */
/*	Save attribute values read with "discrete N"			 */
/*									 */
/*************************************************************************/


void SaveDiscreteNames()
{
    Attribute Att = 0;
    DiscrValue v = 0;
    int Length = 0;

    ForEach(Att, 0, MaxAtt)
    {
	if ( SpecialStatus[Att] != DISCRETE ) continue;

	StreamOut((char *) &MaxAttVal[Att], sizeof(int));

	ForEach(v, 1, MaxAttVal[Att])
	{
	    Length = strlen(AttValName[Att][v]) + 1;

	    StreamOut((char *) &Length, sizeof(int));
	    StreamOut((char *) AttValName[Att][v], Length);
	}
    }
}



/*************************************************************************/
/*									 */
/*	Recover attribute values read with "discrete N"			 */
/*									 */
/*************************************************************************/


void RecoverDiscreteNames()
{
    Attribute Att = 0;
    DiscrValue v = 0;
    int Length = 0;

    ForEach(Att, 0, MaxAtt)
    {
	if ( SpecialStatus[Att] != DISCRETE ) continue;

	StreamIn((C45_String) &MaxAttVal[Att], sizeof(int));

	ForEach(v, 1, MaxAttVal[Att])
	{
	    StreamIn((C45_String) &Length, sizeof(int));

	    AttValName[Att][v] = (char *) malloc(Length);
	    StreamIn(AttValName[Att][v], Length);
	}
    }
}

#endif //__TREES_CPP__