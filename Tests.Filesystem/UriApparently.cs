namespace Tests.Filesystem;

class UriApparently
{
	[Test]
	public void Has_AbsolutePath_With_Slashes_Instead_Of_Backslashes()
	{
		Assert.That(
			new Uri(@"C:\foo\bar\baz.svg.html").AbsolutePath,
			Is.EqualTo(@"C:/foo/bar/baz.svg.html"));
	}

	[Test]
	public void Has_AbsoluteUri_StartingWith_FileProtocol()
	{
		Assert.That(
			new Uri(@"C:\foo\bar\baz.svg.html").AbsoluteUri,
			Is.EqualTo(@"file:///C:/foo/bar/baz.svg.html"));
	}
}
