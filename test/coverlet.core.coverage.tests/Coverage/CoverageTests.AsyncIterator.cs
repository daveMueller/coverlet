// Copyright (c) Toni Solarin-Sodara
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
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
          CoveragePrepareResult coveragePrepareResult = await TestInstrumentationHelper.Run<Issue1836>(async instance =>
                  {
                    await foreach (int item in (IAsyncEnumerable<int>)instance.FunctionThatReturnsIAsyncEnumerable<int>()) { }

                    using var cts = new CancellationTokenSource();
                    cts.Cancel();
                    try
                    {
                      await foreach (int item in (IAsyncEnumerable<int>)instance.FunctionThatReturnsIAsyncEnumerable<int>(cts.Token)) { }
                    }
                    catch (OperationCanceledException) { }
                  }, persistPrepareResultToFile: pathSerialize[0]);
          return 0;
        }, [path]);

        Core.Instrumentation.Document document = TestInstrumentationHelper.GetCoverageResult(path).Document("Instrumentation.AsyncIterator.cs");
        document.AssertLinesCoveredFromTo(BuildConfiguration.Debug, 38, 44);
        document.AssertBranchesCovered(BuildConfiguration.Debug, (40, 2, 1), (40, 3, 3), (43, 0, 1), (43, 1, 2));
        document.ExpectedTotalNumberOfBranches(BuildConfiguration.Debug, 2);
        document.AssertLinesCovered(BuildConfiguration.Release, 39, 40, 42, 43);
        document.AssertBranchesCovered(BuildConfiguration.Release, (40, 2, 1), (40, 3, 3), (43, 0, 1), (43, 1, 2));
        document.ExpectedTotalNumberOfBranches(BuildConfiguration.Release, 2);
      }
      finally
      {
        File.Delete(path);
      }
    }
  }
}
