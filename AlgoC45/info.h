/*************************************************************************/
/*									 */
/*	Calculate information, information gain, and print dists	 */
/*	--------------------------------------------------------	 */
/*									 */
/*************************************************************************/

#ifndef __INFO_H__
 #define __INFO_H__

#include "buildex.i"

float TotalInfo(ItemCount V[], DiscrValue MinVal, DiscrValue MaxVal);
float Worth(float ThisInfo, float ThisGain, float MinGain);
void ResetFreq(DiscrValue MaxVal);
float ComputeGain(float BaseInfo, float UnknFrac, DiscrValue MaxVal, ItemCount TotalItems);
void PrintDistribution(Attribute Att, DiscrValue MaxVal, Boolean ShowNames);

#endif //__INFO_H__