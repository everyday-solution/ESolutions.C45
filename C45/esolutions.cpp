#include <esolutions.h>
#include "header.h"
#include "consult.h"

C45_RETCODE ReadDiscr (Attribute Att, 
							  Tree T, 
							  FEATURES features[])
{
	char Name[500] = {0};
	DiscrValue dv = 0, PNo =  0;
	float P = 0, PSum = 0;
	C45_RETCODE c45RetCode = C45_UNDEFINED_ERROR;

	ForEach(dv, 1, MaxAttVal[Att])
	{
		RangeDesc[Att].Probability[dv] = 0.0;
	}

	for (int i = 0; i < MAX_DISCRETE_ATTRIBUTES; i++)
	{
		if (features[Att].pDiscreteFeatureList)
		{
			if (features[Att].pDiscreteFeatureList->discreteFeature[i].fProbability > 0)
			{
				dv = Which (features[Att].pDiscreteFeatureList->discreteFeature[i].szAttribute, AttValName[Att], 1, MaxAttVal[Att]);

				if (!dv)
				{
					StringCbPrintf(szBuffer, cbDest, "\tPermissible values are %s", AttValName[Att][1]);
					SendOutput (szBuffer);

					ForEach(dv, 2, MaxAttVal[Att])
					{
						StringCbPrintf(szBuffer, cbDest, ", %s", AttValName[Att][dv]);
						SendOutput (szBuffer);
					}

					StringCbPrintf(szBuffer, cbDest, "\n");
					SendOutput (szBuffer);

					c45RetCode = C45_WRONG_ATTRIBUTE_VALUE;
					
					return C45_WRONG_ATTRIBUTE_VALUE;
				}
				RangeDesc[Att].Probability[dv] = features[Att].pDiscreteFeatureList->discreteFeature[i].fProbability / 100;

				StringCbPrintf (szBuffer, 
					cbDest, 
					"%s (%f) %%", 
					features[Att].pDiscreteFeatureList->discreteFeature[i].szAttribute,
					features[Att].pDiscreteFeatureList->discreteFeature[i].fProbability);
				SendOutput (szBuffer);
			}
		}
		RangeDesc[Att].Known = features[Att].bKnown;
	}

	//No consult error
	if (c45RetCode <= 100)
	{
		/*  Check that sum of probabilities is not > 1  */

		PNo = MaxAttVal[Att];
		PSum = 1.0;

		ForEach(dv, 1, MaxAttVal[Att])
		{
			if (RangeDesc[Att].Probability[dv] > Fuzz)
			{
				PSum -= RangeDesc[Att].Probability[dv];
				PNo--;
			}
		}

		if ( PSum < 0 || ! PNo && PSum > Fuzz )
		{
			StringCbPrintf(szBuffer, cbDest, "Probability values must sum to 1\n");
			SendOutput (szBuffer);
			return C45_WRONG_PROBYBILITY;
		}

		/*  Distribute the remaining probability equally among
		the unspecified attribute values  */

		PSum /= PNo;

		ForEach (dv, 1, MaxAttVal[Att])
		{
			if (RangeDesc[Att].Probability[dv] < Fuzz)
			{
				RangeDesc[Att].Probability[dv] = PSum;
			}
		}
	}

	return c45RetCode;
}

C45_RETCODE ReadContin (Attribute Att, Tree T, FEATURES features[MAX_FEATURES])
{
	C45_RETCODE c45RetCode = C45_SUCCESS;

	RangeDesc[Att].LowerBound	= features[Att].continousFeature.fLowerBound;
	RangeDesc[Att].UpperBound	= features[Att].continousFeature.fUpperBound;
	RangeDesc[Att].Known			= features[Att].bKnown;

	StringCbPrintf (szBuffer, 
		cbDest, 
		"%f - %f", 
		features[Att].continousFeature.fLowerBound, 
		features[Att].continousFeature.fUpperBound);

	return c45RetCode;
}

void SendOutput (char* szBuffer)
{
	if ((DEBUGMODE & DEBUG_CONSOLE) == DEBUG_CONSOLE)
	{
		printf (szBuffer);
	}	

	if ((DEBUGMODE & DEBUG_TCP_IP) == DEBUG_TCP_IP)
	{
		SendTCPMessage (szBuffer);
	}
}

void SendTCPMessage (char* szBuffer)
{
	static	bool	bConnected = false;
	static	int	iMaxMessage = 0;
	static	int	bytesSent = 0;

	if (connectSocket == INVALID_SOCKET)
	{
		TCPInitialize ();
	}

	if (connectSocket != INVALID_SOCKET)
	{
		if (!bConnected)
		{
			bConnected = TCPConnect ();
		}

		if (bConnected)
		{
			// Send data.
			char* pXMLPackage = (char*) malloc (MAX_XML_PACKAGE);
			XMLize (szBuffer, pXMLPackage);
			char* pBuffer = (char*) malloc (strlen (pXMLPackage) + 4);

			if (PrependSize (pXMLPackage, pBuffer))
			{
				bytesSent += send (connectSocket, pBuffer, (strlen (pXMLPackage) + 4), 0);
			}
		}
	}
}

bool PrependSize (char* pPackage, 
						char* pBuffer)
{
	bool	bRetCode = false;
	int	iSize = strlen (pPackage);
	int	iSizeIndicator = 1;

	while (iSize / 10 > 0)
	{
		iSizeIndicator++;
		iSize /= 10;
	}

	if (iSize <= 9)
	{
		StringCbPrintf (pBuffer, 
			1 + iSizeIndicator + strlen (pPackage) + 1,
			"%i%i%s", 
			iSizeIndicator,
			strlen (pPackage),
			pPackage);
		
		bRetCode = true;
	}

	return bRetCode;
}

int XMLize (char* szBuffer, 
				char* pXMLStream)
{
	TRACE_PACKAGE package = {0};
	char pXMLDummy[MAX_XML_PACKAGE] = {0};

	GetHeader (&package);

	ConvertPeculiars (szBuffer, pXMLDummy);

	StringCbPrintf (pXMLStream, 
		MAX_XML_PACKAGE, 
		"<?xml version=\"1.0\" encoding=\"utf-8\" ?><package><label><sender>%s</sender></label><payload><tracepackage><header><customer>%s</customer><user>%s</user><station>%s</station><application>%s</application><application_version>%s</application_version><module>%s</module><module_version>%s</module_version><OS>%s</OS></header><data>%s</data></tracepackage></payload></package>\0",
		package.szHost,
		package.szCustomer,
		package.szUser,
		package.szStation,
		package.szApplication,
		package.szApplication_version,
		package.szModule,
		package.szModule_version,
		package.szOS,
		pXMLDummy);	

	return (strlen (pXMLStream));
}

void ConvertPeculiars (char* pXMLStream, 
							  char* pNewXMLStream)
{	
	ReplaceWord (pXMLStream, "&", "&amp;", pNewXMLStream);
	StringCbCopy (pXMLStream, MAX_XML_PACKAGE, pNewXMLStream);

	ReplaceWord (pXMLStream, "'", "&apos;", pNewXMLStream);
	StringCbCopy (pXMLStream, MAX_XML_PACKAGE, pNewXMLStream);

	ReplaceWord (pXMLStream, "<", "&lt;", pNewXMLStream);
	StringCbCopy (pXMLStream, MAX_XML_PACKAGE, pNewXMLStream);

	ReplaceWord (pXMLStream, ">", "&gt;", pNewXMLStream);
	StringCbCopy (pXMLStream, MAX_XML_PACKAGE, pNewXMLStream);

	ReplaceWord (pXMLStream, "\"", "&quot;", pNewXMLStream);
	StringCbCopy (pXMLStream, MAX_XML_PACKAGE, pNewXMLStream);

	ReplaceWord (pXMLStream, "Ä", "&#196;", pNewXMLStream);
	StringCbCopy (pXMLStream, MAX_XML_PACKAGE, pNewXMLStream);

	ReplaceWord (pXMLStream, "Ö", "&#214;", pNewXMLStream);
	StringCbCopy (pXMLStream, MAX_XML_PACKAGE, pNewXMLStream);

	ReplaceWord (pXMLStream, "Ü", "&#220;", pNewXMLStream);
	StringCbCopy (pXMLStream, MAX_XML_PACKAGE, pNewXMLStream);

	ReplaceWord (pXMLStream, "ä", "&#228;", pNewXMLStream);
	StringCbCopy (pXMLStream, MAX_XML_PACKAGE, pNewXMLStream);

	ReplaceWord (pXMLStream, "ö", "&#246;", pNewXMLStream);
	StringCbCopy (pXMLStream, MAX_XML_PACKAGE, pNewXMLStream);

	ReplaceWord (pXMLStream, "ü", "&#252;", pNewXMLStream);
	StringCbCopy (pXMLStream, MAX_XML_PACKAGE, pNewXMLStream);

	ReplaceWord (pXMLStream, "ß", "&#22;", pNewXMLStream);
	StringCbCopy (pXMLStream, MAX_XML_PACKAGE, pNewXMLStream);
}

char* ReplaceWord (char* pOldString, 
						 char* pSearchWord, 
						 char* pReplaceWord, 
						 char* pNewString)
{
	char* pString = NULL;
	int i = 0;
	pNewString[0] = '\0';

	while (1)
	{
		if ((pString = strstr (&pOldString[i], pSearchWord)) == NULL)
		{
			strcat (pNewString, &pOldString[i]);
			break;
		}
		else
		{
			strncat (pNewString, &pOldString[i], pString-&pOldString[i]);

			strcat (pNewString, pReplaceWord);
			i += pString + strlen(pSearchWord) - &pOldString[i];
		}
	}

	return pNewString;
}

bool GetHeader (PTRACE_PACKAGE pPackage)
{
	GetOSInfo (pPackage->szOS);

	/*
	package->szCustomer
	package->szUser,
	package->szStation,
	package->szApplication,
	package->szApplication_version,
	package->szModule,
	package->szModule_version,
	*/

	return true;
}

void GetOSInfo (char* szBuffer)
{
	OSVERSIONINFOEXW osvi = {0};
   BOOL bOsVersionInfoEx;

   // Try calling GetVersionEx using the OSVERSIONINFOEX structure.
   // If that fails, try using the OSVERSIONINFO structure.
   ZeroMemory(&osvi, sizeof(OSVERSIONINFOEX));
   osvi.dwOSVersionInfoSize = sizeof(OSVERSIONINFOEX);

   if (!(bOsVersionInfoEx = GetVersionEx ((OSVERSIONINFO*) &osvi)))
   {
      osvi.dwOSVersionInfoSize = sizeof (OSVERSIONINFO);
      if (!GetVersionEx ( (OSVERSIONINFO *) &osvi) ) 
		{
			StringCbCopy (szBuffer, strlen ("Unknown OS"), "Unknown OS");
		}
   }

   switch (osvi.dwPlatformId)
   {
      // Test for the Windows NT product family.
      case VER_PLATFORM_WIN32_NT:

         // Test for the specific product.
         if ( osvi.dwMajorVersion == 5 && osvi.dwMinorVersion == 2 )
				StringCbCopy (szBuffer, strlen ("Microsoft Windows Server 2003") + 1, "Microsoft Windows Server 2003");

         if ( osvi.dwMajorVersion == 5 && osvi.dwMinorVersion == 1 )
				StringCbCopy (szBuffer, strlen ("Microsoft Windows XP") + 1, "Microsoft Windows XP");

         if ( osvi.dwMajorVersion == 5 && osvi.dwMinorVersion == 0 )
				StringCbCopy (szBuffer, strlen ("Microsoft Windows 2000") + 1, "Microsoft Windows 2000");

         if ( osvi.dwMajorVersion <= 4 )
				StringCbCopy (szBuffer, strlen ("Microsoft Windows NT") + 1, "Microsoft Windows NT");

         // Test for specific product on Windows NT 4.0 SP6 and later.
         if (bOsVersionInfoEx)
         {
            // Test for the workstation type.
            if ( osvi.wProductType == VER_NT_WORKSTATION )
            {
               if( osvi.dwMajorVersion == 4 )
						StringCbCat (szBuffer, strlen (" Workstation 4.0") + 1, " Workstation 4.0");
               else if( osvi.wSuiteMask & VER_SUITE_PERSONAL )
						StringCbCat (szBuffer, strlen (" Home Edition") + 1, " Home Edition");
               else
						StringCbCat (szBuffer, strlen (" Professional") + 1, " Professional");
            }
            
            // Test for the server type.
            else if ( osvi.wProductType == VER_NT_SERVER || 
                      osvi.wProductType == VER_NT_DOMAIN_CONTROLLER )
            {
               if( osvi.dwMajorVersion == 5 && osvi.dwMinorVersion == 2 )
               {
                  if( osvi.wSuiteMask & VER_SUITE_DATACENTER )
							StringCbCat (szBuffer, strlen (" Datacenter Edition") + 1, " Datacenter Edition");
                  else if( osvi.wSuiteMask & VER_SUITE_ENTERPRISE )
							StringCbCat (szBuffer, strlen (" Enterprise Edition") + 1, " Enterprise Edition");
                  else if ( osvi.wSuiteMask == VER_SUITE_BLADE )
							StringCbCat (szBuffer, strlen (" Web Edition") + 1, " Web Edition");
                  else
							StringCbCat (szBuffer, strlen (" Standard Edition") + 1, " Standard Edition");
               }

               else if( osvi.dwMajorVersion == 5 && osvi.dwMinorVersion == 0 )
               {
                  if( osvi.wSuiteMask & VER_SUITE_DATACENTER )
							StringCbCat (szBuffer, strlen (" Datacenter Server") + 1, " Datacenter Server");
                  else if( osvi.wSuiteMask & VER_SUITE_ENTERPRISE )
							StringCbCat (szBuffer, strlen (" Advanced Server") + 1, " Advanced Server");
                  else
							StringCbCat (szBuffer, strlen (" Server") + 1, " Server");
               }

               else  // Windows NT 4.0 
               {
                  if( osvi.wSuiteMask & VER_SUITE_ENTERPRISE )
							StringCbCat (szBuffer, strlen (" Server 4.0, Enterprise Edition") + 1, " Server 4.0, Enterprise Edition");
                  else
							StringCbCat (szBuffer, strlen (" Server 4.0") + 1, " Server 4.0");
               }
            }
         }
         else  // Test for specific product on Windows NT 4.0 SP5 and earlier
         {
            HKEY hKey;
            char szProductType[MAX_CHAR];
            DWORD dwBufLen=MAX_CHAR;
            LONG lRet;

            lRet = RegOpenKeyEx( HKEY_LOCAL_MACHINE,
               "SYSTEM\\CurrentControlSet\\Control\\ProductOptions",
               0, KEY_QUERY_VALUE, &hKey );
            if( lRet != ERROR_SUCCESS )
               StringCbCopy (szBuffer, strlen ("Unknown OS") + 1, "Unknown OS");

            lRet = RegQueryValueEx( hKey, "ProductType", NULL, NULL,
               (LPBYTE) szProductType, &dwBufLen);
            if( (lRet != ERROR_SUCCESS) || (dwBufLen > MAX_CHAR) )
               StringCbCopy (szBuffer, strlen ("Unknown OS") + 1, "Unknown OS");

            RegCloseKey( hKey );

            if ( lstrcmpi( "WINNT", szProductType) == 0 )
					StringCbCat (szBuffer, strlen (" Workstation") + 1, " Workstation");
            if ( lstrcmpi( "LANMANNT", szProductType) == 0 )
					StringCbCat (szBuffer, strlen (" Server") + 1, " Server");
            if ( lstrcmpi( "SERVERNT", szProductType) == 0 )
					StringCbCat (szBuffer, strlen (" Advanced Server") + 1, " Advanced Server");

            printf( "%d.%d ", osvi.dwMajorVersion, osvi.dwMinorVersion );
         }

      // Display service pack (if any) and build number.

         if( osvi.dwMajorVersion == 4 && 
             lstrcmpi( (LPCSTR) osvi.szCSDVersion, "Service Pack 6" ) == 0 )
         {
            HKEY hKey;
            LONG lRet;

            // Test for SP6 versus SP6a.
            lRet = RegOpenKeyEx( HKEY_LOCAL_MACHINE,
               "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Hotfix\\Q246009",
               0, KEY_QUERY_VALUE, &hKey );
            if( lRet == ERROR_SUCCESS )					
					StringCbPrintf (szBuffer, MAX_CHAR, "%s Service Pack 6a (Build %d)\n", szBuffer, osvi.dwBuildNumber & 0xFFFF);
            else // Windows NT 4.0 prior to SP6a
            {
					StringCbPrintf (szBuffer, MAX_CHAR, "%s %s (Build %d)\n", szBuffer, osvi.szCSDVersion, osvi.dwBuildNumber & 0xFFFF);
            }

            RegCloseKey( hKey );
         }
         else // not Windows NT 4.0 
         {
				StringCbPrintf (szBuffer, MAX_CHAR, "%s %s (Build %d)\n", szBuffer, osvi.szCSDVersion, osvi.dwBuildNumber & 0xFFFF);
         }

         break;

      // Test for the Windows Me/98/95.
      case VER_PLATFORM_WIN32_WINDOWS:

         if (osvi.dwMajorVersion == 4 && osvi.dwMinorVersion == 0)
         {
				 StringCbCopy (szBuffer, strlen ("Microsoft Windows 95") + 1, "Microsoft Windows 95");
             if ( osvi.szCSDVersion[1] == 'C' || osvi.szCSDVersion[1] == 'B' )
					 StringCbCat (szBuffer, strlen (" OSR2"), " OSR2");
         } 

         if (osvi.dwMajorVersion == 4 && osvi.dwMinorVersion == 10)
         {
             StringCbCopy (szBuffer, strlen ("Microsoft Windows 98") + 1, "Microsoft Windows 98");
             if ( osvi.szCSDVersion[1] == 'A' )
					 StringCbCat (szBuffer, strlen (" SE"), " SE");
         } 

         if (osvi.dwMajorVersion == 4 && osvi.dwMinorVersion == 90)
         {
				StringCbCopy (szBuffer, strlen ("Microsoft Windows Millennium Edition\n") + 1, "Microsoft Windows Millennium Edition\n");
         } 
         break;

      case VER_PLATFORM_WIN32s:
			StringCbCopy (szBuffer, strlen ("Microsoft Win32s\n") + 1, "Microsoft Win32s\n");
         break;
   }
}
//Initialize TCP connection and returns the number of bytes which can be sent
void TCPInitialize (void)
{
	//----------------------
	// Initialize Winsock
	WSADATA wsaData;
	int iWSAState = WSAStartup (MAKEWORD(2,2), &wsaData);

	if (iWSAState != NO_ERROR)
	{
		printf("Error at WSAStartup()\n");
	}
	else
	{
		//----------------------
		// Create a SOCKET for connecting to server
		connectSocket = socket (AF_INET, SOCK_STREAM, IPPROTO_TCP);

		if (connectSocket == INVALID_SOCKET) 
		{
			printf ("Error at socket(): %ld\n", WSAGetLastError());
			WSACleanup();
		}
	}
}

bool TCPConnect ()
{
	bool bConnected = false;

	//----------------------
	// The sockaddr_in structure specifies the address family,
	// IP address, and port of the server to be connected to.
	sockaddr_in clientService; 
	clientService.sin_family = AF_INET;
	clientService.sin_addr.s_addr = inet_addr (SNIFFERHOST);
	clientService.sin_port = htons (SNIFFERPORT);

	//----------------------
	// Connect to server.
	if (connect (connectSocket, (SOCKADDR*) &clientService, sizeof (clientService)) == SOCKET_ERROR) 
	{
		printf( "Failed to connect.\n" );
		WSACleanup();
	}
	else
	{
		bConnected = true;
	}

	return bConnected;
}

void TCPCleanUp (void)
{
	if (connectSocket)
	{
		closesocket (connectSocket);
		WSACleanup ();
	}
	connectSocket = NULL;
}