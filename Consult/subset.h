#ifndef __SUBSET_H__
 #define __SUBSET_H__

//Variables

//Prototypes
void Combine(DiscrValue x, DiscrValue y, DiscrValue Last);
void Uncombine(DiscrValue x, DiscrValue y);
void PrintSubset(Attribute Att, Set Ss);
void EvalSubset(Attribute Att, ItemNo Fp, ItemNo Lp, ItemCount Items);
void SubsetTest(Tree Node, Attribute Att);

#endif //__SUBSET_H__