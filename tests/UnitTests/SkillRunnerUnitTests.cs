namespace PhotoCli.Tests.UnitTests;

public class SkillRunnerUnitTests
{
	private const string CurrentDirectory = "/project";
	private static readonly string LocalSkillDirectory = Path.Combine(CurrentDirectory, ".claude", "skills", "photocli");

	[Fact]
	public async Task Execute_Should_Write_Bundled_Skill_Files_Into_Current_Directory()
	{
		var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>(), CurrentDirectory);
		var runner = new SkillRunner(new SkillOptions(), fileSystem, Mock.Of<IConsoleWriter>());

		var exitCode = await runner.Execute();

		ShouldHaveWrittenSkillFiles(exitCode, fileSystem, LocalSkillDirectory);
	}

	[Fact]
	public async Task Execute_With_Global_Should_Write_Bundled_Skill_Files_Into_User_Claude_Directory()
	{
		var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>(), CurrentDirectory);
		var runner = new SkillRunner(new SkillOptions(true), fileSystem, Mock.Of<IConsoleWriter>());

		var exitCode = await runner.Execute();

		var claudeDirectory = Environment.GetEnvironmentVariable("CLAUDE_CONFIG_DIR") ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".claude");
		ShouldHaveWrittenSkillFiles(exitCode, fileSystem, Path.Combine(claudeDirectory, "skills", "photocli"));
		fileSystem.Directory.Exists(LocalSkillDirectory).Should().BeFalse();
	}

	[Fact]
	public async Task Execute_Should_Overwrite_Existing_Skill_File()
	{
		var skillMarkdownPath = Path.Combine(LocalSkillDirectory, "SKILL.md");
		var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
		{
			{ skillMarkdownPath, new MockFileData("outdated skill content which is longer than nothing") }
		}, CurrentDirectory);
		var runner = new SkillRunner(new SkillOptions(), fileSystem, Mock.Of<IConsoleWriter>());

		var exitCode = await runner.Execute();

		exitCode.Should().Be(ExitCode.Success);
		fileSystem.File.ReadAllText(skillMarkdownPath).Should().StartWith("---").And.NotContain("outdated");
	}

	[Fact]
	public async Task Execute_Should_Write_Success_Message_With_Skill_Directory()
	{
		var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>(), CurrentDirectory);
		var consoleWriterMock = new Mock<IConsoleWriter>();
		var runner = new SkillRunner(new SkillOptions(), fileSystem, consoleWriterMock.Object);

		await runner.Execute();

		consoleWriterMock.Verify(v => v.WriteSuccess(It.Is<string>(s => s.Contains(LocalSkillDirectory))));
	}

	private static void ShouldHaveWrittenSkillFiles(ExitCode exitCode, MockFileSystem fileSystem, string skillDirectory)
	{
		var skillMarkdownPath = Path.Combine(skillDirectory, "SKILL.md");
		using (new AssertionScope())
		{
			exitCode.Should().Be(ExitCode.Success);
			fileSystem.File.Exists(skillMarkdownPath).Should().BeTrue();
			fileSystem.File.Exists(Path.Combine(skillDirectory, "references", "options.md")).Should().BeTrue();
			fileSystem.File.ReadAllText(skillMarkdownPath).Should().StartWith("---").And.Contain("name: photocli");
		}
	}
}
