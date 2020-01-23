This repository contains the implementation for a simple content delivery network using .NET with a GUI showing the list of files hosted on the server as well as the files present in the cache

The files hosted by the server are present in the folder called ‘ServerFiles’ in the Host Folder. Similarly, Cached files that have been downloaded from the server are stored in the ‘CacheFiles’ folder. The files downloaded by the client appear in the folder named Client that also contains the source code of the ‘Client’ file.

## Software required to run this module:
- The bat files use the command csc (C# Compiler) to compile the code. This can be downloaded from https://visualstudio.microsoft.com/downloads/ by navigating to Build Tools for Visual Studio 2019 under Tools for Visual Studio 2019
- Text editor like Visual Studio Code or Visual Studio

## Instructions to run:

- Open the folder named ‘Host’.
- Run the application _host.bat.
- Enter any key in the terminal window that opens up.
- Open the folder named ‘Cache’.
- Run the _Cache.bat file in this folder.
- Enter any key in the terminal window that pops up.
- Finally, open the folder named ‘Client’
- Run the _client.bat file in this folder.
- A GUI with the list of files hosted by the server will appear in a new window.
- Select one of the files that you want to download from the list of files.
- Once you click the download button, a message shall appear informing the user that the file has been downloaded.
- To view the file in the Cache GUI press the refresh Cache button.
- Cache Log can be checked by clicking the Display Log button in the GUI.
