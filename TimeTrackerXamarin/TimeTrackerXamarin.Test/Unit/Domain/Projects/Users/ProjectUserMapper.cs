using System.Collections.Generic;
using Newtonsoft.Json;
using TimeTrackerXamarin._Domains.API.dto;
using TimeTrackerXamarin._Domains.Projects.Users;
using TimeTrackerXamarin._UseCases.Contracts;
using Xunit;

namespace TimeTrackerXamarin.Test.Unit.Domain.Projects.Users
{
    public class ProjectUserMapperTest
    {
        private ProjectUserMapper mapper;
        public ProjectUserMapperTest()
        {
            mapper = new ProjectUserMapper();
        }

        /**
         * @feature Projects
         * @case Users
         * @scenario Map project users with empty list 
         */
        [Fact]
        public void Map_null()
        {
            //GIVEN
            List<ProjectUser> givenList = null;
            
            //WHEN
            var result = mapper.Map(givenList);
            
            //THEN
            Assert.Null(result);
        }
        /**
         * @feature Projects
         * @case Users
         * @scenario Map project users with list
         */
        [Fact]
        public void Map_List_User()
        {
            //GIVEN
            var givenList = new List<ProjectUser>();
            givenList.Add(
                new ProjectUser
                {
                    id=15,
                    user_id = 15,
                    project_id = 15,
                    user = new JSONDataDto<User>
                    {
                        data = new User
                        {
                            first_name = "Adam", 
                            last_name = "Swoboda"
                        }
                    }
                });
            
            //WHEN
            var result = mapper.Map(givenList);
            
            //THEN
            Assert.Single(result);
            Assert.Equal(givenList[0].user.data.first_name,result[0].first_name);
        }
        
        /**
         * @feature Projects
         * @case Users
         * @scenario Map DB project users with list
         */
        [Fact]
        public void MapDB_List_User()
        {
            //GIVEN
            var givenList = new List<ProjectUser>();
            givenList.Add(
                new ProjectUser
                {
                    id=15,
                    user_id = 15,
                    project_id = 15,
                    user = new JSONDataDto<User>
                    {
                        data = new User
                        {
                            first_name = "Adam", 
                            last_name = "Swoboda"
                        }
                    },
                    userDB = JsonConvert.SerializeObject(new User
                    {
                        first_name = "Adam", 
                        last_name = "Swoboda"
                    })
                });
            
            //WHEN
            var result = mapper.MapDB(givenList);
            
            //THEN
            Assert.Single(result);
            Assert.Equal(givenList[0].user.data.first_name,result[0].first_name);
            Assert.Equal(givenList[0].user.data.last_name,result[0].last_name);
        }
        
        /**
         * @feature Projects
         * @case Users
         * @scenario Map DB project users with list
         */
        [Fact]
        public void MapDB_null()
        {
            //GIVEN
            var givenList = new List<ProjectUser>();
            
            //WHEN
            var result = mapper.MapDB(givenList);
            
            //THEN
            Assert.Null(result);
        }
    }
}
