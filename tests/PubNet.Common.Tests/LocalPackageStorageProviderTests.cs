using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using PubNet.Common.Models;
using PubNet.Common.Services;

namespace PubNet.Common.Tests;

public class LocalPackageStorageProviderTests
{
	private const string existingPackageName = "test_package";
	private const string existingPackageVersion = "1.5.0";
	private DirectoryInfo workingDir = null!;
	private DirectoryInfo existingPackageDir = null!;
	private DirectoryInfo existingVersionDir = null!;
	private DirectoryInfo existingDocsDir = null!;
	private FileInfo existingArchiveFile = null!;

	private void AssertPackageDirExists()
	{
		existingPackageDir.Refresh();

		Assert.That(existingPackageDir.Exists, Is.True, $"Expected directory {existingPackageDir} to exist, but it doesn't");
	}

	private void AssertPackageDirDoesntExist()
	{
		existingPackageDir.Refresh();

		Assert.That(existingPackageDir.Exists, Is.False, $"Expected directory {existingPackageDir} to be deleted, but it wasn't");
	}

	private void AssertVersionDirExists()
	{
		existingVersionDir.Refresh();

		Assert.That(existingVersionDir.Exists, Is.True, $"Expected directory {existingVersionDir} to exist, but it doesn't");
	}

	private void AssertVersionDirDoesntExist()
	{
		existingVersionDir.Refresh();

		Assert.That(existingVersionDir.Exists, Is.False, $"Expected directory {existingVersionDir} to be deleted, but it wasn't");
	}

	private void AssertDocsDirExists()
	{
		existingDocsDir.Refresh();

		Assert.That(existingDocsDir.Exists, Is.True, $"Expected directory {existingDocsDir} to exist, but it doesn't");
	}

	private void AssertDocsDirDoesntExist()
	{
		existingDocsDir.Refresh();

		Assert.That(existingDocsDir.Exists, Is.False, $"Expected directory {existingDocsDir} to be deleted, but it wasn't");
	}

	private void AssertVersionArchiveExists()
	{
		existingArchiveFile.Refresh();

		Assert.That(existingArchiveFile.Exists, Is.True, $"Expected file {existingArchiveFile} to exist, but it doesn't");
	}

	private void AssertVersionArchiveDoesntExist()
	{
		existingArchiveFile.Refresh();

		Assert.That(existingArchiveFile.Exists, Is.False, $"Expected file {existingArchiveFile} to be deleted, but it wasn't");
	}

	[OneTimeTearDown]
	public void CleanupWorkingDir()
	{
		if (workingDir.Exists)
			workingDir.Delete(recursive: true);
	}

	[OneTimeSetUp]
	public void SetupWorkingDir()
	{
		var tempDirPath = Path.GetTempPath();
		var workingDirPath = Path.Combine(tempDirPath, nameof(LocalPackageStorageProviderTests));

		workingDir = new(workingDirPath);
		if (workingDir.Exists)
			workingDir.Delete(recursive: true);

		workingDir.Create();
	}

	[SetUp]
	public void SetupPackage()
	{
		var packagePath = Path.Combine(workingDir.FullName, existingPackageName);
		existingPackageDir = new DirectoryInfo(packagePath);

		var versionPath = Path.Combine(workingDir.FullName, existingPackageName, existingPackageVersion);
		existingVersionDir = new DirectoryInfo(versionPath);

		var docsPath = Path.Combine(workingDir.FullName, existingPackageName, existingPackageVersion, "docs");
		existingDocsDir = new DirectoryInfo(docsPath);

		if (existingPackageDir.Exists)
			existingPackageDir.Delete(recursive: true);

		existingDocsDir.Create(); // creating most-nested sub dir creates all parent dirs

		var archivePath = Path.Combine(existingVersionDir.FullName, "archive.tar.gz");
		existingArchiveFile = new FileInfo(archivePath);
		existingArchiveFile.Open(FileMode.CreateNew).Close();
	}

	private LocalPackageStorageProvider EmitInstance(PackageStorageProviderOptions? options = null)
	{
		options ??= new()
		{
			Path = workingDir.FullName,
		};

		var optionsMock = new Mock<IOptions<PackageStorageProviderOptions>>();

		optionsMock
			.SetupGet(o => o.Value)
			.Returns(options);

		return new(
			NullLogger<LocalPackageStorageProvider>.Instance,
			optionsMock.Object
		);
	}

	[Test]
	public async Task TestDeletesPackageDir()
	{
		var instance = EmitInstance();

		AssertPackageDirExists();

		await instance.DeletePackageAsync(existingPackageName);

		AssertPackageDirDoesntExist();
	}

	[Test]
	public async Task TestDeletesPackageVersionDir()
	{
		var instance = EmitInstance();

		AssertDocsDirExists();
		AssertVersionArchiveExists();
		AssertVersionDirExists();
		AssertPackageDirExists();

		await instance.DeletePackageVersionAsync(existingPackageName, existingPackageVersion);

		AssertDocsDirDoesntExist();
		AssertVersionArchiveDoesntExist();
		AssertVersionDirDoesntExist();
		AssertPackageDirExists();
	}

	[Test]
	public void TestFallsBackToAppDataPath()
	{
		var expectedPath = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			"PubNet",
			"packages",
			existingPackageName,
			existingPackageVersion,
			"archive.tar.gz"
		);

		var instance = EmitInstance(new()
		{
			Path = null,
		});

		var exception = Assert.ThrowsAsync<FileNotFoundException>(async () =>
			await instance.GetArchiveAsync(existingPackageName, existingPackageVersion));

		Assert.That(exception.FileName, Is.EqualTo(expectedPath));
	}

	[Test]
	public async Task TestProvidesArchiveFile()
	{
		var instance = EmitInstance();

		var archiveFile = await instance.GetArchiveAsync(existingPackageName, existingPackageVersion);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(archiveFile, Is.TypeOf<FilesystemFileEntry>());
			Assert.That(archiveFile.Name, Is.EqualTo("archive.tar.gz"));
		}

		var entry = (archiveFile as FilesystemFileEntry)!;
		Assert.That(entry.Info.FullName, Is.EqualTo(existingArchiveFile.FullName));
	}

	[Test]
	public async Task TestProvidesDocs()
	{
		var instance = EmitInstance();

		var docsContainer = await instance.GetDocsAsync(existingPackageName, existingPackageVersion);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(docsContainer, Is.TypeOf<DirectoryFileContainer>());
			Assert.That(docsContainer.Name, Is.EqualTo("docs"));
		}

		var entry = (docsContainer as DirectoryFileContainer)!;
		Assert.That(entry.Info.FullName, Is.EqualTo(existingDocsDir.FullName));
	}

	// TODO: add tests for storing docs
	// TODO: add tests for storing version archive
}
