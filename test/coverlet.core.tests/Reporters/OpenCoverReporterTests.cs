// Copyright (c) Toni Solarin-Sodara
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using Coverlet.Core.Abstractions;
using Coverlet.Core.Reporters;
using Moq;
using Xunit;

namespace Coverlet.Core.Tests.Reporters
{
  public class OpenCoverReporterTests
  {
    [Fact]
    public void TestReport()
    {
      var result = new CoverageResult
      {
        Parameters = new CoverageParameters(),
        Identifier = Guid.NewGuid().ToString(),
        Modules = []
      };
      result.Modules.Add("Coverlet.Core.Tests.Reporters", CreateFirstDocuments());

      var reporter = new OpenCoverReporter();
      string report = reporter.Report(result, new Mock<ISourceRootTranslator>().Object);
      Assert.NotEmpty(report);
      var doc = XDocument.Load(new StringReader(report));
      Assert.DoesNotContain(doc.Descendants().Attributes("sequenceCoverage"), v => v.Value != "33.33");
      Assert.DoesNotContain(doc.Descendants().Attributes("branchCoverage"), v => v.Value != "25");
      Assert.DoesNotContain(doc.Descendants().Attributes("nPathComplexity"), v => v.Value != "4");
    }

    [Fact]
    public void TestFilesHaveUniqueIdsOverMultipleModules()
    {
      var result = new CoverageResult
      {
        Parameters = new CoverageParameters(),
        Identifier = Guid.NewGuid().ToString(),
        Modules = []
      };
      result.Modules.Add("Coverlet.Core.Tests.Reporters", CreateFirstDocuments());
      result.Modules.Add("Some.Other.Module", CreateSecondDocuments());

      var reporter = new OpenCoverReporter();
      string xml = reporter.Report(result, new Mock<ISourceRootTranslator>().Object);
      Assert.NotEqual(string.Empty, xml);

      Assert.Contains(@"<FileRef uid=""1"" />", xml);
      Assert.Contains(@"<FileRef uid=""2"" />", xml);
    }

    [Fact]
    public void TestLineBranchCoverage()
    {
      var result = new CoverageResult
      {
        Identifier = Guid.NewGuid().ToString(),
        Modules = new Modules { { "Coverlet.Core.Tests.Reporters", CreateBranchCoverageDocuments() } },
        Parameters = new CoverageParameters()
      };

      string xml = new OpenCoverReporter().Report(result, new Mock<ISourceRootTranslator>().Object);

      // Line 1: Two branches, no coverage (bec = 2, bev = 0)
      Assert.Contains(@"<SequencePoint vc=""1"" uspid=""1"" ordinal=""0"" sl=""1"" sc=""1"" el=""1"" ec=""2"" bec=""2"" bev=""0"" fileid=""1"" />", xml);

      // Line 2: Two branches, one covered (bec = 2, bev = 1)
      Assert.Contains(@"<SequencePoint vc=""1"" uspid=""2"" ordinal=""1"" sl=""2"" sc=""1"" el=""2"" ec=""2"" bec=""2"" bev=""1"" fileid=""1"" />", xml);

      // Line 3: Two branches, all covered (bec = 2, bev = 2)
      Assert.Contains(@"<SequencePoint vc=""1"" uspid=""3"" ordinal=""2"" sl=""3"" sc=""1"" el=""3"" ec=""2"" bec=""2"" bev=""2"" fileid=""1"" />", xml);

      // Line 4: Three branches, two covered (bec = 3, bev = 2)
      Assert.Contains(@"<SequencePoint vc=""1"" uspid=""4"" ordinal=""3"" sl=""4"" sc=""1"" el=""4"" ec=""2"" bec=""3"" bev=""2"" fileid=""1"" />", xml);
    }

    [Fact]
    public void OpenCoverTestReportDoesNotContainBom()
    {
      var result = new CoverageResult
      {
        Identifier = Guid.NewGuid().ToString(),
        Modules = new Modules { { "Coverlet.Core.Tests.Reporters", CreateBranchCoverageDocuments() } },
        Parameters = new CoverageParameters()
      };

      string report = new OpenCoverReporter().Report(result, new Mock<ISourceRootTranslator>().Object);

      byte[] preamble = Encoding.UTF8.GetBytes(report)[..3];
      Assert.NotEqual(Encoding.UTF8.GetPreamble(), preamble);
    }

    private static Documents CreateFirstDocuments()
    {
      var lines = new Lines
      {
        { 1, 1 },
        { 2, 0 },
        { 3, 0 }
      };

      var branches = new Branches
      {
        new BranchInfo { Line = 1, Hits = 1, Offset = 23, EndOffset = 24, Path = 0, Ordinal = 1 },
        new BranchInfo { Line = 1, Hits = 0, Offset = 23, EndOffset = 27, Path = 1, Ordinal = 2 },
        new BranchInfo { Line = 1, Hits = 0, Offset = 40, EndOffset = 41, Path = 0, Ordinal = 3 },
        new BranchInfo { Line = 1, Hits = 0, Offset = 40, EndOffset = 44, Path = 1, Ordinal = 4 }
      };

      var methods = new Methods();
      string methodString = "System.Void Coverlet.Core.Tests.Reporters.OpenCoverReporterTests.TestReport()";
      methods.Add(methodString, new Method());
      methods[methodString].Lines = lines;
      methods[methodString].Branches = branches;

      var classes = new Classes
      {
        { "Coverlet.Core.Tests.Reporters.OpenCoverReporterTests", methods }
      };

      var documents = new Documents
      {
        { "doc.cs", classes }
      };

      return documents;
    }

    private static Documents CreateSecondDocuments()
    {
      var lines = new Lines
      {
        { 1, 1 },
        { 2, 0 }
      };

      var methods = new Methods();
      string methodString = "System.Void Some.Other.Module.TestClass.TestMethod()";
      methods.Add(methodString, new Method());
      methods[methodString].Lines = lines;

      var classes2 = new Classes
      {
        { "Some.Other.Module.TestClass", methods }
      };

      var documents = new Documents
      {
        { "TestClass.cs", classes2 }
      };

      return documents;
    }

    private static Documents CreateBranchCoverageDocuments()
    {
      var lines = new Lines
            {
                {1, 1},
                {2, 1},
                {3, 1},
                {4, 1},
            };

      var branches = new Branches
            {
                // Two branches, no coverage
                new BranchInfo {Line = 1, Hits = 0, Offset = 23, EndOffset = 24, Path = 0, Ordinal = 1},
                new BranchInfo {Line = 1, Hits = 0, Offset = 23, EndOffset = 27, Path = 1, Ordinal = 2},
                
                // Two branches, one covered
                new BranchInfo {Line = 2, Hits = 1, Offset = 40, EndOffset = 41, Path = 0, Ordinal = 3},
                new BranchInfo {Line = 2, Hits = 0, Offset = 40, EndOffset = 44, Path = 1, Ordinal = 4},
                
                // Two branches, all covered
                new BranchInfo {Line = 3, Hits = 1, Offset = 40, EndOffset = 41, Path = 0, Ordinal = 3},
                new BranchInfo {Line = 3, Hits = 3, Offset = 40, EndOffset = 44, Path = 1, Ordinal = 4},
                
                // Three branches, two covered
                new BranchInfo {Line = 4, Hits = 5, Offset = 40, EndOffset = 44, Path = 1, Ordinal = 4},
                new BranchInfo {Line = 4, Hits = 2, Offset = 40, EndOffset = 44, Path = 1, Ordinal = 4},
                new BranchInfo {Line = 4, Hits = 0, Offset = 40, EndOffset = 44, Path = 1, Ordinal = 4}
            };

      const string methodString = "System.Void Coverlet.Core.Tests.Reporters.OpenCoverReporterTests.TestReport()";
      var methods = new Methods
            {
                {methodString, new Method { Lines = lines, Branches = branches}}
            };

      return new Documents
            {
                {"doc.cs", new Classes {{ "Coverlet.Core.Tests.Reporters.OpenCoverReporterTests", methods}}}
            };
    }
  }
}
