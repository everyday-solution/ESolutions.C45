/************************************************************************/
/*																		*/
/*	Main routine, AlgoC45												*/
/*	------------------													*/
/*																		*/
/************************************************************************/

#include "header.h"
#include "besttree.h"
#include "build.h"
#include "classify.h"
#include "confmat.h"
#include "contin.h"
#include "discr.h"
#include "getdata.h"
#include "getnames.h"
#include "getopt.h"
#include "info.h"
#include "prune.h"
#include "sort.h"
#include "st-thresh.h"
#include "stats.h"
#include "subset.h"
#include "trees.h"

void main (int Argc, char *Argv[])
{
    int o;    
    extern int optind;
    Boolean FirstTime=true;
	 //BATCH = false;
    short Best;

    PrintHeader("decision tree generator");

    /*  Process options  */

	while ((o = getopt(Argc, Argv, "f:bupv:t:w:i:gsm:c:")) != EOF)
	{
		if ( FirstTime )
		{
			printf("\n    Options:\n");
			FirstTime = false;
		}

		switch (o)
		{
			case 'f':
			{
				FileName = optarg;
				printf("\tFile stem <%s>\n", FileName);
				break;
			}

			case 'b':
			{
				BATCH = true;
				printf("\tWindowing disabled (now the default)\n");
				break;
			}

			case 'u':
			{
				UNSEENS = true;
				printf("\tTrees evaluated on unseen cases\n");
				break;
			}

			case 'p':
			{
				PROBTHRESH = true;
				printf("\tProbability thresholds used\n");
				break;
			}

			case 'v':
			{
				VERBOSITY = atoi(optarg);
				printf("\tVerbosity level %d\n", VERBOSITY);
				break;
			}

			case 't':
			{
				TRIALS = atoi(optarg);
				printf("\tWindowing enabled with %d trials\n", TRIALS);
				Check(TRIALS, 1, 10000);
				BATCH = false;
				break;
			}

			case 'w':
			{
				WINDOW = atoi(optarg);
				printf("\tInitial window size of %d items\n", WINDOW);
				Check(WINDOW, 1, 1000000);
				BATCH = false;
				break;
			}

			case 'i':
			{
				INCREMENT = atoi(optarg);
				printf ("\tMaximum window increment of %d items\n", INCREMENT);
				Check(INCREMENT, 1, 1000000);
				BATCH = false;
				break;
			}

			case 'g':
			{
				GAINRATIO = false;
				printf("\tGain criterion used\n");
				break;
			}

			case 's':
			{
				SUBSET = true;
				printf("\tTests on discrete attribute groups\n");
				break;
			}

			case 'm':
			{
				MINOBJS = atoi(optarg);
				printf("\tSensible test requires 2 branches with >=%d cases\n", MINOBJS);
				Check(MINOBJS, 1, 1000000);
				break;
			}

			case 'c':
			{
				CF = atof(optarg);
				printf("\tPruning confidence level %g%%\n", CF);
				Check(CF, Epsilon, 100);
				CF /= 100;
				break;
			}

			case '?':
			{
				printf("unrecognised option\n");
				exit(1);
				break;
			}
		}
	}

    /*  Initialise  */
	GetNames();
	GetData(".data");
   printf("\nRead %d cases (%d attributes) from %s.data\n", MaxItem+1, MaxAtt+1, FileName);

	/*  Build decision trees  */
	if ( BATCH )
	{
		TRIALS = 1;
		OneTree();
		Best = 0;
	}
	else
	{
		Best = BestTree();
	}

   /*  Soften thresholds in best tree  */

	if ( PROBTHRESH )
	{
		printf("Softening thresholds");

		if ( ! BATCH ) 
		{
			printf(" for best tree from trial %d", Best);
		}

		printf("\n");
		SoftenThresh(Pruned[Best]);
		printf("\n");
		PrintTree(Pruned[Best]);
	}

	/*  Save best tree  */

	if ( BATCH || TRIALS == 1 )
   {
		printf("\nTree saved\n");
	}
   else
	{
		printf("\nBest tree from trial %d saved\n", Best);
	}

	SaveTree(Pruned[Best], ".tree");

   /*  Evaluation  */
	printf("\n\nEvaluation on training data (%d items):\n", MaxItem+1);
	Evaluate(false, Best);

	if (UNSEENS)
   {   
		GetData(".test");
		printf("\nEvaluation on test data (%d items):\n", MaxItem+1);
		Evaluate(true, Best);
	}

   exit(0);
}
