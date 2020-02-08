// Copyright 2019 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FluentAssertions;
using Nuke.Common.Git;
using Xunit;

namespace Nuke.Common.Tests
{
    public class GitRepositoryTest
    {
        [Theory]
        [InlineData("https://github.com/nuke-build", "github.com", "nuke-build")]
        [InlineData("https://github.com/nuke-build/", "github.com", "nuke-build")]
        [InlineData("https://github.com/nuke-build/nuke", "github.com", "nuke-build/nuke")]
        [InlineData("https://github.com/nuke-build/nuke.git", "github.com", "nuke-build/nuke")]
        [InlineData("https://user:pass@github.com/nuke-build/nuke.git", "github.com", "nuke-build/nuke")]
        [InlineData(" https://github.com/TdMxm/nuke.git", "github.com", "TdMxm/nuke")]
        [InlineData("git@git.test.org:test", "git.test.org", "test")]
        [InlineData("git@git.test.org/test", "git.test.org", "test")]
        [InlineData("git@git.test.org/test/", "git.test.org", "test")]
        [InlineData("git@git.test.org/test.git", "git.test.org", "test")]
        [InlineData("ssh://git@git.test.org/test.git", "git.test.org", "test")]
        [InlineData("ssh://git@git.test.org:1234/test.git", "git.test.org", "test")]
        [InlineData("ssh://git.test.org/test/test", "git.test.org", "test/test")]
        [InlineData("ssh://git.test.org:1234/test/test", "git.test.org", "test/test")]
        [InlineData("https://git.test.org:1234/test/test", "git.test.org", "test/test")]
        [InlineData("git://git.test.org:1234/test/test", "git.test.org", "test/test")]
        [InlineData("git://git.test.org/test/test", "git.test.org", "test/test")]
        public void FromUrlTest(string url, string endpoint, string identifier)
        {
            var repository = GitRepository.FromUrl(url);
            repository.Endpoint.Should().Be(endpoint);
            repository.Identifier.Should().Be(identifier);
        }

        [Fact]
        public void FromDirectoryTest()
        {
            var repository = GitRepository.FromLocalDirectory(Directory.GetCurrentDirectory()).NotNull();
            repository.Endpoint.Should().NotBeNullOrEmpty();
            repository.Identifier.Should().NotBeNullOrEmpty();
            repository.LocalDirectory.Should().NotBeNullOrEmpty();
            repository.Head.Should().NotBeNullOrEmpty();
        }
        [Fact]
        public void FromWorkTreeTest()
        {
            var branchName = "_worktree";
            var worktreePath = Path.Combine(Directory.GetCurrentDirectory(), branchName);

            if (Directory.Exists(worktreePath))
                Directory.Delete(worktreePath, true);
            Process.Start("git.exe", "worktree prune").WaitForExit();
            Process.Start("git.exe", $"branch -D {branchName}").WaitForExit();

            Process.Start("git.exe", $"worktree add {worktreePath}").WaitForExit();
            try
            {
                var repository = GitRepository.FromLocalDirectory(worktreePath).NotNull();
                repository.Endpoint.Should().NotBeNullOrEmpty();
                repository.Identifier.Should().NotBeNullOrEmpty();
                repository.LocalDirectory.Should().NotBeNullOrEmpty();
                repository.Head.Should().NotBeNullOrEmpty();
                repository.Branch.Should().Be(branchName);
            }
            finally
            {
                if (Directory.Exists(worktreePath))
                    Directory.Delete(worktreePath, true);
                Process.Start("git.exe", "worktree prune").WaitForExit();
                Process.Start("git.exe", $"branch -D {branchName}").WaitForExit();
            }
        }
    }
}
