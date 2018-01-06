#ifndef __CONTIN_H__
 #define __CONTIN_H__

//Variables
extern float
	*SplitGain,	/* SplitGain[i] = gain with att value of item i as threshold */
	*SplitInfo;	/* SplitInfo[i] = potential info ditto */

void EvalContinuousAtt(Attribute Att, ItemNo Fp, ItemNo Lp);
void ContinTest(Tree Node, Attribute Att);
float GreatestValueBelow(Attribute Att, float t);

#endif //__CONTIN_H__