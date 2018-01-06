/*************************************************************************/
/*									 */
/*		Global data for C4.5					 */
/*		--------------------					 */
/*									 */
/*************************************************************************/

#ifndef __EXTERN_H__
 #define __EXTERN_H__

extern  short		MaxAtt;		/* max att number */
extern  short		MaxClass;	/* max class number */
extern  short		MaxDiscrVal;	/* max discrete values for any att */

extern  ItemNo		MaxItem;	/* max data item number */

extern  Description	*Item;		/* data items */

extern  DiscrValue	*MaxAttVal;	/* number of values for each att */

extern  char		*SpecialStatus;	/* special att treatment */

extern  C45_String		*ClassName;		/* class names */
extern  C45_String		*AttName;		/* att names */
extern  C45_String		**AttValName;	/* att value names */
extern  C45_String		FileName;		/* family name of files */

extern  Boolean		AllKnown;	/* true if there have been no splits
								   on atts with missing values above
								   the current position in the tree */

extern short VERBOSITY;	/* verbosity level (0 = none) */
extern short TRIALS;	/* number of trees to be grown */

extern Boolean	GAINRATIO;	/* true=gain ratio, false=gain */
extern Boolean	SUBSET;		/* true if subset tests allowed */
extern Boolean	BATCH;		/* true if windowing turned off */
extern Boolean	UNSEENS;	/* true if to evaluate on test data */
extern Boolean	PROBTHRESH;	/* true if to use soft thresholds */

extern  ItemNo	MINOBJS;	/* minimum items each side of a cut */
extern  ItemNo	WINDOW;		/* initial window size */
extern  ItemNo	INCREMENT;	/* max window increment each iteration */

extern  float	CF;		/* confidence limit for tree pruning */

#endif //__EXTERN_H__