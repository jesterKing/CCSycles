// Attempt to access CSycles through F#

open ccl;

type DeviceOrArg =
  | DeviceList of seq<Device>
  | ArgList of string []

// same function to loop over and print elements of two
// different sequence types. A string [] and a seq<Device>
let print_col list = 
  let doprint x l = for i in l do printfn x i
  match list with
  | DeviceList n ->  doprint "-- %A" n
  | ArgList y -> doprint "~~ %s" y


// small module to help parsing command-line options
module CommandLine =
  type VerbosityOption = Verbose | Silent
  type InOutFile = NoFile | SourceFile | OutputFile

  type FsyclesOptions = {
    verbose : VerbosityOption;
    file : string;
    output : string;
  }

  // tell our parser in what mode we are: single options, or
  // options with argument
  type ParseMode = Toplevel | Filename

  type ParserState = {
    options : FsyclesOptions;
    parseMode : ParseMode
    file: InOutFile
  }

  let parseTopLevel arg curState =
    let {options=optionsSoFar; file=currentFileType} = curState
    match arg with
    | "--silent" ->
      let newOptions = { optionsSoFar with verbose=Silent}
      printfn "in silent %A" newOptions
      {options = newOptions; parseMode = Toplevel; file = currentFileType}
    | "--source" ->
      {options = optionsSoFar ; parseMode = Filename; file = SourceFile}
    | "--out" ->
      {options = optionsSoFar ; parseMode = Filename; file = OutputFile}
    | x ->
        printfn "unknown argument %s" x
        {options = optionsSoFar ; parseMode = Toplevel; file = currentFileType}

  let parseFilenameOptions arg curState =
    let {options=optionsSoFar; file=currentFileType} = curState
    // match argument for one of the filename types (SourceFile, OutputFile)
    match arg with
    | (x:string) when x.StartsWith("-") ->
      printfn "option value can't start with dash"
      // skip this one, continue again on top level
      {curState with parseMode = Toplevel}
    | x -> 
      match currentFileType with
      | SourceFile ->
        // set the source file field
        let newOptions = { optionsSoFar with file=x}
        {options = newOptions; parseMode = Toplevel; file = NoFile}
      | OutputFile ->
        // set the output file field
        let newOptions = { optionsSoFar with output=x}
        {options = newOptions; parseMode = Toplevel; file = NoFile}
      | NoFile ->
        printfn "weird status NoFile"
        {curState with parseMode=Toplevel}

  let fold_function state element = 
    match state with
    | { options = optionsSoFar; parseMode = Toplevel } -> 
      parseTopLevel element state
    | { options = optionsSoFar; parseMode = Filename } -> 
      parseFilenameOptions element state

  let parse args =
    let defaultArgs = { verbose = Verbose; file = "scene_default.xml"; output = "test.png" }
    let initialState = { options = defaultArgs; parseMode = Toplevel; file=NoFile}

    let {options = options } = args |> Array.fold fold_function initialState

    options


[<EntryPoint>]
let main argv = 
  let options = CommandLine.parse argv
  printfn "-> %A <-" options
  let arglist = ArgList argv
  print_col arglist
  printfn ""
  CSycles.initialise()
  print_col (DeviceList Device.Devices)
  CSycles.shutdown()
  0 // done
