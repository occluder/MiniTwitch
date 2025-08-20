[2025/08/20]
```

BenchmarkDotNet v0.15.2, Windows 10 (10.0.19044.6216/21H2/November2021Update)
AMD Ryzen 5 5500 3.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 9.0.304
  [Host]     : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2


```
| Method                | LineCount | Mean            | Error        | StdDev       | Allocated |
|---------------------- |---------- |----------------:|-------------:|-------------:|----------:|
| **Create**                | **1**         |        **30.09 ns** |     **0.553 ns** |     **0.591 ns** |         **-** |
| Create_Parse          | 1         |       388.92 ns |     3.139 ns |     2.783 ns |     688 B |
| Create_Parse_Map_Send | 1         |     1,093.60 ns |    11.128 ns |    10.409 ns |     552 B |
| **Create**                | **100**       |     **3,156.89 ns** |    **29.729 ns** |    **27.808 ns** |         **-** |
| Create_Parse          | 100       |    38,731.54 ns |   371.237 ns |   347.255 ns |   70800 B |
| Create_Parse_Map_Send | 100       |   108,045.17 ns | 1,421.830 ns | 1,260.415 ns |   54912 B |
| **Create**                | **1000**      |    **33,704.15 ns** |   **480.970 ns** |   **449.900 ns** |         **-** |
| Create_Parse          | 1000      |   414,617.39 ns | 6,293.317 ns | 5,886.773 ns |  737464 B |
| Create_Parse_Map_Send | 1000      | 1,112,885.07 ns | 2,444.676 ns | 2,167.141 ns |  566536 B |