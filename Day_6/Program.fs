open System
open System.Collections.Generic
open System.IO

type Direction =
    | North = 0
    | East = 1
    | South = 2
    | West = 3
    
and Tile(isWall: bool, x: int, y: int, isStartingPoint: bool) =
    member val IsWall = isWall with get
    member val IsStartingPoint = isStartingPoint with get
    member val X = x with get
    member val Y = y with get

and GameUnit(x: int, y: int, startingDirection:Direction, tiles:Tile seq) =
    let mutable currentDirection = startingDirection
    let mutable X = x 
    let mutable Y = y
    member val IsInfinite = false with get,set
    member val Tiles = tiles
    member val TileMap : IDictionary<int*int, Tile> = tiles |> Seq.map (fun t -> (t.X, t.Y), t) |> dict
    
    member this.Turn() =
        currentDirection <-
            match currentDirection with
                | Direction.North -> Direction.East
                | Direction.East -> Direction.South
                | Direction.South -> Direction.West
                | Direction.West -> Direction.North
                | _ -> ArgumentOutOfRangeException() |> raise
            
    member inline this.GetTile(x, y) =
      let mutable tile = Unchecked.defaultof<Tile>
      if this.TileMap.TryGetValue((x, y), &tile) then
            Some(tile)
       else None
            
    member this.Peek()  =
       let x,y = match currentDirection with
                     | Direction.North -> X, Y-1
                     | Direction.East -> X+1, Y
                     | Direction.South -> X, Y+1
                     | Direction.West -> X-1, Y
                     | _ -> ArgumentOutOfRangeException() |> raise
       this.GetTile(x,y)

        
    member inline this.CanMove() =
        this.Peek().IsSome
    
    member this.Move() =
        match currentDirection with
            | Direction.North -> Y <- Y-1
            | Direction.East -> X <- X+1
            | Direction.South -> Y <- Y+1
            | Direction.West -> X <- X-1
            | _ -> ArgumentOutOfRangeException() |> raise
     
    member this.SolveInternal(hs : HashSet<int * int * Direction>) =
        seq {
                   if hs.Add(X, Y, currentDirection) then
                       let thisTile = this.GetTile(X, Y)
                       if thisTile.IsSome then
                           yield Tile(thisTile.Value.IsWall, thisTile.Value.X, thisTile.Value.Y, thisTile.Value.IsStartingPoint)
                       if this.CanMove() then
                           this.Move()
#if DEBUG
                           // Console.Clear()
                           // this.PrintBoard(hs)
                           // Console.ReadKey() |> ignore
#endif
                       let nextTile = this.Peek()
                       if nextTile.IsSome then
                           if nextTile.Value.IsWall then
                               this.Turn()
                           yield! this.SolveInternal(hs)
                   else
                      this.IsInfinite <- true
        }
       
    member this.PrintBoard(hs : HashSet<int*int*Direction>) =
       this.Tiles |> Seq.iter(fun i ->
           if i.X = 0 then
               Console.WriteLine()
               
           if (hs |> Seq.tryFind(fun a ->
               let x,y,_ = a
               x = i.X && y = i.Y)).IsSome && not(X = i.X && Y = i.Y) then
               Console.Write 'X'
           elif i.IsWall then
               Console.Write '#'
           elif X = i.X && Y = i.Y then
               Console.Write (match currentDirection with
                                | Direction.North -> '^'
                                | Direction.South -> 'v'
                                | Direction.East -> '>'
                                | Direction.West -> '<'
                                | _ -> ArgumentOutOfRangeException() |> raise)
           else
               Console.Write '.'
           )
       
    member inline this.GetPath() =
         let hs = HashSet<int*int*Direction>()
         this.SolveInternal(hs)
         
    member inline this.Solve() =
        this.GetPath() |> Seq.distinct |> Seq.length
        
let debugInput =
    """
....#.....
.........#
..........
..#.......
.......#..
..........
.#..^.....
........#.
#.........
......#...
    """
    
let getFileContent fileName =
    use sr = File.OpenText(fileName)
    sr.ReadToEnd()

let buildLists (fileContent: string) =
    fileContent.Split("\n", StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries)
        |> Seq.mapi(fun y chars ->
           chars |> Seq.mapi(fun x char ->
               let isWall = char = '#'
               let isStartingPoint = char = '^'
               Tile(isWall, x, y, isStartingPoint))
        )
        |> Seq.collect id

#if DEBUG
let input = buildLists(debugInput)
#else
let input = buildLists(getFileContent("input"))
#endif
let startingPoint = input |> Seq.find(_.IsStartingPoint)
let unit = GameUnit(startingPoint.X, startingPoint.Y, Direction.North, input)
Console.WriteLine(unit.Solve()+1)

#if DEBUG
let unitP2 = GameUnit(startingPoint.X, startingPoint.Y, Direction.North, input)
let path = unitP2.GetPath() |> Seq.distinct

let part2 = path |> Seq.map(fun i ->
                            input
                            |> Seq.map (fun j ->
                                if not i.IsStartingPoint && not i.IsWall then
                                    Some(Tile((j.X = i.X && j.Y = i.Y) || j.IsWall, j.X, j.Y, j.IsStartingPoint))
                                else
                                    None
                            )
                            |> Seq.filter(_.IsSome)
                            |> Seq.map(_.Value)
                            )
            |> Seq.distinct
let tasks = part2 |> Seq.map(fun bruteForce -> async {
       let nuUnit = GameUnit(startingPoint.X, startingPoint.Y, Direction.North, bruteForce)
       nuUnit.Solve() |> ignore
       return nuUnit.IsInfinite
    }
)

let part2Length = tasks 
                  #if DEBUG
                  |> Async.Sequential
                  #else
                  |> Async.Parallel
                  #endif
                  |> Async.RunSynchronously
                  |> Seq.filter(fun i -> i)
                  |> Seq.length

Console.WriteLine(part2Length)
#endif