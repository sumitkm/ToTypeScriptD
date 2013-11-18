﻿using ApprovalTests;
using System.IO;
using Xunit;

namespace ToTypeScriptD.Tests.ExeTests
{
    public class CommandLineArgTests : ExeTestBase
    {
        [Fact]
        public void ExeShouldGenerateHelpOnEmptyInput()
        {
            var result = Execute("").StdOut.StripVersionFromOutput();
            Approvals.Verify(result);
        }

        [Fact]
        public void ExeShouldGenerateHelpWithHelpArgs()
        {
            var result = Execute("--help").StdOut.StripVersionFromOutput();
            Approvals.Verify(result);
        }

        [Fact]
        public void ExeShouldGenerateHelpForDotNet()
        {
            var result = Execute("dotnet").StdOut.StripVersionFromOutput();
            Approvals.Verify(result);
        }

        [Fact]
        public void ExeShouldGenerateHelpForWinMD()
        {
            var result = Execute("winmd").StdOut.StripVersionFromOutput();
            Approvals.Verify(result);
        }

        [Fact]
        public void ExeShouldGenerateOutputForMultipleWinmdFiles()
        {
            var result = Execute(@"winmd C:\Windows\System32\WinMetadata\Windows.Foundation.winmd C:\Windows\System32\WinMetadata\Windows.Networking.winmd");

            (result.StdOut.Length > 100).ShouldBeTrue(result.StdOut.Length + " should be greater than 100");
        }

        [Fact]
        public void VerifyExeGeneratedOutputForMultipleWinmdFiles()
        {
            var result = Execute(@"winmd C:\Windows\System32\WinMetadata\Windows.Foundation.winmd C:\Windows\System32\WinMetadata\Windows.System.winmd");

            result.StdOut.Verify();
        }

        [Fact]
        [ApprovalTests.Reporters.UseReporter(typeof(ApprovalTests.Reporters.P4MergeReporter))]
        public void ExeDuplicateAssemblyShouldStillOnlyGenerateOne()
        {
            var resultDup = Execute(@"winmd C:\Windows\System32\WinMetadata\Windows.Foundation.winmd C:\Windows\System32\WinMetadata\Windows.Foundation.winmd");
            var resultNonDup = Execute(@"winmd C:\Windows\System32\WinMetadata\Windows.Foundation.winmd");

            resultNonDup.ToString().StripHeaderGarbageromOutput()
                .DiffWith(resultDup.ToString().StripHeaderGarbageromOutput());
        }

        [Fact]
        public void ExeShouldBeAbleToGenerateSpecialTypes()
        {
            var resultDup = Execute("winmd --specialTypes");

            resultDup.Verify();
        }


        [Fact]
        public void ExeShouldGenerateWith2SpaceIndentation()
        {
            var resultDup = Execute("winmd --specialTypes ToTypeScriptD.Native.winmd --indentWith SpaceX2");

            resultDup.Verify();
        }

        [Fact]
        public void ExeShouldGiveHelpfulErrorWhenFilesNotFoundInUnknownDirectory()
        {
            var resultDup = Execute(@"winmd C:\TypeScriptD\TypeScriptD\TypeScriptD\Foo.dll C:\TypeScriptD\TypeScriptD\TypeScriptD\Foo.dll");

            resultDup.Verify();
        }

        [Fact]
        public void ExeShouldGiveHelpfulErrorWhenFilesNotFound()
        {
            var resultDup = Execute(@"winmd C:\TypeScriptD_FileNotFound_ThisShouldNotExistOnYourSystem.dll");

            resultDup.Verify();
        }

        [Fact]
        public void ExeShouldApplyRegexFilterOnTypes()
        {
            var resultDup = Execute(@"DotNet ..\..\..\bin\ToTypeScriptD.TestAssembly.CSharp.dll --regexFilter 'ToTypeScriptD.TestAssembly.CSharp.NamespaceSample'");

            resultDup.Verify();
        }

    }
}
