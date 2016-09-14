namespace SMS.EPA.Services.UnitTests
{
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Criteria.ScoreRatingType;
    using SMS.EPA.Model.ScoreRatingType;
    using SMS.EPA.Services.Implement;
    using System.Collections.Generic;

    [TestFixture]
    public class ScoreRatingTypeServiceTests
    {
        [Test]
        public void GetScoreRatingType_AnyArg_ReturnExpectedRatingTypes()
        {
            // Arrange
            var expectedRatingTypes = new List<ScoreRatingTypeModel>();
            expectedRatingTypes.Add(new ScoreRatingTypeModel());
            var scoreRatingTypeService = ArrangeGetScoreRatingType(expectedRatingTypes);

            // Act
            var actual = scoreRatingTypeService.GetScoreRatingType(Arg.Any<GetScoreRatingTypeCriteria>());

            // Assert
            Assert.AreEqual(expectedRatingTypes, actual);
        }

        private ScoreRatingTypeService ArrangeGetScoreRatingType(IEnumerable<ScoreRatingTypeModel> ratingTypes = null)
        {
            var scoreRatingTypeService = Substitute.For<ScoreRatingTypeService>();
            scoreRatingTypeService.GetData<ScoreRatingTypeModel>(Arg.Any<GetScoreRatingTypeCriteria>()).Returns(ratingTypes ?? new List<ScoreRatingTypeModel>());
            return scoreRatingTypeService;
        }
    }
}
