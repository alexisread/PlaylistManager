This borrows heavily from this project:
http://www.codeproject.com/Articles/17266/Drag-and-Drop-Items-in-a-WPF-ListView

Please excuse the code quality - it's a quick utility I'd knocked up.

The primary usage is for getting songs on to Satechi devices,
so that they play as per the order set.
The manager also writes a .pla playlist, which is suitable for playback on TomTom
satnav devices.

To use it, select the folder where your songs are and press scan.
The manager will search through all nested folders to provide a list of available
songs, ordered by creation date.
You can then load a playlist if you choose, to order the list of songs (simple .m3u and .pla supported).
If the add-all-files checkbox is ticked, then any files in the folder which are not on the playlist, are added at the end (flagged orange).
Red songs are where the song (mp3) is missing.
Orange songs are where the song (mp3) is there, but not specified in the playlist.
You can remove songs from the new playlist by pressing remove 
(note this doesn't touch the original folder!).
You can either drag the songs into order, or select a song and use the up/down buttons
to move it. This orders your new playlist.
When you're done, choose an output folder ie. create a folder on the SD card,
with the name of the playlist you want to create.
Next press copy files - the files are copied over in the correct order for playback!

As the manager creates a .pla playlist file, you can use this to (save) the playlist,
and load this file back in if you need to recreate the playlist.
Note that simple .m3u playlists are just the .pla playlists renamed with .m3u

I've had to create this utility, as the Satechi devices read the songs in the order they were COPIED to the card,
changing creation / modified / track number / name / album does not work.



This project and associated files are under the GPL3+ licence.

Alexis Read, Copyright 18/6/14.
