using PubNet.Common.Extensions;
using PubNet.Common.Interfaces;

namespace PubNet.Common.Tests;

public class FilesystemEntryExtensionsTests
{
	[Test]
	public void TestGetEntries()
	{
		const string entryName = "child.txt";
		var fileContainerMock = new Mock<IFileContainer>();
		var filesystemEntryMock = new Mock<IFilesystemEntry>();

		fileContainerMock
			.Setup(c => c.GetEntries())
			.Returns([filesystemEntryMock.Object])
			.Verifiable();

		filesystemEntryMock
			.SetupGet(e => e.Name)
			.Returns(entryName);

		var child = fileContainerMock.Object.GetChildEntry(entryName);

		Assert.That(child, Is.Not.Null);
		Assert.That(child, Is.SameAs(filesystemEntryMock.Object));

		fileContainerMock.VerifyAll();
		filesystemEntryMock.VerifyAll();
	}

	[Test]
	public void TestGetEntriesIsCaseInsensitive()
	{
		const string requestedName = "CHILD.txt";
		const string actualName = "child.txt";
		var fileContainerMock = new Mock<IFileContainer>();
		var filesystemEntryMock = new Mock<IFilesystemEntry>();

		fileContainerMock
			.Setup(c => c.GetEntries())
			.Returns([filesystemEntryMock.Object])
			.Verifiable();

		filesystemEntryMock
			.SetupGet(e => e.Name)
			.Returns(actualName);

		var child = fileContainerMock.Object.GetChildEntry(requestedName);

		Assert.That(child, Is.Not.Null);
		Assert.That(child, Is.SameAs(filesystemEntryMock.Object));

		fileContainerMock.VerifyAll();
		filesystemEntryMock.VerifyAll();
	}

	[Test]
	public void TestGetRelativeEntryHandlesForwardAndBackwardSlashes()
	{
		var fileContainerMock = new Mock<IFileContainer>();
		var fooContainerMock = new Mock<IFileContainer>();
		var barEntryMock = new Mock<IFilesystemEntry>();

		fileContainerMock
			.Setup(c => c.GetEntries())
			.Returns([fooContainerMock.Object])
			.Verifiable();

		fooContainerMock
			.SetupGet(c => c.Name)
			.Returns("foo")
			.Verifiable();

		fooContainerMock
			.Setup(c => c.GetEntries())
			.Returns([barEntryMock.Object])
			.Verifiable();

		barEntryMock
			.SetupGet(e => e.Name)
			.Returns("bar")
			.Verifiable();

		var entryForward = fileContainerMock.Object.GetRelativeEntry("foo/bar");
		var entryBackward = fileContainerMock.Object.GetRelativeEntry("foo\\bar");

		using (Assert.EnterMultipleScope())
		{
			Assert.That(entryForward, Is.Not.Null);
			Assert.That(entryBackward, Is.Not.Null);
		}

		using (Assert.EnterMultipleScope())
		{
			Assert.That(entryForward, Is.SameAs(barEntryMock.Object));
			Assert.That(entryBackward, Is.SameAs(barEntryMock.Object));
		}

		fileContainerMock.VerifyAll();
		fooContainerMock.VerifyAll();
		barEntryMock.VerifyAll();
	}

	[Test]
	public void TestGetRelativeEntryReturnsNullForNonExistentFile()
	{
		var fileContainerMock = new Mock<IFileContainer>();

		fileContainerMock
			.Setup(c => c.GetEntries())
			.Returns([])
			.Verifiable();

		var entry = fileContainerMock.Object.GetRelativeEntry("non-existent.txt");

		Assert.That(entry, Is.Null);

		fileContainerMock.VerifyAll();
	}

	[Test]
	public void TestGetRelativeEntryReturnsNullForNonExistentNestedFile()
	{
		const string nestedName = "nested";
		var fileContainerMock = new Mock<IFileContainer>();
		var nestedContainerMock = new Mock<IFileContainer>();

		fileContainerMock
			.Setup(c => c.GetEntries())
			.Returns([nestedContainerMock.Object])
			.Verifiable();

		nestedContainerMock
			.SetupGet(c => c.Name)
			.Returns(nestedName)
			.Verifiable();

		nestedContainerMock
			.Setup(c => c.GetEntries())
			.Returns([])
			.Verifiable();

		var entry = fileContainerMock.Object.GetRelativeEntry($"{nestedName}/non-existent.txt");

		Assert.That(entry, Is.Null);

		fileContainerMock.VerifyAll();
		nestedContainerMock.VerifyAll();
	}

	[Test]
	public void TestGetRelativeEntryReturnsNullWhenNavigatingIntoFileEntry()
	{
		const string fileName = "foo.txt";
		var fileContainerMock = new Mock<IFileContainer>();
		var fileEntryMock = new Mock<IFileEntry>();

		fileContainerMock
			.Setup(c => c.GetEntries())
			.Returns([fileEntryMock.Object])
			.Verifiable();

		fileEntryMock
			.SetupGet(e => e.Name)
			.Returns(fileName)
			.Verifiable();

		var entry = fileContainerMock.Object.GetRelativeEntry(fileName);

		Assert.That(entry, Is.SameAs(fileEntryMock.Object));

		var nonExistent = fileContainerMock.Object.GetRelativeEntry($"{fileName}/foo");

		Assert.That(nonExistent, Is.Null);

		fileContainerMock.VerifyAll();
		fileEntryMock.VerifyAll();
	}
}
