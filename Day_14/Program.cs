// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace Day_14;

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using ImageMagick;

partial class Program
{
    private const string DebugInput = """
                                      p=0,4 v=3,-3
                                      p=6,3 v=-1,-3
                                      p=10,3 v=-1,2
                                      p=2,0 v=2,-1
                                      p=0,0 v=1,3
                                      p=3,0 v=-2,-2
                                      p=7,6 v=-1,-3
                                      p=3,0 v=-1,-2
                                      p=9,3 v=2,3
                                      p=7,3 v=-1,2
                                      p=2,4 v=2,-3
                                      p=9,5 v=-3,-3
                                      """;
    private const int width = 101;//11;
    private const int height = 103;//7;
    
    static async Task Main(string[] args)
    {
        int quadrantA = 0;
        int quadrantB = 0;
        int quadrantC = 0;
        int quadrantD = 0;
        var robotEntryRegex = RobotEntryRegex();
       
        var matches = robotEntryRegex.Matches(await File.ReadAllTextAsync("input"));
        foreach (Match match in matches)
        {
            var startingPointX = int.Parse(match.Groups[1].Value);
            var startingPointY = int.Parse(match.Groups[2].Value);
            
            
            var velocityX = int.Parse(match.Groups[3].Value);
            var velocityY = int.Parse(match.Groups[4].Value);
            
            var endPointX = PositiveMod(startingPointX + velocityX * 100, width);
            var endPointY = PositiveMod(startingPointY + velocityY * 100, height);
            if (endPointX < MathF.Floor(width / 2f) && endPointY < MathF.Floor(height / 2f))
            {
                quadrantA++;
            }
            if (endPointX > MathF.Floor(width / 2f) && endPointY < MathF.Floor(height / 2f))
            {
                quadrantB++;
            }
            if (endPointX < MathF.Floor(width / 2f) && endPointY > MathF.Floor(height / 2f))
            {
                quadrantC++;
            }
            if (endPointX > MathF.Floor(width / 2f) && endPointY > MathF.Floor(height / 2f))
            {
                quadrantD++;
            }
        }
        Console.WriteLine($"SafetyFactor: {quadrantA*quadrantB*quadrantC*quadrantD}");
        
        var _robots = new List<(Vector2,Vector2)>();
        
        foreach (Match match in matches)
        {
            var startingPointX = int.Parse(match.Groups[1].Value);
            var startingPointY = int.Parse(match.Groups[2].Value);
            var velocityX = int.Parse(match.Groups[3].Value);
            var velocityY = int.Parse(match.Groups[4].Value);
            _robots.Add((new Vector2(startingPointX, startingPointY), new Vector2(velocityX, velocityY)));
        }
        
        ulong i = 1;
        Directory.CreateDirectory("trees");
        var startOfRobots = _robots.Select(x => (x.Item1, x.Item2)).ToArray();
        var safeTasks = new List<Task>();
        var images = new List<MagickImage>();
        do
        {
            for (var j = 0; j < _robots.Count; j++)
            {
                var (point, velocits) = _robots[j];
                var vec = point + velocits;
                _robots[j] = (new Vector2(PositiveMod((int)vec.X, width), PositiveMod((int)vec.Y, height)), velocits);
            }
            
            var image = new MagickImage(MagickColors.Black, width, height);
            foreach (var (position, _) in _robots)
                image.GetPixelsUnsafe().SetPixel((int)position.X, (int)position.Y, [255, 0, 0]);
            images.Add(image);
            safeTasks.Add(image.WriteAsync($"trees\\{i++}.png", MagickFormat.Png));
        } while (!_robots.SequenceEqual(startOfRobots));
      
        await CreateLargeMontageAsync(images, "treeMontage.png");
        await Task.WhenAll(safeTasks);
    }
    
    public static async Task CreateLargeMontageAsync(List<MagickImage> images, string outputPath)
    {
        int totalImages = images.Count;
        uint gridColumns = (uint)Math.Ceiling(Math.Sqrt(totalImages));
        uint gridRows = (uint)Math.Ceiling((double)totalImages / gridColumns);
        var firstImage = images[0];
        uint imageWidth = firstImage.Width;
        uint imageHeight = firstImage.Height;

        using var montage = new MagickImage(
            MagickColors.Black, 
            imageWidth * gridColumns, 
            imageHeight * gridRows
        );

        for (uint i = 0; i < images.Count; i++)
        {
            var currentImage = images[(int)i];
            uint rowIndex = i / gridColumns;
            uint colIndex = i % gridColumns;
            uint xOffset = colIndex * imageWidth;
            uint yOffset = rowIndex * imageHeight;
            montage.Composite(currentImage, (int)xOffset, (int)yOffset, CompositeOperator.Over);
        }

        await montage.WriteAsync(outputPath, MagickFormat.Png);
    }
    
    [GeneratedRegex(@"p=(\d+),(\d+) v=(-?\d+),(-?\d+)")]
    private static partial Regex RobotEntryRegex();
    
    //PositiveMod function for c#, since % does positive and negative modulo ~_~
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PositiveMod(int a, int b)
    {
        return a % b + (b & a % b >> 31);
    }
}