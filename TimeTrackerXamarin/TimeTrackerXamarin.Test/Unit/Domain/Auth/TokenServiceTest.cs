using System;
using Flurl.Http;
using Moq;
using TimeTrackerXamarin._Domains.Auth;
using Xamarin.Essentials.Interfaces;
using Xunit;

namespace TimeTrackerXamarin.Test.Unit.Domain.Auth
{
    public class TokenServiceTest
    {
        private readonly Mock<IPreferences> preferences; 
        private readonly Mock<IFlurlClient> flurlClient; 
        private TokenService tokenService;

        public TokenServiceTest()
        {
            preferences = new Mock<IPreferences>();
            flurlClient = new Mock<IFlurlClient>();

            tokenService = new TokenService(flurlClient.Object, preferences.Object);
        }
        
        /*
         * @feature: Auth
         * @scenario: Get saved token
         * @case: Token
         */
        [Fact]
        public void Get_Token()
        {
            //GIVEN
            string expectedToken = "token";
            preferences.Setup((x) => x.Get("token", "")).Returns("token");
            preferences.Setup((x) => x.ContainsKey("token")).Returns(true);
            preferences.Setup((x) => x.ContainsKey("token_time")).Returns(true);

            //WHEN
            var result = tokenService.Get();

            //THEN
            Assert.Equal(expectedToken,result);
        }
        
        /*
         * @feature: Auth
         * @scenario: Get saved token failure
         * @case: Empty string
         */
        [Fact]
        public void Get_empty()
        {
            //GIVEN
            string expectedToken = "";
            preferences.Setup((x) => x.Get("token", "")).Returns("token");
            preferences.Setup((x) => x.ContainsKey("token")).Returns(false);
            preferences.Setup((x) => x.ContainsKey("token_time")).Returns(false);

            //WHEN
            var result = tokenService.Get();

            //THEN
            Assert.Equal(expectedToken,result);
        }
        
        /*
         * @feature: Auth
         * @scenario: Set token 
         * @case: Token not set, throw exception
         */
        [Theory]
        [InlineData("token")]
        [InlineData("token.token")]
        public void Set_Failure(string token)
        {
            //GIVEN
            string expectedToken = "token";
            
            //WHEN & THEN
            Assert.Throws<Exception>(()=>tokenService.Set("token"));
            
            //THEN
            preferences.Verify((x)=>x.Set("token_time", "token"), Times.Never);
            preferences.Verify((x)=>x.Set("token", "token"), Times.Never);
        }
        
        /*
         * @feature: Auth
         * @scenario: Set token 
         * @case: Token set, save expire date & token
         */
        [Fact]
        public void Set_Success()
        {
            //GIVEN
            string expectedToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJodHRwczovL2FwaS5hZ2lsZWJvYXJkLm1lL2F1dGgiLCJpYXQiOjE2NjkwNDAzODQsImV4cCI6MTY3MTYzMjM4NCwibmJmIjoxNjY5MDQwMzg0LCJqdGkiOiJBZ0tUbFlUVEtVMnh2NEk2Iiwic3ViIjo0MjksInBydiI6IjZiZGVlNDg5ZGYwODE3ZWI3NThjMDM1YjlmNzkzZGViNDBmMzFhY2QifQ.8EJyjWVfis1cJME0QJ0b_nKhc3vgzYhkoxXsf4MIGnI";
            
            //WHEN
            tokenService.Set(expectedToken);
            
            //THEN
            preferences.Verify((x)=>x.Set("token_time", "1671632384"), Times.Once);
            preferences.Verify((x)=>x.Set("token", expectedToken), Times.Once);
        }
        
        /*
         * @feature: Auth
         * @scenario: Remove token 
         * @case: Token removed
         */
        [Fact]
        public void Remove_Success()
        {
            //GIVEN
            //
            
            //WHEN
            tokenService.Remove();
            
            //THEN
            preferences.Verify((x)=>x.Remove("token_time"), Times.Once);
            preferences.Verify((x)=>x.Remove("token"), Times.Once);
        }
        
    }
}