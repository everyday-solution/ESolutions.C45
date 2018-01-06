/*************************************************************************/
/*									 */
/*	Sorting utilities						 */
/*	-----------------						 */
/*									 */
/*************************************************************************/

#ifndef __SORT_CPP__
 #define __SORT_CPP__

#include "header.h"
#include "sort.h"

/*************************************************************************/
/*									 */
/*	Sort items from Fp to Lp on attribute a				 */
/*									 */
/*************************************************************************/


void Quicksort (ItemNo Fp, Attribute Lp, Attribute Att, void (*Exchange)(ItemNo, ItemNo))
{
    register ItemNo Lower, Middle;
    register float Thresh;
    register ItemNo i;

    if ( Fp < Lp )
    {
	Thresh = CVal(Item[Lp], Att);

	/*  Isolate all items with values <= threshold  */

	Middle = Fp;

	for ( i = Fp ; i < Lp ; i++ )
	{ 
	    if ( CVal(Item[i], Att) <= Thresh )
	    { 
		if ( i != Middle ) (*Exchange)(Middle, i);
		Middle++; 
	    } 
	} 

	/*  Extract all values equal to the threshold  */

	Lower = Middle - 1;

	for ( i = Lower ; i >= Fp ; i-- )
	{
	    if ( CVal(Item[i], Att) == Thresh )
	    { 
		if ( i != Lower ) (*Exchange)(Lower, i);
		Lower--;
	    } 
	} 

	/*  Sort the lower values  */

	Quicksort(Fp, Lower, Att, Exchange);

	/*  Position the middle element  */

	(*Exchange)(Middle, Lp);

	/*  Sort the higher values  */

	Quicksort(Middle+1, Lp, Att, Exchange);
    }
}

#endif //__SORT_CPP__