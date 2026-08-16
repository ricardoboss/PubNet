using System.Formats.Tar;
using System.IO.Compression;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using PubNet.API.Controllers;
using PubNet.API.DTO.Packages.Errors;
using PubNet.API.Services;
using PubNet.Common.Interfaces;
using PubNet.Common.Models;
using PubNet.Database.Models;

namespace PubNet.API.Tests;

public class StorageControllerTests
{
	[Test]
	public async Task TestRejectsInvalidPackageNames()
	{
		using var env = new TestEnvironment();
		var packageStorageProviderMock = new Mock<IPackageStorageProvider>();
		packageStorageProviderMock
			.Setup(p => p.StoreArchiveAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IFileEntry>(),
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(Sha256Hash.From(new('0', 64)));

		var dataProtectorMock = new Mock<IDataProtector>();
		dataProtectorMock.Setup(p => p.Protect(It.IsAny<byte[]>())).Returns((byte[] data) => data);
		dataProtectorMock.Setup(p => p.Unprotect(It.IsAny<byte[]>())).Returns((byte[] data) => data);

		var dataProtectionProviderMock = new Mock<IDataProtectionProvider>();
		dataProtectionProviderMock.Setup(p => p.CreateProtector(nameof(EndpointHelper)))
			.Returns(dataProtectorMock.Object);

		var endpointHelper = new EndpointHelper(dataProtectionProviderMock.Object);
		var archivePath = CreatePackageArchive("../../tmp/pubnet-cve-candidate");

		try
		{
			var uploader = await env.AddAuthorAsync("uploader", Role.Default);

			var pending = new PendingArchive
			{
				Uuid = Guid.CreateVersion7(),
				ArchivePath = archivePath,
				Uploader = uploader,
				UploadedAtUtc = DateTimeOffset.UtcNow,
			};

			env.Db.PendingArchives.Add(pending);
			await env.Db.SaveChangesAsync();

			var finalizeUrl = endpointHelper.SignEndpoint(
				$"https://localhost/storage/finalize?pendingId={pending.Uuid:D}");

			var httpContext = new DefaultHttpContext
			{
				Request =
				{
					Scheme = "https",
					Host = new("localhost"),
					QueryString = new(new Uri(finalizeUrl).Query),
				},
			};

			var controller = new StorageController(NullLogger<StorageController>.Instance, env.Db,
				packageStorageProviderMock.Object, endpointHelper)
			{
				ControllerContext = new()
				{
					HttpContext = httpContext,
				},
			};

			var result = await controller.FinalizeUpload(pending.Uuid.ToString("D"));

			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.InstanceOf<ObjectResult>());
				Assert.That((result as ObjectResult)?.StatusCode,
					Is.EqualTo(PubNetStatusCodes.Status472InvalidPubSpec));
				Assert.That((result as ObjectResult)?.Value, Is.InstanceOf<InvalidPubSpecErrorDto>());
				Assert.That(await env.Db.Packages.AnyAsync(), Is.False);
				packageStorageProviderMock.Verify(
					p => p.StoreArchiveAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IFileEntry>(),
						It.IsAny<CancellationToken>()), Times.Never);
			}
		}
		finally
		{
			if (File.Exists(archivePath))
				File.Delete(archivePath);
		}
	}

	private static string CreatePackageArchive(string packageName)
	{
		var archivePath = Path.Combine(Path.GetTempPath(), $"{Guid.CreateVersion7():N}.tar.gz");
		using var archiveStream = File.Create(archivePath);
		using var gzipStream = new GZipStream(archiveStream, CompressionLevel.SmallestSize);
		using var tarWriter = new TarWriter(gzipStream);
		using var pubSpecStream = new MemoryStream(Encoding.UTF8.GetBytes($"""
			name: {packageName}
			version: 1.0.0
			"""));

		var pubSpecEntry = new PaxTarEntry(TarEntryType.RegularFile, "pubspec.yaml")
		{
			DataStream = pubSpecStream,
		};

		tarWriter.WriteEntry(pubSpecEntry);

		return archivePath;
	}
}
