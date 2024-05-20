# Theory of operation

## High level description

The software consists of two components, the GUI control application and a DLL that is injected into ACC that hooks the network routines.

The DLL is loaded using [DLL search order hijacking](https://medium.com/@sealteamsecs/dll-search-order-hijacking-c9c46ea9026c) technique
to get our code loaded into the ACC process and uses the fantastic [minhook library](https://github.com/TsudaKageyu/minhook) to hook into
the network routines of ACC to detect when a LAN server discovery is attempted and produce fake reply packets representing the configured
servers. The servers to fake are received from the GUI application via a named pipe.

## ACC LAN server discovery protocol

ACC discovers servers on the same LAN by sending a small broadcast (or targeted unicast when `serverList.json` trick is used) UDP packet to
port 8999 and listening for replies. Each discovery attempt has a unique integer id, replies with non-matching id are discarded.

## Loading the DLL into ACC process

DLL search order hijacking was chosen as the technique for loading the DLL as it is fairly commonly used by game mods already (such as the
Custom Shaders Patch for the original Assetto Corsa) and as such is less commonly flagged by anti-cheat and anti-virus software as dangerous.

The DLL chosen for the hijack is `hid.dll`, this was based on it having a fairly small API surface that needs to be proxied while also being
on the critical path so that any failure of the proxy code is immediately obvious (the game will fail to detect any USB input devices). There
are other options that could be used were this to break in later game updates.

## DLL operation

Upon being loaded, the DLL hooks its own wrapper functions over `recvfrom()` and `sendto()` from `ws2_32.dll`.

When `sendto()` is called, the wrapper code checks if the packet being sent looks like a LAN discovery request and if it does, reads the
current server list from the GUI via a named pipe and sets the DLL internal state to reflect an in-progress discovery. Finally, the call is
passed through to the original `sendto()` function.

The game repeatedly calls `recvfrom()` while waiting for responses to a LAN discovery request. When the function is called, the wrapper code
checks if the DLL internal state is indicating an in-progress discovery, in which case it will immediately build a packet reflecting the next
server to be returned and return back to the game code. If no discovery is ongoing or we have already returned all the servers, the code
simply forwards the call to the original `recvfrom()` function.
