// Copyright (c) Toni Solarin-Sodara
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using System.Threading.Tasks;
using Coverlet.Core.CoverageSamples.Tests;
using Coverlet.Core.Tests;
using Coverlet.Core;
using Coverlet.Tests.Utils;
using Xunit;

namespace Coverlet.CoreCoverage.Tests
{
  public partial class CoverageTests
  {
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void SkipAutoPropsInRecords(bool skipAutoProps)
    {
      string path = Path.GetTempFileName();
      try
      {
        FunctionExecutor.RunInProcess(async (string[] parameters) =>
        {
          CoveragePrepareResult coveragePrepareResult = await TestInstrumentationHelper.Run<RecordWithPropertyInit>(instance =>
          {
            instance.RecordAutoPropsNonInit = string.Empty;
            instance.RecordAutoPropsInit = string.Empty;
            string readValue = instance.RecordAutoPropsInit;
            readValue = instance.RecordAutoPropsNonInit;
            return Task.CompletedTask;
          },
                  persistPrepareResultToFile: parameters[0], skipAutoProps: bool.Parse(parameters[1]));

          return 0;
        }, [path, skipAutoProps.ToString()]);

        if (skipAutoProps)
        {
          TestInstrumentationHelper.GetCoverageResult(path)
            .GenerateReport(show: true)
              .Document("Instrumentation.Records.cs")
              .AssertNonInstrumentedLines(BuildConfiguration.Debug, 12, 13)
              .AssertNonInstrumentedLines(BuildConfiguration.Release, 12, 13)
              .AssertLinesCovered(BuildConfiguration.Debug, (7, 1), (9, 1), (10, 1), (11, 1))
              .AssertLinesCovered(BuildConfiguration.Release, (10, 1));
        }
        else
        {
          TestInstrumentationHelper.GetCoverageResult(path)
            .GenerateReport(show: true)
              .Document("Instrumentation.Records.cs")
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

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void SkipRecordWithProperties(bool skipAutoProps)
    {
      string path = Path.GetTempFileName();
      try
      {
        FunctionExecutor.RunInProcess(async (string[] parameters) =>
        {
          CoveragePrepareResult coveragePrepareResult = await TestInstrumentationHelper.Run<ClassWithRecordsAutoProperties>(instance =>
          {
            return Task.CompletedTask;
          },
                      persistPrepareResultToFile: parameters[0], skipAutoProps: bool.Parse(parameters[1]));

          return 0;
        }, [path, skipAutoProps.ToString()]);

        if (skipAutoProps)
        {
          TestInstrumentationHelper.GetCoverageResult(path)
            .GenerateReport(show: true)
              .Document("Instrumentation.Records.cs")
              .AssertNonInstrumentedLines(BuildConfiguration.Debug, 18, 18)
              .AssertNonInstrumentedLines(BuildConfiguration.Release, 18, 18)
              .AssertLinesCovered(BuildConfiguration.Debug, (21, 1), (22, 1), (23, 1))
              .AssertLinesCovered(BuildConfiguration.Release, (22, 1));
        }
        else
        {
          TestInstrumentationHelper.GetCoverageResult(path)
            .GenerateReport(show: true)
              .Document("Instrumentation.Records.cs")
              .AssertLinesCovered(BuildConfiguration.Debug, (18, 1), (20, 1), (21, 1), (22, 1), (23, 1))
              .AssertLinesCovered(BuildConfiguration.Release, (18, 1), (20, 1), (22, 1));
        }
      }
      finally
      {
        File.Delete(path);
      }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void SkipInheritingRecordsWithProperties(bool skipAutoProps)
    {
      string path = Path.GetTempFileName();
      try
      {
        FunctionExecutor.RunInProcess(async (string[] parameters) =>
        {
          CoveragePrepareResult coveragePrepareResult = await TestInstrumentationHelper.Run<ClassWithInheritingRecordsAndAutoProperties>(instance =>
          {
            return Task.CompletedTask;
          },
            persistPrepareResultToFile: parameters[0], skipAutoProps: bool.Parse(parameters[1]));

          return 0;
        }, [path, skipAutoProps.ToString()]);

        if (skipAutoProps)
        {
          TestInstrumentationHelper.GetCoverageResult(path)
            .GenerateReport(show: true)
            .Document("Instrumentation.Records.cs")
            .AssertNonInstrumentedLines(BuildConfiguration.Debug, 28, 28)
            .AssertNonInstrumentedLines(BuildConfiguration.Release, 28, 28)
            .AssertLinesCovered(BuildConfiguration.Debug, (30, 1), (33, 1), (34, 1), (35, 1))
            .AssertLinesCovered(BuildConfiguration.Release, (34, 1));

        }
        else
        {
          TestInstrumentationHelper.GetCoverageResult(path)
            .GenerateReport(show: true)
            .Document("Instrumentation.Records.cs")
            .AssertLinesCovered(BuildConfiguration.Debug, (28, 1), (30, 1), (33, 1), (34, 1), (35, 1))
            .AssertLinesCovered(BuildConfiguration.Release, (28, 1), (30, 1), (34, 1));
        }
      }
      finally
      {
        File.Delete(path);
      }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void SkipInheritingRecordsWithPropertiesABC(bool skipAutoProps)
    {
      string path = Path.GetTempFileName();
      try
      {
        FunctionExecutor.RunInProcess(async (string[] parameters) =>
        {
          CoveragePrepareResult coveragePrepareResult = await TestInstrumentationHelper.Run<ClassWithRecordsEmptyPrimaryConstructor>(instance =>
            {
              return Task.CompletedTask;
            },
            persistPrepareResultToFile: parameters[0], skipAutoProps: bool.Parse(parameters[1]));

          return 0;
        }, [path, skipAutoProps.ToString()]);

        if (skipAutoProps)
        {
          TestInstrumentationHelper.GetCoverageResult(path)
            .GenerateReport(show: true)
            .Document("Instrumentation.Records.cs")
            .AssertNonInstrumentedLines(BuildConfiguration.Debug, 39, 39)
            //.AssertNonInstrumentedLines(BuildConfiguration.Release, 39, 39)
            .AssertLinesCovered(BuildConfiguration.Debug, (42, 1), (47, 1))
            .AssertLinesCovered(BuildConfiguration.Release, (45, 1));

        }
        else
        {
          TestInstrumentationHelper.GetCoverageResult(path)
            .GenerateReport(show: true)
            .Document("Instrumentation.Records.cs")
            .AssertLinesCovered(BuildConfiguration.Debug, (42, 1), (47, 1));
          //.AssertLinesCovered(BuildConfiguration.Release, (39, 1), (41, 1), (45, 1));
        }
      }
      finally
      {
        File.Delete(path);
      }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void SkipInheritingRecordsWithPropertiesABCDEF(bool skipAutoProps)
    {
      string path = Path.GetTempFileName();
      try
      {
        FunctionExecutor.RunInProcess(async (string[] parameters) =>
        {
          CoveragePrepareResult coveragePrepareResult = await TestInstrumentationHelper.Run<ClassWithAbstractRecords>(instance =>
            {
              return Task.CompletedTask;
            },
            persistPrepareResultToFile: parameters[0], skipAutoProps: bool.Parse(parameters[1]));

          return 0;
        }, [path, skipAutoProps.ToString()]);

        if (skipAutoProps)
        {
          TestInstrumentationHelper.GetCoverageResult(path)
            .GenerateReport(show: true)
            .Document("Instrumentation.Records.cs")
            .AssertNonInstrumentedLines(BuildConfiguration.Debug, 39, 39)
            .AssertNonInstrumentedLines(BuildConfiguration.Release, 39, 39)
            .AssertLinesCovered(BuildConfiguration.Debug, (41, 1), (44, 1), (45, 1), (46, 1))
            .AssertLinesCovered(BuildConfiguration.Release, (45, 1));

        }
        else
        {
          TestInstrumentationHelper.GetCoverageResult(path)
            .GenerateReport(show: true)
            .Document("Instrumentation.Records.cs")
            .AssertLinesCovered(BuildConfiguration.Debug, (39, 1), (41, 1), (44, 1), (45, 1), (46, 1))
            .AssertLinesCovered(BuildConfiguration.Release, (39, 1), (41, 1), (45, 1));
        }
      }
      finally
      {
        File.Delete(path);
      }
    }
  }
}
