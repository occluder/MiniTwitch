```

BenchmarkDotNet v0.15.6, Windows 10 (10.0.19044.6575/21H2/November2021Update)
AMD Ryzen 5 5500 3.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3


```
| Method                | LineCount | Mean          | Error         | StdDev        | Median        | Allocated |
|---------------------- |---------- |--------------:|--------------:|--------------:|--------------:|----------:|
| **Create**                | **1**         |      **31.60 ns** |      **0.661 ns** |      **1.408 ns** |      **30.98 ns** |         **-** |
| Create_Parse          | 1         |     389.41 ns |      5.285 ns |      4.685 ns |     390.17 ns |     688 B |
| Create_Parse_Map_Send | 1         |     882.13 ns |     13.109 ns |     11.621 ns |     877.73 ns |     552 B |
| **Create**                | **100**       |   **3,105.14 ns** |     **25.887 ns** |     **22.948 ns** |   **3,096.98 ns** |         **-** |
| Create_Parse          | 100       |  40,461.81 ns |    808.299 ns |    716.536 ns |  40,277.36 ns |   70800 B |
| Create_Parse_Map_Send | 100       |  88,711.75 ns |  1,515.037 ns |  1,417.166 ns |  87,883.01 ns |   54912 B |
| **Create**                | **1000**      |  **34,697.97 ns** |    **356.785 ns** |    **316.280 ns** |  **34,663.30 ns** |         **-** |
| Create_Parse          | 1000      | 430,142.75 ns |  8,547.709 ns |  9,145.953 ns | 429,267.16 ns |  737464 B |
| Create_Parse_Map_Send | 1000      | 925,647.85 ns | 17,444.460 ns | 20,089.066 ns | 916,212.26 ns |  566536 B |
