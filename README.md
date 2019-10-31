# SQL Analyzer

[![Build status](https://ci.appveyor.com/api/projects/status/vdgcjc30q62c2rnx?svg=true)](https://ci.appveyor.com/project/olsh/sql-analyzer-net)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=sql-analyzer&metric=alert_status)](https://sonarcloud.io/dashboard?id=sql-analyzer-net)


A Roslyn-based analyzer for SQL related stuff in .NET


## Analyzers

### SQL001: SQL type is not specified

Noncompliant Code Example:  
```csharp
Query<Thing>("select * from Thing where Name = @Name", new { Name = abcde });
```

Compliant Solution:  
```csharp
Query<Thing>("select * from Thing where Name = @Name", new {Name = new DbString { Value = "abcde", IsFixedLength = true, Length = 10, IsAnsi = true }});
```

https://github.com/StackExchange/Dapper/blob/master/Readme.md#ansi-strings-and-varchar


### SQL002: SQL parameters mismatch

Noncompliant Code Example:  
```csharp
var dog = connection.Query<Dog>("select Age = @Age, Id = @Id", new { Id = guid });
```

Compliant Solution:  
```csharp
var dog = connection.Query<Dog>("select Age = @Age, Id = @Id", new { Age = (int?)null, Id = guid });
```

### SQL003: Using 'Query' method is not optimal here

Noncompliant Code Example:  
```csharp
var dog = connection.Query<Dog>("select * from dogs").Single();
```

Compliant Solution:  
```csharp
var dog = connection.QuerySingle<Dog>("select * from dogs");
```

https://github.com/StackExchange/Dapper#performance
