/*************************************************************************/
/*									 */
/*  This file is included in case your version of Unix doesn't include   */
/*  the getopt utility.  If it does, discard this file and amend the     */
/*  Makefile accordingly.						 */
/*									 */
/*  There is no copyright on this file.					 */
/*									 */
/*************************************************************************/


#include <stdio.h>
#include <header.h>
#include "getopt.h"

int optind = 1;
char *optarg;


int getopt(int Argc, char **Argv, char *Str)
{
    int Optchar = 0;
    char *Option = Nil;

    if ( optind >= Argc ) return EOF;

    Option = Argv[optind++];

    if ( *Option++ != '-' ) return '?';

    Optchar = *Option++;

    while ( *Str && *Str != Optchar ) Str++;
    if ( ! *Str ) return '?';

    if ( *++Str == ':' )
    {
	if ( *Option ) optarg = Option;
	else
	if ( optind < Argc ) optarg = Argv[optind++];
	else
	Optchar = '?';
    }

    return Optchar;
}
