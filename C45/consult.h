#ifndef __CONSULT_H__
 #define __CONSULT_H__

#include <esolutions.h>

	/*  The interview module uses a more complex description of an
	    case called a "Range Description".   The value of an
	    attribute is given by
	    - lower and upper bounds (continuous attribute)
	    - probability of each possible value (discrete attribute)  */

struct ValRange
	{
	    Boolean	Known,		/* is range known? */
			Asked;		/* has it been asked? */
	    float	LowerBound,	/* lower bound given */
			UpperBound,	/* upper ditto */
			*Probability;	/* prior prob of each discr value */
	};
typedef struct ValRange *RangeDescRec;

extern short TRACE; 
extern RangeDescRec	RangeDesc; 
extern Tree DecisionTree;		/* tree being used */
extern float	*LowClassSum,	/* accumulated lower estimates */
					*ClassSum;		/* accumulated central estimates */ 

#define Fuzz	0.01			/* minimum weight */


//Prototypes
void ClassifyCase (Tree Subtree, float Weight, FEATURES pFeatures[MAX_FEATURES]);
float Interpolate (Tree t, float v);
float Area (Tree t, float v);
void InterpretTree ();
void Decision (ClassNo c, float p, float lb, float ub);
void InterpretSingleCase (FEATURES pFeatures[MAX_FEATURES], OUTCLASSES outClasses[MAX_OUTCLASSES]);

#endif //__CONSULT_H__