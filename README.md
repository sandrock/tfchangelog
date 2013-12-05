tfchangelog
===========

A very simple CLI program to display a TFS 2010 + changelog.

You specify the TFS collection URL and the source control path. You can specify a min/max changeset ID to fetch.

The output is simple text and can be redirected to a file is desired.

## Examples

Help message when not specifying arguments.

	> TfChangelog.exe
	Usage:
	TfChangelog /collection:TeamProjectCollectionUrl /path:$/ProjectName/Branch/ [/min:min] [/max:max] [/format:CSV]
	    /collection: Required. The URL the the TFS collection
	    /path:       Required. The source control path starting with $/
	    /min:        Optional. Minimum desired changeset ID
	    /max:        Optional. Maximum desired changeset ID
	    /format:     Optional. Not implemented yet
	
Example with project, with specific branch and minimum changeset.

	> TfChangelog.exe /collection:https://tfs.mycompany.com:8082/tfs/Collection1 /path:$/Release/ /min:887

	----
	
	C889 by ********* on 05/12/2013 14:11:13
	
	********
	*************
	
	----
	
	C888 by ********* on 05/12/2013 13:47:34
	
	**********************
	************
	
	----
	
	C887 by ***************** on 05/12/2013 13:16:14
	
	********************
	*******************************************
	***************************
	**************************************************************************************
	
	----

## Installation

I do not provide a compiled version of this app.

There is a build script Build.Release.bat to build the app without Visual Studio installed.

The Team Foundation binaries should be installed on your computer.

