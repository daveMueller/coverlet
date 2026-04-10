// Copyright (c) Toni Solarin-Sodara
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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
    [Fact]
    public void AsyncIterator()
    {
      string path = Path.GetTempFileName();
      try
      {
        FunctionExecutor.Run(async (string[] pathSerialize) =>
        {
          CoveragePrepareResult coveragePrepareResult = await TestInstrumentationHelper.Run<AsyncIterator>(async instance =>
                  {
                    int res = await (Task<int>)instance.Issue1104_Repro();

                  }, persistPrepareResultToFile: pathSerialize[0]);
          return 0;
        }, [path]);

        TestInstrumentationHelper.GetCoverageResult(path)
        .Document("Instrumentation.AsyncIterator.cs")
        .AssertLinesCovered(BuildConfiguration.Debug,
                            // Issue1104_Repro()
                            (14, 1), (15, 1), (17, 203), (18, 100), (19, 100), (20, 100), (22, 1), (23, 1),
                            // CreateSequenceAsync()
                            (26, 1), (27, 202), (28, 100), (29, 100), (30, 100), (31, 100), (32, 1)
                            )
        .AssertBranchesCovered(BuildConfiguration.Debug,
                               // Issue1104_Repro(),
                               (17, 0, 1), (17, 1, 100),
                               // CreateSequenceAsync()
                               (27, 0, 1), (27, 1, 100)
                               )
        .ExpectedTotalNumberOfBranches(BuildConfiguration.Debug, 2);
      }
      finally
      {
        File.Delete(path);
      }
    }

    [Fact]
    public void AsyncIterator_Issue1836()
    {
      string path = Path.GetTempFileName();
      try
      {
        FunctionExecutor.Run(async (string[] pathSerialize) =>
        {
          CoveragePrepareResult coveragePrepareResult = await TestInstrumentationHelper.Run<AsyncIteratorIssue1836>(async instance =>
                  {
                    await foreach (int item in (IAsyncEnumerable<int>)instance.Issue1836_GenericFunctionWithConcellationThatReturnsIAsyncEnumerable<int>()) { }

                    using var cts = new CancellationTokenSource();
                    cts.Cancel();
                    try
                    {
                      await foreach (int item in (IAsyncEnumerable<int>)instance.Issue1836_GenericFunctionWithConcellationThatReturnsIAsyncEnumerable<int>(cts.Token)) { }
                    }
                    catch (OperationCanceledException) { }
                  }, persistPrepareResultToFile: pathSerialize[0]);
          return 0;
        }, [path]);

        Core.Instrumentation.Document document = TestInstrumentationHelper.GetCoverageResult(path).Document("Instrumentation.AsyncIterator.Issue1836.cs");
        document.AssertLinesCoveredFromTo(BuildConfiguration.Debug, 16, 23);
        document.AssertBranchesCovered(BuildConfiguration.Debug, (18, 2, 1), (18, 3, 3), (21, 0, 1), (21, 1, 2));
        document.ExpectedTotalNumberOfBranches(BuildConfiguration.Debug, 2);
        document.AssertLinesCovered(BuildConfiguration.Release, 17, 18, 20, 21);
        document.AssertBranchesCovered(BuildConfiguration.Release, (18, 2, 1), (18, 3, 3), (21, 0, 1), (21, 1, 2));
        document.ExpectedTotalNumberOfBranches(BuildConfiguration.Release, 2);
      }
      finally
      {
        File.Delete(path);
      }
    }

    [Fact]
    public void AsyncIterator_Issue1335()
    {
      string path = Path.GetTempFileName();
      try
      {
        FunctionExecutor.Run(async (string[] pathSerialize) =>
        {
          CoveragePrepareResult coveragePrepareResult = await TestInstrumentationHelper.Run<AsyncIteratorIssue1335>(async instance =>
          {
            var enumerable = AsyncEnumerable.Range(1, 95);

            IAsyncEnumerable<IAsyncEnumerable<int>> batches = instance.ExecuteReproduction(enumerable, 10);
            await batches.Select(batch => batch.ToArrayAsync()).ToArrayAsync();

          }, persistPrepareResultToFile: pathSerialize[0]);
          return 0;
        }, [path]);

        Core.Instrumentation.Document document = TestInstrumentationHelper.GetCoverageResult(path).GenerateReport(show: true).Document("Instrumentation.AsyncIterator.Issue1335.cs");
        document.AssertLinesCoveredFromTo(BuildConfiguration.Debug, 12, 18);
        document.AssertLinesCoveredFromTo(BuildConfiguration.Debug, 21, 27);
        document.AssertBranchesCovered(BuildConfiguration.Debug, (14, 0, 1), (14, 1, 10), (23, 0, 86), (23, 1, 95), (23, 2, 10), (23, 3, 85));
        document.ExpectedTotalNumberOfBranches(BuildConfiguration.Release, 2);
        document.AssertLinesCovered(BuildConfiguration.Release, 13, 14, 16, 22, 23, 25);
        document.AssertBranchesCovered(BuildConfiguration.Release, (14, 0, 1), (14, 1, 10), (23, 0, 86), (23, 1, 95), (23, 2, 10), (23, 3, 85));
        document.ExpectedTotalNumberOfBranches(BuildConfiguration.Release, 2);
      }
      finally
      {
        File.Delete(path);
      }
    }
  }
}
