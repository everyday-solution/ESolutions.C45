#include <header.h>
#include <getopt.h>
#include <consult.h>
#include <getnames.h>
#include <trees.h>
#include <userint.h>

/*************************************************************************/
/*								  	 */
/*  Main routine for classifying items using a decision tree	  	 */
/*								  	 */
/*************************************************************************/
void main (int Argc, char *Argv[])
{
    int o;
    extern char *optarg;
    extern int optind;
    Attribute a;

    PrintHeader("decision tree interpreter");

    /*  Process options  */

    while ( (o = getopt(Argc, Argv, "tvf:")) != EOF )
    {
	switch (o)
	{
	    case 't':	TRACE = 1;
			break;
	    case 'v':	VERBOSITY = 1;
			break;
	    case 'f':	FileName = optarg;
			break;
	    case '?':	printf("unrecognised option\n");
			exit(1);
	}
    }

    /*  Initialise  */

    GetNames();

    DecisionTree = GetTree(".tree");
    if ( TRACE ) PrintTree(DecisionTree);

    /*  Allocate value ranges  */

    RangeDesc = (struct ValRange *) calloc(MaxAtt+1, sizeof(struct ValRange));

    ForEach(a, 0, MaxAtt)
    {
		if ( MaxAttVal[a] )
		{
			RangeDesc[a].Probability =
			(float *) calloc(MaxAttVal[a]+1, sizeof(float));
		}
	}

    /*  Consult  */

    Clear();
    while ( true )
    {
	InterpretTree();
    }
}
