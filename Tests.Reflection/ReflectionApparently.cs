namespace Tests.Reflection;

public class ReflectionApparently
{
	[Test]
	public void Can_Find_Running_Process_Name()
	{
		Assert.That(
			System.Diagnostics.Process.GetCurrentProcess().ProcessName.ToLower(),
			Is.EqualTo("testhost"));
	}

	[Test]
	public void Can_Find_All_Loaded_Assemblies()
	=> Assert.That(
		LoadedAssemblies,
		Has.One.Matches<System.Reflection.Assembly>(a => a.FullName!.StartsWith("Tests.Reflection")));

	[Test]
	public void Can_Find_All_Types_Defined_In_LoadedAssembly()
	=> Assert.That(
		LoadedTypes,
		Has.One.Matches<System.Type>(t => t == GetType()).And.One.Matches<System.Type>(t => t == typeof(String)));

	[Test]
	public void Can_Find_All_Types_Defined_In_CurrentOrganizationNamespace()
	=> Assert.That(
		LoadedTypesHavingNamespaceStartingWith(string.Join('.', GetType().Namespace!.Split('.').Take(2))),
		Has.One.Matches<System.Type>(t => t == GetType()).And.None.Matches<System.Type>(t => t == typeof(String)));

	public static IReadOnlyCollection<System.Reflection.Assembly> LoadedAssemblies
	{
		get
		{
			var loadedAssemblies = new List<System.Reflection.Assembly>();
			loadedAssemblies.Add(System.Reflection.Assembly.GetExecutingAssembly());
			loadedAssemblies.AddRange(
				AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(loadedAssembly => loadedAssembly.GetReferencedAssemblies())
					.Distinct()
					.Select(referencedAssemblyName => System.Reflection.Assembly.Load(referencedAssemblyName)));
			return loadedAssemblies;
		}
	}

	public static IReadOnlyCollection<System.Type> LoadedTypes
	{
		get
		{
			var allTypesLoaded = LoadedAssemblies
				.SelectMany(assembly =>
					assembly
						.GetTypes()
						.SelectMany(t => new [] { t }.Concat(t.GetNestedTypes())))
				.Distinct()
				.ToArray();
			return allTypesLoaded;
		}
	}

	public static IReadOnlyCollection<System.Type> LoadedTypesHavingNamespaceStartingWith(string prefix)
	=> LoadedTypes
		.Where(t => t?.Namespace is not null && t.Namespace!.StartsWith(prefix))
		.ToArray();
}
