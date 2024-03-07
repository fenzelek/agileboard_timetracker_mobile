using System;
using System.Collections.Generic;
using TimeTrackerXamarin.i18n;
using TimeTrackerXamarin.Services;
using Xunit;

namespace TimeTrackerXamarin.Test.Unit.Services
{
    public class DateTimeConverterTest
    {
        private DateTimeConverter converter;
        public DateTimeConverterTest()
        {
            converter = new DateTimeConverter();
        }

        /*
        * @feature Services
        * @scenario Convert date to proper format and language
        * @case Formatted date
        */
        [Fact]
        public void Convert_stringdate()
        {
            //GIVEN

            //WHEN
            var result = new List<string>()
            {
                converter.ConvertTest(new DateTime(2022, 6, 6), Language.English) as string,
                converter.ConvertTest(new DateTime(2022, 6, 6), Language.Polish) as string,
                converter.ConvertTest(new DateTime(2022, 6, 6), Language.Ukrainian) as string,
            };
            //THEN
            Assert.IsType<string>(result[0]);
            Assert.IsType<string>(result[1]);
            Assert.IsType<string>(result[2]);
            Assert.Equal("Monday, 06.06.2022", result[0]);
            Assert.Equal("poniedziałek, 06.06.2022", result[1]);
            Assert.Equal("понеділок, 06.06.2022", result[2]);

        }
    }
}