// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open ccl;

let devprint d =
  printfn "Device %A" d

let print_strings l =
  for i in l do
    printfn "%s " i

let print_devices ds =
  for d in ds do
    devprint d

[<EntryPoint>]
let main argv = 
  print_strings argv
  printfn ""
  CSycles.initialise()
  print_devices Device.Devices
  CSycles.shutdown()
  0 // return an integer exit code
