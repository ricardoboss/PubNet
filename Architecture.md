# PubNet Architecture

## High Level Block Diagram

```mermaid
flowchart LR
    nuget[NuGet Client] & dart[Dart Client] & website[PubNet Website] --> caddy["Reverse Proxy<br/>(Caddy)"]

    subgraph PubNet
        caddy --> frontend["Frontend<br/>(Blazor WASM)"] & api["API<br/>(ASP.NET)"]
        api & worker["Worker<br/>(.NET)"] --- common((Common Services))
        common --> db[("DB<br/>(Postgres)")] & storage[("Blob Storage<br/>(Minio)")] & search[("Search<br/>(Meilisearch)")]
    end
```

## API Overview

```mermaid
mindmap
  root((PubNet))
    Authentication(Authentication)
      Login(Login)
      Register(Register)
    Authors(Authors)
      By Username(By Username)
        Packages(Packages)
    Packages(Packages)
      Dart[Dart]
        New(New)
        Index(Index)
        By Name(By Name)
          Discontinue(Discontinue)
          By Version(By Version)
            Archive(Archive)
            Docs(Docs)
            Analysis(Analysis)
            Retract(Retract)
        Storage(Storage)
            Upload(Upload)
            Finalize(Finalize)
      NuGet[NuGet]
        Publish(Publish)
        index.json(index.json)
        Query(Query)
        Autocomplete(Autocomplete)
        Vulnerabilities(Vulnerabilities)
        By Id(By Id)
          By Version(By Version)
            Archive(Archive)
            Docs(Docs)
            Analysis(Analysis)
```

## Database Schema

```mermaid
erDiagram
    Author ||--o| Identity: has
    Author ||--o{ DartPackage: owns
    DartPackage ||--o{ DartPackageVersion: contains
    DartPackageVersion ||--o| DartPackageVersionAnalysis: has
    DartPendingArchive }o--|| Author: "uploaded by"
    Author ||--o{ NugetPackage: owns
    NugetPackage ||--o{ NugetPackageVersion: contains
    DartPackageVersion ||--|| PackageArchive: has
    NugetPackageVersion ||--|| PackageArchive: has
```
