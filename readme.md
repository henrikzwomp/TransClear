# TransClear

**A Windows thumbnail handler (shell extension) for Lxf- and Io-files**

Copyright © 2023 Henrik Persson (Parts taken from SharpShell are Copyright © 2014 Dave Kerr)

### Disclaimer & License

THIS SOFTWARE IS PROVIDED 'AS-IS', WITHOUT ANY EXPRESS OR IMPLIED WARRANTY. IN NO EVENT WILL THE AUTHOR BE HELD LIABLE FOR ANY DAMAGES ARISING FROM THE  USE OF THIS SOFTWARE.

Permission is granted to anyone to use this software for any purpose, including commercial applications, and to alter it and redistribute it freely, subject to the following restrictions:

- The origin of this software must not be misrepresented; you must not claim that you wrote the original software.
- Altered versions must be plainly marked as such, and must not be misrepresented as being the original software.

### About 

TransClear is a Thumbnail handler for Lxf- and Io-files made for Windows. 

It is a "shell extension" that will replace the normal file icon for files made by LEGO Digital Designer or LEGO Studio with an image of its contents. TransClear do not generate the image itself, it simply uses an already generated image that can be found in the files. The image is generated when you save a file. This means that TransClear has no control over how the image looks (other than its size). (Note that Windows will only show thumbnails for files when the correct views are chosen in the Explorer window)

This project is derived from SharpShell source code (Link below). The Handler was first constructed with mostly Sharpshell code. After I got it working, I then cut out everything I didn't need and rewrote anything I didn't like. I also had to make a few improvements to their IStream wrapper (ComStream.cs). It could not handle calls to the Read method with the offset parameter set to anything other than zero and it did not release the IStream object when it was done with it. The first problem meant that the wrapper wouldn't work with built in .net zip-file reader. The other problem would prevent a user from editing, moving and deleting a file for a minute or more after the Thumbnail was generated. The Installer was derived from SharpShell code in a similar way.

### Requirements 

- Windows Vista or newer
- .NET Framework 4.8

### Installing 

Start with creating a system restore point for Windows (as you always should before executing strange programs downloaded from the Internet ;) )

After that you will only need to execute Installer.exe and click the big button that is marked "Install". TransClear should now be loaded into the 
memory and be used every time the shell asks for a thumbnail for an lxf- or Io-file. The handler will stop working if you change the location of the 
TransClear files. If you want to move them, uninstall TransClear first and then install TransClear again after the move.

You might run into the problem that Windows won't generate any thumbnail for your old files. This is because Windows will save thumbnails for all files even if a thumbnail handler is missing. This means that some file might not get a new thumbnail until they have been modified or after the thumbnail cache has been cleared (You can google how to do this). 

If you don't see any thumbnails for any files (including non Lxf- or Io-files), your Windows might be configured to not generate them at all (This you can also google how to change).

### Known Issues

For some reason I can't get thumbnails to show consistently in the "Open File"-dialog window for LDD. Very annoying.

### Contact information and links

E-mail: henrik@zwomp.com

SharpShell: https://github.com/dwmkerr/sharpshell

### Release history

- 1.0 First public release. 
- 2.0 Support for Io-files added.