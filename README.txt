
# netTTS

## Overview

netTTS is a TCP/IP service that converts text to speech.  It can be
built from source code on any recent Windows machine without
requireing any tools that are not part of the standard Windows
installation.

However, Windows does not inclue the equivalent of "netcat", so in order to
test the installation you'll need either cygwin, WSL, a third party
netcat program or access to a linux system on your local network, more below.

The service uses .Net 4.0 and the System.Speech API to convert SSML
(or plain text) to .wav audio, which is returned to the client.

Using SSML, many parameters of text to speech can be customized over
arbitrary parts of the text, on a per query basis.  In addition to
SSML control, netTTS provides for adjustment of volume and rate
(amplitude and frequency scaling) that is sticky and applied to all
request, after SSML markup has been applied.

## Installation

download the source to %USERPROFILE%\source\repos so that you end up
with the netTTS directory directly under the repos directory.  I think
some versions of VS capitalize the 'repos' directory.  The scripts for
installing and managing netTTS have this part of the installation path
hardcoded, so be aware, you must have the exact path and case is
significant.  The path is hardcoded so that no environment variables
need to be set by the user.  This might have been a dumb decision :/

In a CMD window, without admin privs, type:
> cd %USERPROFILE%\source\repos
> git clone REPO
> cd netTTS
> .\scripts\doClean.bat
> .\scripts\doBuild.bat
> .\scripts\doRelease.bat

At this time, if you have a netcat program available, you can try out
netTTS as follows.  Note that the get/set Volume/Rate scripts are bash
scripts and need "nc".  I use cygwin to get bash and nc on Windows.
Any GNU/Linux machine on the same LAN as the netTTS service should be
able to run the scripts with little or no modification, You'll need to
edit the host "mark" in the test scripts and change it to a host or IP
address that matches the machine you're running netTTS on.

> .\scripts\doStart.bat 6620

Now in bash:

$ .\scripts\getRate
0
$ .\scripts\setRate 5
$ echo "Hi Mom" | nc mark 6620 > HiMom.wav
$ echo "Hi Dad" | nc mark 6620 > /dev/dsp # cygwin

To make the service available, in the background, when you log in type:

> .\scripts\doInstall.bat

All of the scripts assume the they are invoked from the netTTS root
directory, so they are prefixed with the "./scripts" directory when
invoking them.

The doStop.bat script stops the netTTS service and removes the
netTTS.pid file.  The .pid file contains the process id of the
currently running (if any), netTTS service.

The doShow.bat script shows information about any running netTTS
service.  The doChkInstall.bat script shows even more information,
including the locations of the netTTS log and pid files.

The doUninstall.bat script removes the side effects of installing
netTTS, except for leaving the installation directory under repos
intact.

## Query Protocol

By default, withthout any command prefix/suffix text sent to the
service is passed directly to the System.Speech.synthesizer API where
it is processed as either SSML or plain text and rendered as .wav
audio encoded speech.  You get whatever defaults .net
provides for your locale.

In addition, there are several explicit commands that get and set the
global volume and/or rate of rendered speech.  All commands are
delimited with '@->-' and '-<-@\n'.  A command name follows the prefix
delimiter.  The SET commands require a space followed by an argument.
All commands end with the suffix delimiter, which must end with
newline.

@->-GET-VOLUME-<-@\n
@->-GET-RATE-<-@\n
@->-SET-VOLUME [spec]-<-@\n
@->-SET-RATE [spec]-<-@\n

In the last two commands, [spec] takes one of either absolute or
relative form.

The absolute form is an integer without sign and the value or rate is
set to that value unless the value is out of range.  Out of range values
are ignored.

The relative form is a signed integer.  If the sign is '+' the integer
value is added to the current setting and if the result is within
range the volume or rate is changed.  If the sign is '-' the same is
done, except the value is subtracted from the current setting.

## To Do

* Timeout for request completion.
* TLS security, host based keys.
* Command line opts and/or TOML config
* Listener IP address generality (all, specific ip(s)).
* Verbose mode/logging
* Arbitrary installation directory.
* Get rid of cygwin for get/set scripts (only really needcs nc, so write C# nc)


Ends here.
