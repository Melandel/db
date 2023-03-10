using AutoMapper;
using Tests.TestData;

namespace Tests.AutoMapper;


class AutoMapperApparentlyMapsFromNullableGuid
{
	class AutoMapperProfile: Profile
	{
		public AutoMapperProfile()
		{
			CreateMap<ClassWithNullableGuidPropertyCalledGuid1, ClassWithTwoGuidPropertiesCalledGuid1AndGuid2>()
				.ForMember(d => d.Guid2, opt => opt.Ignore());
		}
	}

	[Test]
	public void Into_Matching_Guid_When_Input_Is_Valid_By_Default()
	{
		// Arrange
		var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile())));
		var c1 = new ClassWithNullableGuidPropertyCalledGuid1();
		c1.Guid1 = Guid.NewGuid();

		// Act
		var c2 = mapper.Map<ClassWithTwoGuidPropertiesCalledGuid1AndGuid2>(c1);

		// Assert
		Assert.That(c2.Guid2, Is.EqualTo(Guid.Empty));
	}

	[Test]
	public void Into_EmptyGuid_When_Input_Is_Null_By_Default()
	{
		// Arrange
		var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile())));
		var c1 = new ClassWithNullableGuidPropertyCalledGuid1();
		c1.Guid1 = null;

		// Act
		var c2 = mapper.Map<ClassWithTwoGuidPropertiesCalledGuid1AndGuid2>(c1);

		// Assert
		Assert.That(c2.Guid1, Is.EqualTo(Guid.Empty));
	}
}
