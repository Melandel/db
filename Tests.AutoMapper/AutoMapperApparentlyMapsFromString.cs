using AutoMapper;
using Tests.TestData;

namespace Tests.AutoMapper;

class AutoMapperApparentlyMapsFromString
{
	class AutoMapperProfile2: Profile
	{
		public AutoMapperProfile2()
		{
			CreateMap<ClassWithStringPropertyCalledGuid, ClassWithGuidPropertyCalledGuid>();
				//.ForMember(d => d.guid, opt => opt.MapFrom(src => src.guid));
		}
	}

	[Test]
	public void Throw_AutoMapperMappingException_When_Expected_Output_Is_Guid_And_Input_Is_Not_A_Guid()
	{
		// Arrange
		var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile2())));
		var c1 = new ClassWithStringPropertyCalledGuid();
		c1.Guid = "foo";

		// Act & Assert
		Assert.That(
			() => { var c2 = mapper.Map<ClassWithGuidPropertyCalledGuid>(c1); },
			Throws.InstanceOf<AutoMapperMappingException>()
		);
	}

	[Test]
	public void Into_EmptyGuid_When_Expected_Output_Is_Guid_And_Input_Is_Null()
	{
		// Arrange
		var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile2())));
		var c1 = new ClassWithStringPropertyCalledGuid();
		c1.Guid = null;


		// Act
		var c2 = mapper.Map<ClassWithGuidPropertyCalledGuid>(c1);

		// Act & Assert
		Assert.That(c2.Guid, Is.EqualTo(Guid.Empty));
	}

	[Test]
	public void Throw_AutoMapperMappingException_When_Expected_Output_Is_Guid_And_Input_Is_EmptyString()
	{
		// Arrange
		var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile2())));
		var c1 = new ClassWithStringPropertyCalledGuid();
		c1.Guid = "";

		// Act & Assert
		Assert.That(
			() => { var c2 = mapper.Map<ClassWithGuidPropertyCalledGuid>(c1); },
			Throws.InstanceOf<AutoMapperMappingException>()
		);
	}

	[Test]
	public void Into_Matching_Guid_When_Expected_Output_Is_Guid_And_Input_Is_ValidGuid_By_Default()
	{
		// Arrange
		var c1 = new ClassWithStringPropertyCalledGuid() { Guid = "0b749673-efe9-4019-990f-29bc24d80b1a" };
		var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile2())));

		// Act
		var c2 = mapper.Map<ClassWithGuidPropertyCalledGuid>(c1);

		// Assert
		Assert.That(c2.Guid, Is.EqualTo(Guid.Parse("0b749673-efe9-4019-990f-29bc24d80b1a")));
	}
}
