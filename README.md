# escape_ls for CitizenFX

Escape the island gamemode for FiveM LS

To edit it, open `escape_ls.sln` in Visual Studio.

To build it, run `build.cmd`. To run it, run the following commands to make a symbolic link in your server data directory:

```dos
cd /d [PATH TO THIS RESOURCE]
mklink /d X:\cfx-server-data\resources\[local]\escape_ls dist
```

Afterwards, you can use `ensure escape_ls` in your server.cfg or server console to start the resource.
