namespace SMS.EPA.Services.UnitTests
{
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;
    using System.Collections.Generic;

    [TestFixture]
    public class MenuServiceTests
    {
        private IMenuService menuService;

        #region GetMenusByEmployeeId
        [Test]
        public void GetMenusByEmployeeId_AnyArg_ReturnInstanceOfListOfMenuGroupModel()
        {
            // Arrange
            menuService = Substitute.For<IMenuService>();

            // Act
            var actual = menuService.GetMenusByEmployeeId(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(IList<MenuGroupModel>), actual);
        }

        [Test]
        public void GetMenusByEmployeeId_NoParent_ReturnExpectedMenuGroup()
        {
            // Arrange
            var expectedMenuGroups = new List<MenuItemModel>();
            expectedMenuGroups.Add(new MenuItemModel() { ParentId = null, MenuId = 1, DisplayOrder = 2 });
            expectedMenuGroups.Add(new MenuItemModel() { ParentId = null, MenuId = 2, DisplayOrder = 3 });
            expectedMenuGroups.Add(new MenuItemModel() { ParentId = null, MenuId = 3, DisplayOrder = 1 });

            var menuGroups = new List<MenuGroupModel>();
            menuGroups.Add(new MenuGroupModel() { ParentId = null, MenuId = 3, DisplayOrder = 1 });
            menuGroups.Add(new MenuGroupModel() { ParentId = null, MenuId = 1, DisplayOrder = 2 });
            menuGroups.Add(new MenuGroupModel() { ParentId = null, MenuId = 2, DisplayOrder = 3 });

            var menuService = ArrangeGetMenusByEmployeeId(expectedMenuGroups);

            // Act
            var actual = menuService.GetMenusByEmployeeId(1);

            // Assert
            Assert.AreEqual(menuGroups[0].MenuId, actual[0].MenuId);
            Assert.AreEqual(menuGroups[1].MenuId, actual[1].MenuId);
            Assert.AreEqual(menuGroups[2].MenuId, actual[2].MenuId);
        }

        [Test]
        public void GetMenusByEmployeeId_HaveParent_ReturnExpectedMenuGroup()
        {
            // Arrange
            var expectedMenuGroups = new List<MenuItemModel>();
            expectedMenuGroups.Add(new MenuItemModel() { ParentId = null, MenuId = 1, DisplayOrder = 2 });
            expectedMenuGroups.Add(new MenuItemModel() { ParentId = 1, MenuId = 2, DisplayOrder = 3 });
            expectedMenuGroups.Add(new MenuItemModel() { ParentId = null, MenuId = 3, DisplayOrder = 1 });

            var menuGroups = new List<MenuGroupModel>();
            menuGroups.Add(new MenuGroupModel() { ParentId = null, MenuId = 3, DisplayOrder = 1 });
            menuGroups.Add(new MenuGroupModel() { ParentId = null, MenuId = 1, DisplayOrder = 2 });
            menuGroups[1].MenuItems = new List<MenuItemModel>()
            {
                new MenuGroupModel() { ParentId = 1, MenuId = 2, DisplayOrder = 3 }
            };

            var menuService = ArrangeGetMenusByEmployeeId(expectedMenuGroups);

            // Act
            var actual = menuService.GetMenusByEmployeeId(1);

            // Assert
            Assert.AreEqual(menuGroups[0].MenuId, actual[0].MenuId);
            Assert.AreEqual(menuGroups[1].MenuId, actual[1].MenuId);
            Assert.AreEqual(menuGroups[1].MenuItems[0].MenuId, actual[1].MenuItems[0].MenuId);
        }

        #endregion

        private MenuService ArrangeGetMenusByEmployeeId(IList<MenuItemModel> menuGroups = null)
        {
            var menuService = Substitute.For<MenuService>();
            menuService.GetData<MenuItemModel>(Arg.Any<GetMenusByEmployeeIdCriteria>()).Returns(menuGroups ?? new List<MenuItemModel>());
            return menuService;
        }
    }
}
