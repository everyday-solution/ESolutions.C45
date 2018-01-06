#ifndef __TREES_H__
 #define __TREES_H__

//Constants

#define	Tab		"|   "
#define	TabSize		4
#define	Width		80	/* approx max width of printed trees */

	/*  If lines look like getting too long while a tree is being
	    printed, subtrees are broken off and printed separately after
	    the main tree is finished	 */


//Prototyps
void Show(Tree T, short Sh);
short MaxLine(Tree St);
void ShowBranch (short Sh, Tree T, DiscrValue v);
void Indent(short Sh, char* Mark);
void SaveTree(Tree T, C45_String Extension);
void OutTree(Tree T);
void StreamOut (C45_String s, int n);
void StreamIn (C45_String s, int n);
Tree GetTree (C45_String Extension);
Tree InTree();
void SaveDiscreteNames();
void RecoverDiscreteNames();
void Sprout(Tree Node, DiscrValue Branches);
Tree Leaf (ItemCount* ClassFreq, ClassNo NodeClass, ItemCount Cases, ItemCount Errors);
void PrintTree(Tree T);
Tree CopyTree(Tree T);
int TreeSize(Tree Node);
void ReleaseTree (Tree Node);

#endif //__TREES_H__

