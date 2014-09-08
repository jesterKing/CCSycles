Cycles and dependencies
=======================

This folder contains the Cycles source code, C-API source code, C# wrapper
source code and source code for the necessary dependencies pthreads, glew and
OpenImageIO.

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

Rhino additions to Cycles
=========================

c_api
csycles

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
