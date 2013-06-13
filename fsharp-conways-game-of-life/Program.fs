(*
Any live cell with fewer than two live neighbours dies, as if caused by under-population.
Any live cell with two or three live neighbours lives on to the next generation.
Any live cell with more than three live neighbours dies, as if by overcrowding.
Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
*)

open System

let VerticalBoundary = 25
let HorizontalBoundary = 80

type Universe = int * int list

let UniverseWithGlider = [ (0, 1); (1, 2); (2, 0); (2, 1); (2, 2) ]
let UniverseWithBlinker =  [ (2, 2); (2, 3); (2, 4) ]
let UniverseWithBeacon = [0 .. 3] |> List.map (fun i -> i / 2, i % 2) |> List.map (fun (x, y) -> [ (x, y); (x + 2, y + 2) ]) |> List.concat
let UniverseRandomLifeForm = 
    Console.Write("Provide cells count and then hit any key to get a new generation: ")
    let random = new Random()
    let liveCellsCount = Console.ReadLine() |> Int32.Parse
    [0 .. liveCellsCount] |> List.map (fun _ -> (random.Next(0, HorizontalBoundary), random.Next(0, VerticalBoundary))) |> Seq.distinct |> Seq.toList

let output universe = 
    Console.Clear()
    let rec outputInternal cellslist =
        match cellslist with
        | [] -> ()
        | head::tail -> 
            let x,y = head
            Console.SetCursorPosition(x, y)
            Console.Write('■')
            outputInternal tail
    outputInternal universe

let isCellInObservableUniverse (x,y) = x >= 0 && y >= 0 && x < HorizontalBoundary && y < VerticalBoundary

let getAdjacentCells (x, y) = 
    [0 .. 8] |> List.map (fun i -> (x + (i / 3) - 1, y + (i % 3) - 1))

let evolve universe =
    let allAdjacentCells = universe |> List.map getAdjacentCells |> List.concat |> Seq.distinct |> Seq.toList 
    let produceCellState (a, b) =
        let getAliveNeighbours (x:int, y:int) = (Math.Abs(a - x) <= 1 && Math.Abs(b - y) <= 1 && (a <> x || b <> y))
        let countOfNeighbours = universe |> List.filter getAliveNeighbours |> List.length
        let isCellAlive = universe |> List.filter (fun (x,y) -> x = a && y = b) |> List.length |> (fun l -> l > 0)
        let state = (a, b, isCellAlive, countOfNeighbours)
        state
    let shouldCellLive (_,_,isCellAlive, countOfNeighbours) =
        match isCellAlive, countOfNeighbours with
        | true, n when n >= 2 && n <= 3 -> true
        | false, n when n = 3 -> true
        | _ -> false
    let produceCell (x,y,_,_) = (x,y)

    allAdjacentCells |> List.map produceCellState |> List.filter shouldCellLive |> List.map produceCell |> List.filter isCellInObservableUniverse
    
let rec live universe =
    output universe
    let readKey = Console.ReadKey()
    match readKey.Key with
    | ConsoleKey.Escape -> universe
    | _ ->  universe |> evolve |> live

UniverseRandomLifeForm |> live |> ignore