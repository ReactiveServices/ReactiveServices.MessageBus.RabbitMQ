﻿/*
Reactive Services
 
Copyright (c) Rafael Romão 2015
 
All rights reserved.
 
MIT License
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
associated documentation files (the ""Software""), to deal in the Software without restriction,
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO
THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS
OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("ReactiveServices.MessageBus.RabbitMQ")]
[assembly: AssemblyDescription("Reactive Services library containing a RabbitMQ implementation of the Reactive Services Message Bus client")]
[assembly: Guid("f9473de4-d0d5-45bf-8bf9-a5f066399a0c")]

[assembly: AssemblyProduct("Reactive Services")]
[assembly: AssemblyCompany("Reactive Services")]
[assembly: AssemblyCopyright("Copyright © Rafael Romão 2015")]

[assembly: ComVisible(false)]

// These versions will be patched by AppVeyor CI
[assembly: AssemblyVersion("0.3.*")]
[assembly: AssemblyFileVersion("0.3.0.0")]
[assembly: AssemblyInformationalVersion("0.3.0.0")]

[assembly: InternalsVisibleTo("ReactiveServices.MessageBus.RabbitMQ.Tests")]