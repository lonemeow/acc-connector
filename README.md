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

Known issue, the installer is not signed so Windows thinks it's suspicious. Signing the installer does not _really_ signify anything
about its safety, but I am working on getting it signed as soon as possible and will release a new version that is signed as soon as I can.

## My anti-virus complains about it!

Unfortunately, due to the way this works, some anti-virus software likes to think it's some kind of virus or malware (technically they are
not very wrong; this does in fact alter the behavior of an existing program). I don't know a good way around this yet, for now you will
likely have to tell your anti-virus software to ignore the fact that this is behaving in a suspicious way.

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

# Creating URLs for ACC Connector

The application adds a custom URI handler for URIs with a scheme of `acc-connect`. The hostname part can be either IP or DNS name, and
the port should be the TCP port of the server (ACC communicates the UDP port through the TCP connection once established). Additionally,
you can add query parameters `persistent=true` to denote that the server is more or less permanently available at the specified address and can
be saved for future use, and `name=<myservername>` to show a more friendly name.

Since many programs such as Discord won't show unknown protocol links as clickable, it's best to create links using a redirect page. This
repository hosts such, use links in format `https://lonemeow.github.io/acc-connector/?hostname=<hostname_or_ip>&port=<tcp_port>&name=<server_name>&persistent=<persistent>`
to create redirects (`name` and `persistent` are of course optional and should only be used if appropriate).

Here's a demo server link in clickable form: 
https://lonemeow.github.io/acc-connector/?hostname=18.217.125.33&port=9911&name=Demo+server&persistent=true
