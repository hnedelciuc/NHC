# NHC
Needle in a Haystack in a Crypt (NHC) v1.0

-------
NHC DOES NOT USE, NOR DOES IT ATTEMPT TO USE THE INTERNET OR ANY OTHER NETWORK. IT IS A 100% OFFLINE APP.

THIS APP IS NOT A VIRUS, IT IS NOT A TROJAN, IT IS NOT A HACKING OR PHISHING TOOL OF ANY KIND. IT IS NOT MALWARE!
IT IS JUST A BAD FILE ARCHIVER, FOR CHRIST'S SAKE....
-------

Copyright (C) 2023 Horia Nedelciuc

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

-----------------
GENERAL OVERVIEW
-----------------

NHC is a file compression and encryption archive creator program for the Windows OS (similar to 7-Zip or WinRAR but much more primitive).
It takes files or folders as input, and then compresses and encrypts them to create an .NHC archive. Unfortunately, as of today, it does not
support .ZIP or .RAR or .7Z or any other type of archive. The output archive that it generates can be a solid/single archive, or it can be split
into parts (i.e. files) of desired size in case the output archive is too big and needs to be split for storage/transportation. The archive can
then be decrypted and decompressed to recreate the initial files at a desired output folder.

---------------------------------
TECHNICAL IMPLEMENTATION DETAILS
---------------------------------

* It uses it's own original archive container (with .NHC extension) for holding data.
* It uses .NET's compression capabilities for Deflate compression and Igor Pavlov's open source 7Z library for LZMA compression (https://www.7-zip.org).
* It can also combine both these compression algorithms to generate an even smaller-sized output archive in case the Max Compression option is chosen.
* It also uses open source code found on StackOverflow for handling long file names.

    A link to the profile of StackOverflow user wolf5, author of the long file name handling piece of code: 

    https://stackoverflow.com/users/37643/wolf5

    ...and his answer to a StackOverflow question which includes this piece of code can be found here:

    https://stackoverflow.com/questions/5188527/how-to-deal-with-files-with-a-name-longer-than-259-characters

    This reference is included in the source files also.

* Also, .NET's symmetric encryption capabilities (AES, TripleDES, RC2) are used for 128, 192, and 256bit encryption,
as well as my own (really bad, amateurish, not-to-be-ever-really-used-by-anyone) encryption algorithm called NeedleCrypt.

----------------------------------------------
REASONS FOR MAKING THIS CODE AVAILABLE ONLINE
----------------------------------------------

* It goes without saying that this app is open source. I am releasing it here under Apache 2.0 license. This seems like a good license. I don't know that much
about how open source licensing works. If you want me to change the license to something else, please provide an explanation and I'll see what we can do.
* This was a personal project I started working on as an educational exercise and as an instrument for learning the .NET platform and the C# programming language.
* It is full of bad programming patterns, bad coding, bad project files organization. This is an amateur project of a beginner developer, so please don't judge
too harshly.

I am putting this project online for two reasons:

1) it might serve as an inspiration for C# beginners as to how to implement:
* .NET's compression and encryption functionality, as well as
* LZMA encryption functionality freely provided by Igor Pavlov on his website (https://www.7-zip.org),
* support for long file name handling in .NET Framework applications by using a class of methods that call the Windows API developed by wolf5 (see link above), and
* use of .NET's WinForms to create a basic application, such as a file archiver.
* a custom-made archive container for storing metadata regarding the contents of the archive (file names, size, flags, etc), and the multitude of buffers of
compressed and encrypted data in a compact way

Hey, I know my code is bad. But it can still be useful as a reference for someone intersted in building something similar. And I hope people looking at this code
will end up writing something better. Take what is good, ignore what is bad.

2) it might serve as proof to antivirus services that this application means no harm and works just as indended (i.e. does not contain any
deceptive or malicious code or deceptive design or intent).

Here's my rant regarding antiviruses mistakenly labeling the executable of this code as malicious throughout the years. If you want you can go on reading,
but if you don't, you can skip this "RANT" section:

---------------------------------------------------------------
RANT RELATED TO THIS APP BEING MISTAKENLY LABELED AS MALICIOUS
---------------------------------------------------------------

For the past years as I worked on this project for fun in my free time, I've used this app personally and I've also shared it with a few family
members and very close friends, who have used it casually.

This code has never before been made publicly available, nor was it ever intended to be made available (at least, not in this "unfinished" form).
I used this app for myself, for fun, just for the sake and satisfaction of knowing that I created my own file archiver for personal use.

Unfortunately, I have repeatedly faced the problem of antiviruses marking it as dangerous, as a trojan/virus, receiving crazy (and quite original)
names like UACAttack, URSU (meaning "bear" in Romanian, like.... what??? how do these guys even come up with these names?). Obviously, it was a
false positive. I'm not even mad at these antivirus service providers, the app was not digitally signed, and I can see how they thought on a first
glance that it is "suspicious". But in reality the application has always worked the way it's user interface informs the user. There was never
any intent on my part to use it maliciously.

This app has undergone incremental modifications, as I added functionality as my free time allowed. However, throughout it's development lifecycle,
the code of this app has never contained anything remotely malicious or deceptive. There are no "secret" actions behind-the-scenes, there has
never been "hidden" functionality in this app. It does what it says to the user it does. The user interface is pretty straight-forward:
You have the ability to add files or folders to the list, then you choose the encryption algorithm, use a password or a random key,
choose compression level and then click Compress/Encrypt. It also works pretty much the same way for decompression/decryption.

Frankly, I don't quite understand how did these guys who reviewed their antivirus' "automatic threat submission" found anything dangerous about it?
They were certainly able to run the executable and "test it out" in some "sandbox" and see for themselves what it does. The decompiled/reverse engineered
code should reveal pretty clearly that this app does not use network functionality to connect anywhere. It has a user-friendly UI, clearly stating what
each button is doing, it even has an "About" button revealing the name of the author of the app! I don't even believe they ever received any personal complaints
from anyone regarding this app. I certainly never "complained" about my own app to them, nor did my family members or very close friends. These antivirus
staff persons have just assumed that there is something "fishy" and "dangerous", probably based on an unfortunate incident which was a mistake on my part,
which probably raised their suspicion, which I will describe below. But how in the world did they come up with these crazy virus names? 
What is it about this app that makes it worthy of such a nickname: "bear" ("URSU")?? Why not "COW", "FISH", or "CROCODILE"?

I remember the first time it was detected and labeled as a malicious program. I've just added the ability to create a shortcut for the application by
the pressing of a button in one of its menus, also I've added the ability to create a file extension association for .NHC in Windows Registry and add
a shortcut to the "Send To..." right-click context menu in Windows. In order to be able to do that, I had to add request for elevated access
(i.e. Run as Administrator) from the user before the app being able relaunch itself and begin registering its extension to the Windows registry.

But before I tell you how it became labeled as a virus, I will tell you a few more details for the sake of context. I am also developing a Christian
Apologetics website using C# MVC, and I use it as a personal statement of faith blog, adding articles and videos over there. I'm hosting that website on a
dedicated server for which I'm paying a monthly subscription to a hosting provider. I have remote access to that server via Remote Desktop functionality in
Windows and every once in a while I connect to that server remotely and manage the website (i.e upload new versions of the website to the server, debug problems,
... well, website administration stuff). Now when I upload content remotely on the server I sometimes compress necessary files locally with a file archiver
(like WinRAR or 7-Zip) and then upload that archive to the server and unpack the files there in order to update website data.

So here we arrive at "the incident". I made the "mistake" of uploading my NHC file archiver executable to that remote server to use it instead of WinRAR
or 7-Zip which I usually use to pack and unpack my website files. I said: "since I have my own archive creator, why not use it for this website admin tasks instead
of someone else's archive program?". Well.... it was a bad idea. As soon as I ran NHC on that server via Remote Desktop to unpack my stuff over there,
I might have made the mistake to try to create file associations for the .NHC extension, thus elevating the prompt to request Admin access. Immediately my
app became "detected" as malware. The Windows Defender solution used on the server probably thought that I'm a "hacker" connecting to a "victim" computer remotely
and running this "super dangerous trojan" for some "bad motives". All I did is run my own piece of software on my own server to decompress my own files...
But so it happened that Windows' malware detection red-flags got raised as NHC seemingly crossed "the line". I've stopped using NHC on that remote server as a file
archiving tool since then, but ever since that incident, whenever I make a change to my NHC app's code and recompile the binary executable, it automatically is
labeled as a variant of UACAttack or URSU or whatever any given antivirus chooses to label my "super-dangerous file archiver app". I had to stop using Norton Security
and then BitDefender on my several desktop/laptop computers which I work on since those antivirus solutions would just go crazy forcefully deleting my app and
making it impossible for me to use my own app on my own devices. Windows Defender was not that aggressive and I eventually was able to continue using my app on
my devices by adding an exclusion. But whenever I copy the executable on another Windows machine it automatically is "detected" as a "threat".

For the last few years I've just ignored the problem and even laughed about it. This was a personal project that I did for fun. I never intended it to
be offered as a viable program to others (at least not in this unfinished and unpolished form). Honestly, I'm not a good programmer. I am self-taught.
This is a piece of pretty bad code and programming practices and I was ashamed to make it available on GitHub or any similar code-sharing service.
The IT community is so harsh when it comes to code review. There are so many "purists" and "perfectionists" when it comes to writing and/or reviewing code.
I did not receive formal education in Computer Science and I don't know the best "design patterns" or "clean-code" rules. I was especially cautious to post
my source code online, because here people can leave mean comments under the comfort of anonynymity. I am fully aware that my code is bad. I know there are
some amazing software developers out there, much more talented and much more professional than I am so I didn't want to put this out here and become a target
of their ridicule.

I also do understand these guys working for antivirus solutions though, and I'm not even mad at them. They don't have the time, energy, or interest to analyse such
"automatic detections/submissions" thoroughly and there is also probably some automatic handling of such "detections" where probably some AI decides whether
the file is dangerous or not. But at this particular moment in time this issue started to annoy me and it even hurts a bit. My very modest and quite bad app
is being falsely labeled as malware and I (indirectly) feel accused of being a "hacker" or an "immoral person" and it just feels bad. It feels bad that some
antivirus employees are sitting in some office holding a meeting and probably deciding what "animal name" to use to label my so-called "malicious" code.
That's why I've decided to just make the source code available online, as is, flaws and all, for their's or anyone's inspection if they ever bother to take a look.

I will try to submit a link to this repository to antivirus makers as proof that this app is harmless. On my local computer, I have older versions of this code
that I could provide to any antivirus service who wants additional proof that this app is harmless. This code uploaded here on GitHub is the latest,
most up-to-date version of the source code of this app.

If NHC is "dangerous", well, it is just as "dangerous" as 7-Zip or WinRAR or any other respectable archiver. It works as intended and does what it's user interface
says it does. I've even included an "About" button on the user interface which shows, when pressed, the copyright information with my full name included.
I wonder if these antivirus services employees ever wondered how come this "idiot hacker super-villain" left his full name in the About section? What kind of
hacking tool creator does that? I didn't use a pseudonym or nickname, I actually used my real name, and you can find me on Facebook too!

If NHC can be used for malicious intent to encrypt someone's files, then any other archiver can do that as well.

NHC does not even attempt to connect to the internet! It is an 100% offline app. It is just a file archiver, that creates archives, compresses and encrypts them,
how did it ever become labeled a trojan? 7-Zip compresses and encrypts files as well. WinRAR also. What is it about the executable of this code that made
them think it is a hacking tool, if no one even personally/directly complained to them about it?

Sorry for my long rant. I just hope this code serves as a source of ideas for other begginner developers who are interested in building a similar app.
I hope they will end up writing beter code than I ever did.



------------
KNOWN ISSUES
------------


1) As I learned more about software development and file encryption I became aware of a security flaw in my code. For example, I now know that the IV
(initialization vector) has to be randomly generated at the moment of archive creation and has to be written/embedded in the archive when using a password-based
encryption. That is why, the current implementation (initial commit) which I am making avaiable here on GitHub has this security flaw of using a hardcoded IV
for all archives it creates using password encryption. The option of using a random encryption key currently available in this app is more secure than the option
of using a password-based encryption, since I am storing the IV inside the randomly-generated encryption key file, which is supposed to be stored in a safe place
anyway to ensure the security of the encrypted archive. I haven't gotten time lately to correct this issue (related to the IV being hardcoded), so please bear in
mind that such an issue exists. I will do my best to come up with a fix in future public commits.

2) NeedleCrypt is really an "amateur-grade" encryption algorithm. Please do not use it to encrypt your sensitive files. RC2 and TripleDES are outdated technology
also. Use AES encryption instead. I was just fooling around with encryption trying to experiment and understand how it works, this is how I ended up inventing this
lame NeedleCrypt algorithm. It's a bad design, bad code, it's slow, and I don't think it's any good in terms of security of data.

3) The "Use Buit-in Default" for encryption is basically no encryption protection at all. The contents are in fact encrypted, but I am using a hardcoded IV and key
stored in the executable so there is no protection at all. I should probably eventually rename that label to "No encryption" and skip the encryption phase
all together so that only compression takes place.

3) I know I am supposed to create test coverage and run automated tests to ensure code stability. I started this project when I was a complete beginner in C#
and I didn't know at that time how to write unit testing. I have not had time to add test coverage for this project up to this point.


-------------------------------------------
RUNNING THIS APP ON OTHER OPERATING SYSTEMS
-------------------------------------------

* The binary of this app should run without problems on ARM-based Windows devices, since Windows offers a built-in emulator for x86 code on its ARM-based
version of Windows.
* This app works well on x86-64 Linux machines via Wine/Mono.
* The app is also working on Intel-based macOS machines via CrossOver, a Wine/Mono-based app (https://www.codeweavers.com/crossover).
* I have not tested the app with CrossOver running on ARM-based (Apple-Silicon) Macs, nor with Wine/Mono running on ARM-based Linux devices.


-----------
DISCLAIMER
-----------

It should go without saying that this file archiver should be used "AS IS". It may contain several security flaws. It may contain several bugs which may make
your archives prone to corruption and you may lose your private files due to such potential bugs in the code which is made available here on GitHub.
Please use this code and the resulting compiled software at your own risk.

If you are implementing encryption or compression in your own app and want to use this code as a reference, please do not consider it a model to follow.
Especially please be reticent and cautious if you are implementing such functionality in a professional setting as a company/business employee developing
professional, proprietary applications. Remember, I am just a random noob on GitHub, I know nothing about encryption and compression really... And I'm not
even joking.

I will end by repeating this:

THIS APP IS NOT A VIRUS, IT IS NOT A TROJAN, IT IS NOT ANY OTHER KIND OF MALWARE. IT IS JUST A BAD FILE ARCHIVER, FOR CHRIST'S SAKE....

(C) 2023 Horia Nedelciuc, author of "Needle in a Haystack in a Crypt" (NHC) file archiver v1.0.



---


