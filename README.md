# ACC Connector

Are you tired of being unable to play your favourite racing simulator because the lobby servers are down?
No more! ACC Connector allows direct IP based connection to any server.

# How to use

1. Download the installer from https://github.com/lonemeow/acc-connector/releases
1. Install the app
1. Click `Install hook` to have the hook installed into your ACC
1. Add a server or click on an ACC Connector server link
1. Start ACC
1. Click `LAN SERVERS`
1. Join the server

# Community

Join our [Discord community](https://discord.gg/frA8rB8ePZ) for support and discussion.

# Is this safe to use?

## Windows SmartScreen blocks it!

Known issue, the installer is not signed so Windows things it's suspicious. Signing the installer does not _really_ signinify anything
about its safety, but I am working on getting it signed as soon as possible and will release a new version that is signed as soon as I can.

## The safety of the installer and app

The app is safe; it's source code is entirely public and can be reviewed by anyone with the skills to understand C++ and C# code.

It is also safe as the official releases are built using only software from Github and Microsoft; only public runners and official
tools from Microsoft etc are used - there can not be even accidental malware unless the official Github runners are infected.

## Safety of hooking into ACC

This app uses the same mechanism of hooking into ACC as many other unofficial game mods do; in particular this is the same mechanism as
Custom Shaders Patch for the older Assetto Corsa game uses. Some anti virus software _might_ flag this as suspicious and there is nothing
we can do about that.

# How can I help?

If you are a software developer, you can help by contributing pull requests that fix bugs or add features.

You can also help by making sure you let the server providers, league managers and other parties know you want them to publish ACC
Connector links for their servers!

# Creating URIs for ACC Connector

The application adds a custom URI handler for URIs with a scheme of `acc-connector`. The hostname part can be either IP or DNS name, and
the port should be the TCP port of the server (ACC communicates the UDP port through the TCP connection once established). Additionally,
you can add query parameters `persistent=true` to denote that the server is more or less permanently available at the specified address and can
be saved for future use, and `name=<myservername>` to show a more friendly name.

Here's an example URI that points to a non-existent server but is syntactically valid:
[acc-connect://my-dummy-server.not-a-real-domain.com:12765/?name=My+dummy+server&persistent=true](acc-connect://my-dummy-server.not-a-real-domain.com:12765/?name=My+dummy+server&persistent=true)

Many programs such as Discord don't translate arbitrary URI schemes to clickable links; you can use `https://lonemeow.github.io/acc-connector/?target=<acc-connect-uri>`
as a proxy to get clickable links that simply translate to the custom URI scheme. For your own web pages, it's probably better to just
use the custom scheme directly in links.
