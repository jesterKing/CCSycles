C[CS]?ycles : CCycles and CSycles for Cycles
============================================

C[CS]?ycles aims to provide a C API around Cycles. Building on CCycles also a
C# wrapper is available.

*Note 1* current work relies on Boost 1.55.0 being in a specific location, and the
stage/ folder with libraries. A prebuilt version with properly named libs and
paths can be downloaded at: https://dl.dropboxusercontent.com/u/1769373/boost_1_55_0.7z

*Note 2* This is currently targeted at Windows platform only, but patches to
improve cross-platform compiling and executing are likely candidates for
acceptance

*Note 3* The grand plan is to improve CCycles and CSycles and prepare for
submission to Blender upstream repository. There are still many hurdles to jump
over before this project is ready for that. Until that moment main development
of both parts is conducted in this repository.

*Note 4* No OSL support effort has been made, as for the RhinoCycles plugin the focus is on CUDA support.

*Note 5* The C[CS]?ycles main developer is Nathan Letwory. You can contact him at nathan@mcneel.com or find him as jesterKing in IRC channel #blendercoders of the Freenode network.

*Note 6* CUDA precompiled kernels are found in a 'special' location. This needs to be made configurable as well.

ROADMAP / TODO
==============

See Notes above, most are TODO items that need to be tackled in a useful way before C[CS]?ycles is ready for upstream. In addition:

* Add the rest of shader and texture nodes
* Documentation
* Documentation
* Improve csycles_tester to do complete XML support
* JSON support

Cycles and dependencies
=======================

This folder contains the Cycles source code, C-API source code, C# wrapper
source code and source code for the necessary dependencies pthreads, glew,
clew, cuew and a modified OpenImageIO.

The Cycles source
=================

Current Cycles revision: 220fcd43a9e48c3ca7be766433495a54d5471d71 (previous: 9fcaac5009b567edc59c4831b6a0580211d1d290)

The Cycles source code is in the following folders:
  bvh (bounding volume hierarchy, https://en.wikipedia.org/wiki/Bounding_volume_hierarchy)
  device
  kernel
  render
  subd
  util

The 'doc' folder holds all license texts of Cycles and for the parts that are
directly used in Cycles.

OpenImageIO tools
=================

Only a small part of the OpenImageIO library is used on the Rhino version
of Cycles. Image loading is handled by Rhino existing image code.

pthreads
========

pthreads contains the threading library used by Cycles. The library is
compiled as a DLL.

clew
====

clew is the OpenCL execution wrangler library

cuew
====

cuew is the CUDA execution wrangler library

C-API and C# wrapper code
=========================

ccycles (C API)
csycles (C# wrapper around CCycles)
csycles_tester (C# tester program, reimplementation of Cycles
                standalone)

Building
========

1. Clone repository
2. Get boost_1_55_0.7z and extract in such location that it lives 4 folders up from cycles.sln
3. Open cycles.sln
4. Build solution for csycles_tester
5. run csycles_tester with an XML test file from Cycles stand-alone

License for CCycles and CSycles
===============================

Copyright 2014 Robert McNeel and Associates

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
