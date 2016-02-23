REM @ECHO OFF

REM Clearing out stage\lib, because switching between
REM platforms doesn't necessarily rebuild these libs
REM properly.

SETLOCAL ENABLEEXTENSIONS

SET vBoostVersion=1_59

SET vFrom=%CD%
SET vBinv2=%CD%\bin.v2
SET vMe=%~n0
SET vParent=%~dp0

SET vConfiguration=%1
SET vBitness=%2
SET vVsVersion=%3

REM by default assume vc120
SET vPlatformToolset=vc120
SET vBuildId=v120

REM but change to vc140 when compiling with VS2015
IF "%vVsVersion%"=="14.0" (SET vPlatformToolset=vc140)

IF "%vVsVersion%"=="14.0" (SET vBuildId=v140)

SET vToolset=msvc-%vVsVersion%

SET vBuild=%vParent%build%vConfiguration%
SET vBuildStatic=%vParent%build%vConfiguration%static
SET vStage=%vParent%stage%vConfiguration%
SET vStageLib=%vStage%\lib
SET vStageStatic=%vStage%static
SET vStageStaticLib=%vStageStatic%\lib

SET vBuildLog=%vParent%\build.log

REM First check if the vBoostVersion dlls already exist.
REM If they do, exit this script, do nothing.

IF EXIST "%vStage%" (
 FOR /F %%i in ('dir /b "%vStageLib%\*%vBuildId%.dll"') do (
	ECHO DLLs already exist.
  GOTO :DONE
 )
)

IF EXIST "%vBinv2%" @RMDIR /S /Q %vBinv2%

IF EXIST "%vStage%" @RMDIR /S /Q %vStage%

IF EXIST "%vBuild%" @RMDIR /S /Q %vBuild%

IF EXIST "%vStageStatic%" @RMDIR /S /Q %vStageStatic%

IF EXIST "%vBuildStatic%" @RMDIR /S /Q %vBuildStatic%

ECHO Bootstrapping and building boost. %vConfiguration% %vBitness% %vToolset%.
@CALL %vFrom%\bootstrap.bat > NUL

REM The following libraries can be built for boost:
REM    atomic
REM    chrono
REM    context
REM    coroutine
REM    date_time
REM    exception
REM    filesystem
REM    graph
REM    graph_parallel
REM    iostreams
REM    locale
REM    log
REM    math
REM    mpi
REM    program_options
REM    python
REM    random
REM    regex
REM    serialization
REM    signals
REM    system
REM    test
REM    thread
REM    timer
REM    wave
REM To build only specific libraries use --with-<libname> on the
REM command-line to b2

ECHO Prepare headers

CALL %vFrom%\b2 headers > NUL

ECHO Build shared libs

CALL %vFrom%\b2 --layout=tagged --buildid=%vBuildId% toolset=%vToolset% warnings=off variant=%vConfiguration% link=shared threading=multi address-model=%vBitness% -d0 --stagedir=%vStage% --build-dir=%vBuild% --with-date_time --with-chrono --with-filesystem --with-locale --with-regex --with-system --with-thread --with-serialization stage > %vBuildLog%

IF "%ERRORLEVEL%" NEQ "0" ECHO BUILD FAILED BUILD FAILED BUILD FAILED


SETLOCAL ENABLEDELAYEDEXPANSION

ECHO Ensure proper naming

PUSHD %vStage%\lib
IF "%1"=="debug" (
	FOR /f "tokens=*" %%f IN ('dir /b *gd-%vBuildId%.lib') DO (
		@SET PTH=%%~dpf
		@SET newname=!PTH!lib%%f
		@MOVE "!PTH!%%f" "!newname!"
	)
	POPD
REM	COPY %vStage%\lib\*gd-%vBuildId%.dll ..\bin\Debug\
)

IF "%1"=="release" (
	FOR /f "tokens=*" %%f IN ('dir /b *mt-%vBuildId%.lib') DO (
		@SET PTH=%%~dpf
		@SET newname=!PTH!lib%%f
		@MOVE "!PTH!%%f" "!newname!"
	)
	POPD
REM	COPY %vStage%\lib\*mt-%vBuildId%.dll ..\bin\Release\
)

:DONE

REM Need to remove this extra directory, it gets created but isn't
REM in the Boost .gitignore, so it will show as if our Boost
REM submodule is modified. Removing this directory will ensure
REM we developers won't get confused - more than we are.

ECHO Ensure Boost isn't modified

SET vArchBin=%vFrom%\libs\config\checks\architecture\bin
IF EXIST "%vArchBin%" @RMDIR /S /Q %vArchBin%

ENDLOCAL

ECHO
ECHO Boost build complete.
ECHO

EXIT /B 0
