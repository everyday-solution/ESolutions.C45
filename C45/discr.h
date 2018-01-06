#ifndef __DISCR_H__
#define __DISCR_H__

//Prototypes
void ComputeFrequencies(Attribute Att, ItemNo Fp, ItemNo Lp);
float DiscrKnownBaseInfo(DiscrValue KnownItems, ItemCount MaxVal);
void DiscreteTest(Tree Node, Attribute Att);
void EvalDiscreteAtt(Attribute Att, ItemNo Fp, ItemNo Lp, ItemCount Items);

#endif //__DISCR_H__