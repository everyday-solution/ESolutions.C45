//Constants

#ifndef __PRUNE_H__
 #define __PRUNE_H__

#define	LocalVerbosity(x)	if (Sh >= 0 && VERBOSITY >= x)
#define	Intab(x)		Indent(x, "| ")

//Prototypes
Boolean Prune(Tree T);
float EstimateErrors(Tree T, ItemNo Fp, ItemNo Lp, short Sh, Boolean UpdateTree);
void CheckPossibleValues(Tree T);

#endif //__PRUNE_H__