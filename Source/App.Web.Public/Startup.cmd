ECHO Check if this task is running on the compute emulator.

IF "%ComputeEmulatorRunning%" == "true" (
    ECHO This task is running on the compute emulator. Perform tasks that must be run only in the compute emulator.
    GOTO END
)

ECHO This task is running on the cloud. Perform tasks that must be run only in the cloud.

ECHO Enable compression for HTTP 1.0 clients and proxies (Windows Azure CDN)
%windir%\system32\inetsrv\appcmd set config -section:httpCompression -noCompressionForHttp10:false
%windir%\system32\inetsrv\appcmd set config -section:httpCompression -noCompressionForProxies:false

ECHO Add a compression section to the Web.config file.
%windir%\system32\inetsrv\appcmd set config -section:urlCompression -doDynamicCompression:true -commit:apphost

:END
ECHO Done.
