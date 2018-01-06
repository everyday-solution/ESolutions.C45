/*************************************************************************/
/*								  	 */
/*	Routine for printing confusion matrices				 */
/*	---------------------------------------				 */
/*								  	 */
/*************************************************************************/
#ifndef __CONFMAT_CPP__
#define __CONFMAT_CPP__

#include "header.h"
#include "confmat.h"

void PrintConfusionMatrix(ItemNo *ConfusionMat)
{
    short Row, Col;

    if ( MaxClass > 20 ) return;  /* Don't print nonsensical matrices */

    /*  Print the heading, then each row  */

    printf("\n\n\t");
    ForEach(Col, 0, MaxClass)
    {
	printf("  (%c)", 'a' + Col);
    }

    printf("\t<-classified as\n\t");
    ForEach(Col, 0, MaxClass)
    {
	printf(" ----");
    }
    printf("\n");

    ForEach(Row, 0, MaxClass)
    {
	printf("\t");
	ForEach(Col, 0, MaxClass)
	{
	    if ( ConfusionMat[Row*(MaxClass+1) + Col] )
	    {
		printf("%5d", ConfusionMat[Row*(MaxClass+1) + Col]);
	    }
	    else
	    {
		printf("     ");
	    }
	}
	printf("\t(%c): class %s\n", 'a' + Row, ClassName[Row]);
    }
    printf("\n");
}

#endif //__CONFMAT_CPP__