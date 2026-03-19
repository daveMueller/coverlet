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
using Tmds.Utils;
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
                    // Normal iteration covering all items
                    await foreach (int item in (IAsyncEnumerable<int>)instance.GetNumbersAsync()) { }

                    // Cancelled iteration covering the throw branch of the ternary
                    var cts = new CancellationTokenSource();
                    cts.Cancel();
                    try
                    {
                      await foreach (int item in (IAsyncEnumerable<int>)instance.GetNumbersAsync(cts.Token)) { }
                    }
                    catch (OperationCanceledException) { }
                  }, persistPrepareResultToFile: pathSerialize[0]);
          return 0;
        }, [path]);

        Core.Instrumentation.Document document = TestInstrumentationHelper.GetCoverageResult(path).Document("Instrumentation.AsyncIterator.cs");
        document.AssertLinesCoveredFromTo(BuildConfiguration.Debug, 39, 43);
        document.AssertBranchesCovered(BuildConfiguration.Debug,
                                       // foreach loop branches (ordinals start at 2 due to [EnumeratorCancellation] state machine code)
                                       (40, 2, 1), (40, 3, 3),
                                       // ternary conditional branches: false=throw (cancelled), true=return item (normal)
                                       (43, 0, 1), (43, 1, 2));
        document.ExpectedTotalNumberOfBranches(BuildConfiguration.Debug, 2);
      }
      finally
      {
        File.Delete(path);
      }
    }
  }
}
