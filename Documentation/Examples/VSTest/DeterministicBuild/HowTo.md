﻿To run test we need to generates packages to reference in on test project.
Run from repo root

```shell
C:\git\coverlet
λ dotnet pack
Microsoft (R) Build Engine version 16.5.0+d4cbfca49 for .NET Core
Copyright (C) Microsoft Corporation. All rights reserved.

  Restore completed in 73,36 ms for C:\git\coverlet\src\coverlet.core\coverlet.core.csproj.
  Restore completed in 73,41 ms for C:\git\coverlet\test\coverlet.testsubject\coverlet.testsubject.csproj.
  Restore completed in 73,33 ms for C:\git\coverlet\test\coverlet.tests.projectsample.excludedbyattribute\coverlet.tests.projectsample.excludedbyattribute.csproj.
  Restore completed in 73,34 ms for C:\git\coverlet\src\coverlet.collector\coverlet.collector.csproj.
  Restore completed in 73,35 ms for C:\git\coverlet\test\coverlet.tests.xunit.extensions\coverlet.tests.xunit.extensions.csproj.
  Restore completed in 75,92 ms for C:\git\coverlet\test\coverlet.integration.tests\coverlet.integration.tests.csproj.
  Restore completed in 73,41 ms for C:\git\coverlet\src\coverlet.console\coverlet.console.csproj.
  Restore completed in 73,36 ms for C:\git\coverlet\test\coverlet.tests.projectsample.empty\coverlet.tests.projectsample.empty.csproj.
  Restore completed in 73,47 ms for C:\git\coverlet\src\coverlet.msbuild.tasks\coverlet.msbuild.tasks.csproj.
  Restore completed in 73,37 ms for C:\git\coverlet\test\coverlet.core.tests.samples.netstandard\coverlet.core.tests.samples.netstandard.csproj.
  Restore completed in 76,37 ms for C:\git\coverlet\test\coverlet.collector.tests\coverlet.collector.tests.csproj.
  Restore completed in 77,05 ms for C:\git\coverlet\test\coverlet.integration.template\coverlet.integration.template.csproj.
  Restore completed in 77,2 ms for C:\git\coverlet\test\coverlet.core.performancetest\coverlet.core.performancetest.csproj.
  Restore completed in 87,7 ms for C:\git\coverlet\test\coverlet.core.tests\coverlet.core.tests.csproj.
  coverlet.core -> C:\git\coverlet\src\coverlet.core\bin\Debug\netstandard2.0\coverlet.core.dll
  coverlet.collector -> C:\git\coverlet\src\coverlet.collector\bin\Debug\netcoreapp2.0\coverlet.collector.dll
  coverlet.msbuild.tasks -> C:\git\coverlet\src\coverlet.msbuild.tasks\bin\Debug\netstandard2.0\coverlet.msbuild.tasks.dll
  coverlet.console -> C:\git\coverlet\src\coverlet.console\bin\Debug\net6.0\coverlet.console.dll
  Successfully created package 'C:\git\coverlet\bin\Debug\Packages\coverlet.collector.6.0.1-preview.6.g918cd179e0.nupkg'.
  Successfully created package 'C:\git\coverlet\bin\Debug\Packages\coverlet.collector.6.0.1-preview.6.g918cd179e0.snupkg'.
  Successfully created package 'C:\git\coverlet\bin\Debug\Packages\coverlet.msbuild.6.0.1-preview.6.g918cd179e0.nupkg'.
  Successfully created package 'C:\git\coverlet\bin\Debug\Packages\coverlet.msbuild.6.0.1-preview.6.g918cd179e0.snupkg'.
  Successfully created package 'C:\git\coverlet\bin\Debug\Packages\coverlet.console.6.0.1-preview.6.g918cd179e0.nupkg'.
  Successfully created package 'C:\git\coverlet\bin\Debug\Packages\coverlet.console.6.0.1-preview.6.g918cd179e0.snupkg'.
```

Add collectors package version generated to `"..\Documentation\Examples\VSTest\DeterministicBuild\XUnitTestProject1\XUnitTestProject1.csproj"`

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.13.0" />
    <PackageReference Include="xunit.v3" Version="1.1.0" />
    <PackageReference Include="xunit.runner.visualstudio" Version="3.0.2">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
    </PackageReference>
    <!-- version comes from coverlet.coverlet.collector.6.0.1-preview.8.gcb9b802a5f.snupkg -->
    <PackageReference Include="coverlet.collector.6.0.1-preview.8.gcb9b802a5f.nupkg" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\ClassLibrary1\ClassLibrary1.csproj" />
  </ItemGroup>

</Project>

```

Go to test project folder and run

```shell
C:\git\coverlet\Documentation\Examples\VSTest\DeterministicBuild (detbuilddocs -> origin)
λ dotnet test --collect:"XPlat Code Coverage" /p:DeterministicSourcePaths=true -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.DeterministicReport=true
Test run for C:\git\coverlet\Documentation\Examples\VSTest\DeterministicBuild\XUnitTestProject1\bin\Debug\netcoreapp3.1\XUnitTestProject1.dll(.NETCoreApp,Version=v6.0)
Microsoft (R) Test Execution Command Line Tool Version 17.5.0
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test execution, please wait...

A total of 1 test files matched the specified pattern.

Attachments:
  C:\git\coverlet\Documentation\Examples\VSTest\DeterministicBuild\XUnitTestProject1\TestResults\7305d38e-0134-4fda-a99c-3672b410f472\coverage.cobertura.xml
Test Run Successful.
Total tests: 1
     Passed: 1
 Total time: 1,3472 Seconds
```

You should see on output folder the coverlet source root mapping file generated.
This is the confirmation that you're running coverage on deterministic build.

```text
Documentation\Examples\VSTest\DeterministicBuild\XUnitTestProject1\bin\Debug\net6.0\CoverletSourceRootsMapping_XUnitTestProject1
```
