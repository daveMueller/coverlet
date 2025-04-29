﻿// Copyright (c) Toni Solarin-Sodara
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using System.Threading.Tasks;
using Coverlet.Core;
using Coverlet.Core.CoverageSamples.Tests;
using Coverlet.Core.Tests;
using Coverlet.Tests.Utils;
using Xunit;

namespace Coverlet.CoreCoverage.Tests
{
  public partial class CoverageTests
  {
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void SkipAutoProps(bool skipAutoProps)
    {
      string path = Path.GetTempFileName();
      try
      {
        FunctionExecutor.Run(async (string[] parameters) =>
        {
          CoveragePrepareResult coveragePrepareResult = await TestInstrumentationHelper.Run<AutoProps>(instance =>
                  {
                    instance.AutoPropsNonInit = 10;
                    instance.AutoPropsInit = 20;
                    int readValue = instance.AutoPropsNonInit;
                    readValue = instance.AutoPropsInit;
                    return Task.CompletedTask;
                  },
                  persistPrepareResultToFile: parameters[0], skipAutoProps: bool.Parse(parameters[1]));

          return 0;
        }, [path, skipAutoProps.ToString()]);

        if (skipAutoProps)
        {
          TestInstrumentationHelper.GetCoverageResult(path)
              .Document("Instrumentation.AutoProps.cs")
              .AssertNonInstrumentedLines(BuildConfiguration.Debug, 12, 13)
              .AssertNonInstrumentedLines(BuildConfiguration.Release, 12, 13)
              .AssertLinesCoveredFromTo(BuildConfiguration.Debug, 9, 11)
              .AssertLinesCovered(BuildConfiguration.Debug, (7, 1))
              .AssertLinesCovered(BuildConfiguration.Release, (10, 1));
        }
        else
        {
          TestInstrumentationHelper.GetCoverageResult(path)
              .Document("Instrumentation.AutoProps.cs")
              .AssertLinesCoveredFromTo(BuildConfiguration.Debug, 7, 13)
              .AssertLinesCoveredFromTo(BuildConfiguration.Release, 10, 10)
              .AssertLinesCoveredFromTo(BuildConfiguration.Release, 12, 13);
        }
      }
      finally
      {
        File.Delete(path);
      }
    }
  }
}
