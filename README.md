# APIMon

APIMon project makes it easier investigate behavior of the program on the level of API calls. With APIMon developer can specify expected program activity using Colored Petri Nets.

## The project consists of three relatively independent parts.

<b>APIMonLib</b> performs basic processing of intercepted API calls at the target of interception.
Defines useful PInvoke signatures for ntdll.dll, kernel32.dll, ws2_32.dll.
Wraps parameters and results of intercepted calls into messages and delivers them to processing facility.
Asynchronously sends them to injecting process for further processing.

<b>CPN</b> part defines specialized implementation of Colored Petri Nets (CPN) model for fast processing of API calls.
Provides effective mechanism to define CPNs in C#.LINQ
Provides extensions useful for API call monitoring

<b>APIMon</b> is the central part of the project.
Injects interception library to target processes with help of EasyHook
Defines useful building blocks for effective and readable description of APImonitoring CPNs
Assembles CPN capable of detecting malicious functionalities.
Redirects messages coming from target processes to CPN.
May launch, inject library, kill processes from predefined list.

APIMon project can not enforce security and can be easily avoided by the modern malware. Nevertheless APIMon is very useful tool for reverse engineering of the program behavior.

The application of Colored Petri Nets to malware detection is described in the articles:
A. G. Tokhtabayev, V. A. Skormin and A. M. Dolgikh, "Detection of Worm Propagation Engines in the System Call Domain using Colored Petri Nets", IPCCC 2008

A. Tokhtabayev, V. Skormin and A Dolgikh, “Expressive, Efficient and Obfuscation Resilient Behavior Based IDS” ESORICS 2010, September, 2010 Athens, Greece.
