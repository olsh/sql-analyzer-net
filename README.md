# SQL Analyzer

[![Build status](https://ci.appveyor.com/api/projects/status/wbpd1xk21drdqy0t?svg=true)](https://ci.appveyor.com/project/olsh/sql-analyzer-net)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=sql-analyzer-net&metric=alert_status)](https://sonarcloud.io/dashboard?id=sql-analyzer-net)
[![codecov](https://codecov.io/gh/olsh/sql-analyzer-net/branch/master/graph/badge.svg)](https://codecov.io/gh/olsh/sql-analyzer-net)
[![Nuget](https://img.shields.io/nuget/v/SqlAnalyzer.Net)](https://www.nuget.org/packages/SqlAnalyzer.Net/)
[![Visual Studio Marketplace](https://img.shields.io/visual-studio-marketplace/v/olsh.sqlanalyzer?label=VS%20Market)](https://marketplace.visualstudio.com/items?itemName=olsh.sqlanalyzer)


A Roslyn-based analyzer for SQL related stuff in .NET

## Analyzers

| Rule\Library                                                                           |       Dapper       |       ADO.NET      |  Entity Framework  |
|----------------------------------------------------------------------------------------|:------------------:|:------------------:|:------------------:|
| [SQL001: SQL type is not specified](rules/SQL001.md)                                   | :heavy_check_mark: |                    |                    |
| [SQL002: SQL parameters mismatch](rules/SQL002.md)                                     | :heavy_check_mark: | :heavy_check_mark: |                    |
| [SQL003: Using 'Query' method is not optimal here](rules/SQL003.md)                    | :heavy_check_mark: |                    |                    |
| [SQL004: Using 'QueryMultiple' method is not optimal here](rules/SQL004.md)            | :heavy_check_mark: |                    |                    |
| [SQL005: Using 'SaveChanges' method in a loop can affect performance](rules/SQL005.md) |                    |                    | :heavy_check_mark: |