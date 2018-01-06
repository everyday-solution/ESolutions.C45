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
    short Row = 0, Col = 0;

    if ( MaxClass > 20 ) return;  /* Don't print nonsensical matrices */

    /*  Print the heading, then each row  */

    StringCbPrintf(szBuffer, cbDest, "\n\n\t");
	 SendOutput (szBuffer);
    ForEach(Col, 0, MaxClass)
    {
	StringCbPrintf(szBuffer, cbDest, "  (%c)", 'a' + Col);
	SendOutput (szBuffer);
    }

    StringCbPrintf(szBuffer, cbDest, "\t<-classified as\n\t");
	 SendOutput (szBuffer);
    ForEach(Col, 0, MaxClass)
    {
	StringCbPrintf(szBuffer, cbDest, " ----");
	SendOutput (szBuffer);
    }
    StringCbPrintf(szBuffer, cbDest, "\n");
	 SendOutput (szBuffer);

    ForEach(Row, 0, MaxClass)
    {
	StringCbPrintf(szBuffer, cbDest, "\t");
	SendOutput (szBuffer);
	ForEach(Col, 0, MaxClass)
	{
	    if ( ConfusionMat[Row*(MaxClass+1) + Col] )
	    {
		StringCbPrintf(szBuffer, cbDest, "%5d", ConfusionMat[Row*(MaxClass+1) + Col]);
		SendOutput (szBuffer);
	    }
	    else
	    {
		StringCbPrintf(szBuffer, cbDest, "     ");
		SendOutput (szBuffer);
	    }
	}
	StringCbPrintf(szBuffer, cbDest, "\t(%c): class %s\n", 'a' + Row, ClassName[Row]);
    }
    StringCbPrintf(szBuffer, cbDest, "\n");
	 SendOutput (szBuffer);
}

#endif //__CONFMAT_CPP__