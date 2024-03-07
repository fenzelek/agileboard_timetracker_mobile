using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using SQLite;
using TimeTrackerXamarin._Domains.API.dto;
using TimeTrackerXamarin._Domains.Projects.Users;
using TimeTrackerXamarin._UseCases.Contracts;
using Xunit;

namespace TimeTrackerXamarin.Test.Integration.Domain.Projects.Users
{
    public class LocalUserSourceTest : IDisposable
    {
        private readonly SQLiteAsyncConnection localDatabase;
        private readonly LocalUserDataSource source;
        private readonly Mock<IDatabaseConnector> connector;

        public LocalUserSourceTest()
        {
            localDatabase = new SQLiteAsyncConnection(":memory:");
            connector = new Mock<IDatabaseConnector>();
            connector.Setup((db) => db.Create())
                .Returns(localDatabase);
            source = new LocalUserDataSource(connector.Object);
        }

        /*
         * @feature Projects
         * @scenario Get all users
         * @case Gets users from local database    
         */
        [Fact]
        public async void GetAll_ListUsers()
        {
            // GIVEN
            var expectedUser = new ProjectUser
            {
                id = 1,
                user_id = 2,
                project_id = 3
            };
            
            await localDatabase.InsertAsync(expectedUser);

            // WHEN
            var result = await source.GetProjectUsers(expectedUser.project_id);
            var first = result.First();

            // THEN
            Assert.Equal(expectedUser.user_id, first.user_id);
            Assert.Equal(expectedUser.project_id, first.project_id);
            Assert.Equal(expectedUser.id, first.id);
        }

        /*
         * @feature Projects
         * @scenario Save user
         * @case Saves user to local database
         */
        [Fact]
        public async void SaveUser()
        {
            // GIVEN
            var expectedUser = new ProjectUser
            {
                id = 1,
                user_id = 2,
                project_id = 3,
                user = new JSONDataDto<User>
                {
                    data = new User
                    {
                        id = 4,
                    }
                }
            };

            // WHEN
            var success = await source.SaveProjectUsers(new List<ProjectUser>
            {
                expectedUser
            });

            // THEN
            Assert.True(success);
            
            var result = await localDatabase.Table<ProjectUser>().FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.Equal(expectedUser.user_id, result.user_id);
            Assert.Equal(expectedUser.project_id, result.project_id);
            Assert.Equal(expectedUser.id, result.id);
        }

        public void Dispose()
        {
            localDatabase.CloseAsync();
        }
    }
}