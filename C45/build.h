#ifndef __BUILD_H__
#define __BUILD_H__



//Prototypes
void InitialiseTreeData();
void InitialiseWeights ();
Tree FormTree(ItemNo Fp, ItemNo Lp);
ItemNo Group(DiscrValue V, ItemNo Fp, ItemNo Lp, Tree TestNode);
void Swap(ItemNo a, ItemNo b);

#endif //__BUILD_H__