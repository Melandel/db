
namespace Tests.FileSystem;

public class Tests
{
	[Test]
	public void GetsDirectoryName_Does_NOT_End_With_Separator()
	{
		Assert.That(
			Path.GetDirectoryName(@"C:\foo\bar\baz.html"),
			Is.EqualTo(@"C:\foo\bar"));
	}

	[Test]
	public void GetFileNameWithoutExtension_Does_NOT_Have_Separator()
	{
		Assert.That(
			Path.GetFileNameWithoutExtension(@"C:\foo\bar\baz.html"),
			Is.EqualTo(@"baz"));
	}

	[Test]
	public void GetFileNameWithoutExtension_Goes_Until_Last_Dot()
	{
		Assert.That(
			Path.GetFileNameWithoutExtension(@"C:\foo\bar\baz.svg.html"),
			Is.EqualTo(@"baz.svg"));
	}

	[Test]
	public void GetExtension_StartsWith_Dot_Character()
	{
		Assert.That(
			Path.GetExtension(@"C:\foo\bar\baz.html"),
			Is.EqualTo(@".html"));
	}

	[Test]
	public void GetExtension_Keeps_Only_Text_After_LAST_Dot()
	{
		Assert.That(
			Path.GetExtension(@"C:\foo\bar\baz.svg.html"),
			Is.EqualTo(@".html"));
	}

	[Test]
	public void Join_Adds_Separators()
	{
		Assert.That(
			Path.Join(@"C:\foo\bar", "baz.svg", ".html"),
			Is.EqualTo(@"C:\foo\bar\baz.svg\.html"));
	}
}
