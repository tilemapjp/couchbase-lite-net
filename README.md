couchbase-lite-net
==================

Native API port of Couchbase Lite for Android to C#.  
This branch is experimental implementation in iOS/Android, not tested well especially in replication with server function.

Running Tests
=============

.Net version:
-------------

The replication unit tests currently require a running
instance of `sync_gateway`. Prior to running the
replication tests, start `sync_gateway` with the following
command:

*nix:
   /path/to/sync_gateway -pretty -verbose=true Couchbase.Lite/Couchbase.Lite.Tests/Assets/GatewayConfig.json

Windows:
   {TBD}
   
Mobile version (iOS / Android)
------------------------------

Run Couchabase.Lite.{Touch/Droid}.Tests projects in your device.  
Replication test is not added in testcase now.

Current test is success if you run it as "Run Everything", but sometimes fail if you run each test only.  
I'm now analyzing why it is, but I think it is not problem of library but problem of NUnit TestRunner.

Porting Code
============

This project is a port of the Couchbase Lite portable Java codebase,
ported to C#.  The port was done with the assistance of Sharpen,
a tool that converts Java code to C#. An idiomatic C# public API
was defined in XML, and we used an XSLT stylesheet to generate
stubs for all C# types and members.

Once the Java source was bulk converted to C#, and the public API
stubs generated, we replaced the stubs one-by-one with the coverted
source, which we also refactored into idiomatic C#. We used temporary 
shims in some cases to simulate key Java classes/types that don't 
directly map to .NET Framework classes/types. Those shims that 
haven't yet been removed will disappear eventually.

The upstream Java project is:

    https://github.com/couchbase/couchbase-lite-java-core
    
The current codebase is based on commit:
	03895431fe71ed9ccd74c9cbc11ad88c8ae65602

Android sample
==============

Port modified to be used in Android Projects. A Couchbase Lite for Xam.Android sample can be found here: https://github.com/SotoiGhost/CouchXamAndroidSample
