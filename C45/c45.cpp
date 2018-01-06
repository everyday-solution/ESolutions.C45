#include <c45.h>

BOOL WINAPI DllMain (HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
	// Perform actions based on the reason for calling.
	switch (fdwReason)
	{ 
		case DLL_PROCESS_ATTACH:
		{
			break;
		}

		case DLL_THREAD_ATTACH:
		{
			break;
		}

		case DLL_THREAD_DETACH:
		{
			break;
		}

		case DLL_PROCESS_DETACH:
		{
			ReleaseTree (DecisionTree);			
         break;
		}
	}
	return TRUE;  // Successful DLL_PROCESS_ATTACH.
}

/// <summary>
/// Generates Descision Tree to file
/// </summary>
/// <param name="pszFilename">Filename (filestem) of xyz.name and xyz.data</param>
/// <param name="bGainRatio">If true, information gain ratio is used, otherwise the absolute information gain is used</param>
/// <param name="bSubtreeRaising">Subtree raising make the tree more narrow (faster), but less certain</param>
/// <param name="bProbtresh">Use soften threshold</param>
/// <param name="iConfidenceLevel">Confidence level for tree pruning, default 25 (%)</param>
/// <returns>C45 Returncode</returns>
DLL C45_RETCODE GenerateDTree (	char*				pszFilename, 
											bool				bGainRatio, 
											bool				bSubtreeRaising, 
											bool				bSoftTresh, 
											int				iConfidenceLevel, 
											DEBUGPARAMETER	debugParameter)
{
	C45_RETCODE iRetCode = C45_UNDEFINED_ERROR;
	FileName = pszFilename;
	VERBOSITY = debugParameter.iVerbosity;
	DEBUGMODE = debugParameter.iDebugMode;
	StringCbCopy (SNIFFERHOST, MAX_CHAR, debugParameter.szSnifferHost);
	SNIFFERPORT = debugParameter.iSnifferPort;

	short Best;

	GetNames();
	GetData (".data");

	StringCbPrintf (szBuffer, cbDest, "\nRead %d cases (%d attributes) from %s.data\n", MaxItem + 1, MaxAtt + 1, FileName);
	SendOutput (szBuffer);

	/*  Build decision trees  */	
	unsigned long dwStartTime = GetTickCount();
	Best = BestTree();
	statistics.iTreeGenerationTime = GetTickCount() - dwStartTime;

	/*  Soften thresholds in best tree  */

	if (bSoftTresh)
	{
		StringCbPrintf (szBuffer, cbDest, "Softening thresholds");
		SendOutput (szBuffer);
		StringCbPrintf(szBuffer, cbDest, " for best tree from trial %d\n", Best);
		SendOutput (szBuffer);
		SoftenThresh(Pruned[Best]);
		StringCbPrintf(szBuffer, cbDest, "\n");
		SendOutput (szBuffer);
		PrintTree(Pruned[Best]);
	}

	/*  Save best tree  */
	SaveTree(Pruned[Best], ".tree");
	DecisionTree = Pruned[Best];
	StringCbPrintf(szBuffer, cbDest, "\nTree saved\n");	
	SendOutput (szBuffer);

	/*  Evaluation  */

	StringCbPrintf (szBuffer, cbDest, "\n\nEvaluation on training data (%d items):\n", MaxItem+1);
	SendOutput (szBuffer);
	Evaluate(false, Best);

	//Evaluate trees produced on unseen cases
	/*
	if (UNSEENS)
	{   
		GetData(".test");
		StringCbPrintf(szBuffer, cbDest, "\nEvaluation on test data (%d items):\n", MaxItem+1);
		Evaluate(true, Best);
	}
	*/

	iRetCode = C45_SUCCESS;

	return iRetCode;
}

/// <summary>
/// Tries to classify feature data
/// </summary>
/// <param name="pszFilename">Filname of def-files</param>
/// <param name="pFeatures">Pointer to the first field of the features array</param>
/// <param name="iClasses">Number of result classes</param>
/// <param name="pszClassBuffer">Out-Parameter for classification result (class)</param>
/// <param name="piBestGuess">Pointer to the best guess probability</param>
/// <param name="piLowerBound">Pointer to the lower bound of the probability intervall</param>
/// <param name="piUpperBound">Pointer to the upper bound of the probability intervall</param>
/// <returns>Class</returns>
//DLL char* Consult (char* pszFilename, PFEATURES pFeatures[], char* pszClassBuffer, int *piProbability)
DLL C45_RETCODE Consult (	char*				pszFilename, 
									PFEATURES		pFeatures, 
									int*				pClassesCount, 
									POUTCLASSES*	pOutClasses, 
									DEBUGPARAMETER	debugParameter)
{
	C45_RETCODE c45RetCode = C45_UNDEFINED_ERROR;
	char* szClass = NULL;
	FEATURES features [MAX_FEATURES] = {0};
	OUTCLASSES outClasses [MAX_OUTCLASSES] = {0};

	VERBOSITY = debugParameter.iVerbosity;
	DEBUGMODE = debugParameter.iDebugMode;
	StringCbCopy (SNIFFERHOST, MAX_CHAR, debugParameter.szSnifferHost);
	SNIFFERPORT = debugParameter.iSnifferPort;

	//Mergin pFeatures with local feature struct array
	for (int i = 0; i < MAX_FEATURES; i++)
	{
		features[i] = *pFeatures++;
	}

	for (int i = 0; i < MAX_OUTCLASSES; i++)
	{
		outClasses[i].pClassBuffer =(PCLASSBUFFER) malloc (sizeof(PCLASSBUFFER));
	}

	Attribute a;

	FileName = (char*) malloc (MAX_CHAR);
	strcpy (FileName, pszFilename);
	
	GetNames();

	if (!DecisionTree)
	{
		DecisionTree = GetTree (".tree");
	}
	statistics.iTreeSize = DecisionTree->Items;

   if (TRACE) 
	{
		PrintTree (DecisionTree);
	}

   /*  Allocate value ranges  */
	RangeDesc = (struct ValRange *) calloc (MaxAtt+1, sizeof(struct ValRange));

	ForEach(a, 0, MaxAtt)
	{
		if ( MaxAttVal[a] )
		{
			RangeDesc[a].Probability = (float *) calloc (MaxAttVal[a]+1, sizeof(float));
		}
	}

   Clear();

	unsigned long dwStartTime = GetTickCount();
	InterpretSingleCase (features, outClasses);
	statistics.iDecisionTime = GetTickCount() - dwStartTime;

	//Merge determined output classes with out-parameter
	for (int i = 0; i < MaxClass; i++)
	{		
		(*pOutClasses)->fBestGuess = outClasses[i].fBestGuess;
		(*pOutClasses)->fLowerBound = outClasses[i].fLowerBound;
		(*pOutClasses)->fUpperBound = outClasses[i].fUpperBound;
		strcpy ((*pOutClasses)->pClassBuffer->szClassBuffer, outClasses[i].pClassBuffer->szClassBuffer);

		(*pOutClasses)++;
	}

	*pClassesCount = MaxClass;

	return c45RetCode;
}

DLL void GetStatistics (PSTATISTICS pStatistics)
{
	pStatistics->iTreeSize = statistics.iTreeSize;
	pStatistics->iDecisionTime = statistics.iDecisionTime;
	pStatistics->iTreeGenerationTime = statistics.iTreeGenerationTime;
}
